using System;
using System.Linq;
using UniRx;
using UnityEngine.SceneManagement;

namespace LlapiExample
{
    public class ServerStartGameLogic : INetworkLogic, IServerLogic
    {
        private StartGameView startGame;
        private OutgoingCommandsQueue outgoings;
        private ConnectionsRepository connections;
        private ConnectionStatus status;

        public ServerStartGameLogic(
            StartGameView startGame,
            ConnectionsRepository connections,
            OutgoingCommandsQueue outgoings,
            ConnectionStatus status)
        {
            this.startGame = startGame;
            this.connections = connections;
            this.outgoings = outgoings;
            this.status = status;
        }

        private void OnStatusChange(ConnectionState state)
        {
            if(status.IsReady && !status.InGame)
            {
                startGame.Show();
            } else
            {
                startGame.Hide();
            }
        }

        private void OnStartGame()
        {
            outgoings.Enqueue(new StartGame());
            var task = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
            var time = 0f;
            var timer = new System.Diagnostics.Stopwatch();
            timer.Start();
            while(!task.isDone && time < 1f)
            {
                time += timer.ElapsedMilliseconds / 1000f;
            }
            timer.Stop();

            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(1));
            status.Append(ConnectionState.InGame);
        }

        public void Update()
        {
        }

        public void Setup()
        {
            startGame.OnStartGame += OnStartGame;
            status.Observable.Subscribe(OnStatusChange);
        }
    }
}