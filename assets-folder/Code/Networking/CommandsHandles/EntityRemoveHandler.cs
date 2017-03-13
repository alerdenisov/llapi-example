using Zenject;
using UniRx;
using UnityEngine;
using System;

namespace LlapiExample
{
    public class EntityRemoveHandler : BaseCommandHandler<EntityRemove>
    {
        private DiContainer container;
        private OwnTable table;

        public EntityRemoveHandler(IncomingCommandsQueue incomings, DiContainer container, OwnTable table) : base(incomings)
        {
            this.container = container;
            this.table = table;
        }

        protected override void OnCommand(EntityRemove command)
        {
            var owner = table.Owner(command.id);
            if (owner == -1)
            {
                Debug.LogError("Owner not defined: " + command.id);
                return;
            }

            var repository = container.ResolveId<CommanderStatus>(owner);
            repository.Destroy(command.id);
        }
    }
}
