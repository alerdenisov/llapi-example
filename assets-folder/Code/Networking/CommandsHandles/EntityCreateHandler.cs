using Zenject;
using UniRx;
using UnityEngine;
using System;

namespace LlapiExample
{
    public class EntityCreateHandler : BaseCommandHandler<EntityCreate>
    {
        private DiContainer container;

        public EntityCreateHandler(IncomingCommandsQueue incomings, DiContainer container) : base(incomings)
        {
            this.container = container;
        }

        protected override void OnCommand(EntityCreate command)
        {
            var owner = command.Connection;
            var repository = container.ResolveId<CommanderStatus>(command.Connection);
            repository.SpawnPrefab(command.prefab, command.position, command.forward, command.id);
        }
    }
}
