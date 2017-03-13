using Zenject;
using UniRx;
using UnityEngine;
using System;

namespace LlapiExample
{
    public class CharacterShootHandler : BaseCommandHandler<CharacterShoot>
    {
        private DiContainer container;

        public CharacterShootHandler(IncomingCommandsQueue incomings, DiContainer container) : base(incomings)
        {
            this.container = container;
        }

        protected override void OnCommand(CharacterShoot command)
        {
            var owner = command.Connection;
            var repository = container.ResolveId<CommanderStatus>(command.Connection);

            if (!repository.Character)
            {
                return;
            }

            repository.Character.Shoot();
        }
    }
}
