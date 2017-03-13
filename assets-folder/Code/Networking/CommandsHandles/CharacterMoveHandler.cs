using Zenject;
using UniRx;
using UnityEngine;
using System;

namespace LlapiExample
{
    public class CharacterMoveHandler : BaseCommandHandler<CharacterMove>
    {
        private Firerer firererPrefab;
        private DiContainer container;

        public CharacterMoveHandler(IncomingCommandsQueue incomings, Firerer firererPrefab, DiContainer container) : base(incomings)
        {
            this.firererPrefab = firererPrefab;
            this.container = container;
        }

        protected override void OnCommand(CharacterMove command)
        {
            var owner = command.Connection;
            var repository = container.ResolveId<CommanderStatus>(command.Connection);

            if (!repository.Character)
            {
                return;
            }

            repository.Character.Translate(command.position);
            repository.Character.Move(command.destination);
        }
    }
}
