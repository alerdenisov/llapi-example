using Zenject;
using UniRx;
using UnityEngine;
using System;

namespace LlapiExample
{
    public class CharacterLookHandler : BaseCommandHandler<CharacterLook>
    {
        private Firerer firererPrefab;
        private DiContainer container;

        public CharacterLookHandler(IncomingCommandsQueue incomings, Firerer firererPrefab, DiContainer container) : base(incomings)
        {
            this.firererPrefab = firererPrefab;
            this.container = container;
        }

        protected override void OnCommand(CharacterLook command)
        {
            var owner = command.Connection;
            var repository = container.ResolveId<CharacterStatus>(command.Connection);

            if (!repository.Controller)
            {
                return;
            }

            repository.Controller.ShootDirection(command.shootDirection);
        }
    }
}
