using Zenject;
using UniRx;
using UnityEngine;
using System;

namespace LlapiExample
{
    public class CharacterBulletHandler : BaseCommandHandler<CharacterBullet>
    {
        private DiContainer container;

        public CharacterBulletHandler(IncomingCommandsQueue incomings, DiContainer container) : base(incomings)
        {
            this.container = container;
        }

        protected override void OnCommand(CharacterBullet command)
        {
            var owner = command.Connection;
            var repository = container.ResolveId<CommanderStatus>(command.Connection);

            if (!repository.Character)
            {
                return;
            }

            //repository.Character.SpawnBullet(command.position, command.forward);
        }
    }
}
