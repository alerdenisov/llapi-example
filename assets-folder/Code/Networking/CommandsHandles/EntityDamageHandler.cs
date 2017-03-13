using Zenject;
using UniRx;
using UnityEngine;
using System;

namespace LlapiExample
{
    public class EntityDamageHandler : BaseCommandHandler<EntityDamage>
    {
        private DiContainer container;
        private OwnTable table;

        public EntityDamageHandler(IncomingCommandsQueue incomings, DiContainer container, OwnTable table) : base(incomings)
        {
            this.container = container;
            this.table = table;
        }

        protected override void OnCommand(EntityDamage command)
        {
            var owner = table.Owner(command.id);
            if(owner == -1)
            {
                Debug.LogError("Owner not defined: " + command.id);
                return;
            }

            var repository = container.ResolveId<CommanderStatus>(owner);
            repository.Damage(command.id, command.amount);
        }
    }
}
