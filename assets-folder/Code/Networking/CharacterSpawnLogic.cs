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
        [Inject(Id = 0)] private CommanderStatus characterStatus;
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

            characterStatus.SetTeam(status.IsClient ? Team.TeamB : Team.TeamA);

            characterStatus.ObservableCharacter.Subscribe(OnCharacter);

            respawnView.OnRespawn += OnRespawnButton;
            playerInput.Camera = camera;

            OnPlayStatusChange(playStatus.Current);
        }

        private void OnRespawnButton()
        {
            playStatus.Set(PlayState.Respawn);
        }

        public void OnCharacter(Firerer character)
        {
            if(character)
            {
                playerInput.Firerer = character;
                character.OnDie += OnDie;
            }
        }

        private void OnDie()
        {
            playStatus.Set(PlayState.Dead);
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
                var x = UnityEngine.Random.Range(-5f, 5f);
                var z = UnityEngine.Random.Range(16f, 22f);
                var side = characterStatus.Team == Team.TeamA ? -1f : 1f;

                var position = new Vector3(x, 0, z * side);
                var entity = characterStatus.SpawnCharacter(position);
                var cmd = new EntityCreate();
                cmd.prefab = PrefabType.Character;
                cmd.id = entity.Id;
                outgoings.Enqueue(cmd);
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
