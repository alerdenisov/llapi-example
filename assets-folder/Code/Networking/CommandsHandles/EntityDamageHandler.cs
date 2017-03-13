using Zenject;
using UniRx;
using UnityEngine;
using System;

namespace LlapiExample
{
    public class EntityDamageHandler : BaseCommandHandler<EntityDamage>
    {
        private DiContainer container;

        public EntityDamageHandler(IncomingCommandsQueue incomings, DiContainer container) : base(incomings)
        {
            this.container = container;
        }

        protected override void OnCommand(EntityDamage command)
        {
            var owner = command.Connection;
            var repository = container.ResolveId<CommanderStatus>(command.Connection);

            if (!repository.Character)
            {
                return;
            }

            //repository.Character.SpawnCrate(command.position, command.size);
        }
    }
}
