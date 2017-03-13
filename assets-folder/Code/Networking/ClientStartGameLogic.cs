using Zenject;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LlapiExample
{
    public class ClientStartGameLogic : INetworkLogic, IClientLogic
    {
        [Inject]
        private IncomingCommandsQueue incomings;
        [Inject]
        private ConnectionStatus status;
        [Inject]
        private ConnectionsRepository connections;

        public ClientStartGameLogic()
        {
        }

        public void Update()
        {
        }

        public void Setup()
        {
            incomings.Observable.Subscribe(OnMessageIncome);
            connections.Connections.Subscribe(OnConnected);
            connections.Disconnections.Subscribe(OnDisconnected);
        }

        private void OnConnected(int id)
        {
            status.Remove(ConnectionState.Disconnected);
            status.Remove(ConnectionState.Connecting);
            status.Append(ConnectionState.Connected);
        }

        private void OnDisconnected(int id)
        {
            status.Set(ConnectionState.Disconnected);
        }


        private void OnMessageIncome(ICommand cmd)
        {
            if(cmd is StartGame)
            {
                var task = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
                var time = 0f;
                var timer = new System.Diagnostics.Stopwatch();
                timer.Start();
                while (!task.isDone && time < 1f)
                {
                    time += timer.ElapsedMilliseconds / 1000f;
                }
                timer.Stop();

                SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(1));

                status.Append(ConnectionState.InGame);
            }
        }
    }
}