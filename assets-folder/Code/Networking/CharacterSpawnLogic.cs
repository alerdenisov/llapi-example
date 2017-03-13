using Zenject;
using UniRx;
using UnityEngine;
using System;

namespace LlapiExample
{
    public class CharacterSpawnLogic : INetworkLogic
    {
        #region Dependencies
        [Inject] private ConnectionStatus status;
        [Inject] private GameplayCamera camera;
        [Inject] private FirererPlayerInput playerInput;
        [Inject] private PlayStatus playStatus;
        [Inject] private RespawnView respawnView;
        [Inject] private OutgoingCommandsQueue outgoings;
        [Inject(Id = 0)] private CharacterStatus characterStatus;
        [Inject] private DiContainer container;
        #endregion

        private float respawnAfter;

        public void Setup()
        {
            UnityEngine.Object.DontDestroyOnLoad(camera.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(playerInput.gameObject);

            OnStatusChange(status.Current);

            status.Observable.Subscribe(OnStatusChange);
            playStatus.Observable.Subscribe(OnPlayStatusChange);

            playStatus.SetTeam(status.IsClient ? Team.TeamB : Team.TeamA);

            characterStatus.ObservableController.Subscribe(OnCharacterController);

            respawnView.OnRespawn += OnRespawnButton;
            playerInput.Camera = camera;

            OnPlayStatusChange(playStatus.Current);
        }

        private void OnRespawnButton()
        {
            playStatus.Set(PlayState.Respawn);
        }

        public void OnCharacterController(FirererController controller)
        {
            if(controller)
            {
                playerInput.Firerer = controller;
            }
        }

        public void OnPlayStatusChange(PlayState state)
        {
            if (!status.InGame)
            {
                respawnView.Hide();
                return;
            }

            if (state != PlayState.Alive && state != PlayState.Await)
            {
                respawnView.Show();
                respawnView.Await = state != PlayState.Respawn;
            }
            else
            {
                respawnView.Hide();
            }

            if(state == PlayState.Respawn)
            {
                respawnAfter = 5f;
                respawnView.RespawnWait(respawnAfter);
            }

            if(state == PlayState.Await)
            {
                characterStatus.Spawn();
                outgoings.Enqueue(container.Instantiate<CharacterSpawn>());
            }
        }

        public void OnStatusChange(ConnectionState state)
        {
            playerInput.enabled = status.InGame && !status.IsPaused;
            OnPlayStatusChange(playStatus.Current);
        }

        public void Update()
        {
            if (playStatus.Current == PlayState.Respawn)
            {
                respawnAfter -= Time.deltaTime;
                respawnView.RespawnWait(respawnAfter);

                if (respawnAfter <= 0)
                {
                    playStatus.Set(PlayState.Await);
                }
            }
        }
    }
}
