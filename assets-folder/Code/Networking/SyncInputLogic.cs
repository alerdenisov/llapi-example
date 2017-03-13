using Zenject;
using UniRx;
using UnityEngine;
using System;

namespace LlapiExample
{
    public class SyncInputLogic : INetworkLogic
    {
        [Inject] private OutgoingCommandsQueue outgoings;
        [Inject(Id = 0)] private CommanderStatus character;
        [Inject] private DiContainer container;

        public SyncInputLogic()
        {

        }

        public void OnShoot()
        {
            outgoings.Enqueue(container.Instantiate<CharacterShoot>());
        }

        public void Setup()
        {
            character.ObservableCharacter.Subscribe(OnCharacter);
        }

        private void OnCharacter(Firerer character)
        {
            if (!character) return;

            character.OnShoot += OnShoot;
            character.OnBuild += OnShoot;
        }

        public void Update()
        {
            if (!character.Character)
                return;

            outgoings.Enqueue(container.Instantiate<CharacterLook>());
            outgoings.Enqueue(container.Instantiate<CharacterMove>());
        }
    }
}