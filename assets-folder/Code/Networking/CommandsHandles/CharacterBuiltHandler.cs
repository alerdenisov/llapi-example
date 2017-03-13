using Zenject;
using UniRx;
using UnityEngine;
using System;

namespace LlapiExample
{
    public class CharacterBuiltHandler : BaseCommandHandler<CharacterBuilt>
    {
        private DiContainer container;

        public CharacterBuiltHandler(IncomingCommandsQueue incomings, DiContainer container) : base(incomings)
        {
            this.container = container;
        }

        protected override void OnCommand(CharacterBuilt command)
        {
            var owner = command.Connection;
            var repository = container.ResolveId<CommanderStatus>(command.Connection);

            if (!repository.Character)
            {
                return;
            }

            //repository.SpawnCrate(command.position, command.size);
        }
    }
}
