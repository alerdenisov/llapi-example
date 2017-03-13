using Zenject;
using UniRx;
using UnityEngine;

namespace LlapiExample
{
    public class InitializeGameObjectsLogic : INetworkLogic
    {
        [Inject]
        private ConnectionStatus status;

        [Inject]
        private GameplayCamera camera;

        [Inject]
        private FirererPlayerInput playerInput;

        public void Setup()
        {
            Object.DontDestroyOnLoad(camera.gameObject);
            Object.DontDestroyOnLoad(playerInput.gameObject);
            OnStatusChange(status.Current);
            status.Observable.Subscribe(OnStatusChange);
        }

        public void OnStatusChange(ConnectionState state)
        {
            playerInput.enabled = status.InGame && !status.IsPaused;
        }

        public void Update()
        {
        }
    }
}
