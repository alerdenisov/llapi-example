using Zenject;
using UniRx;
using UnityEngine;
using System;

namespace LlapiExample
{
    public class CharacterSpawnHadler : BaseCommandHandler<CharacterSpawn>
    {
        private Firerer firererPrefab;
        private DiContainer container;

        public CharacterSpawnHadler(IncomingCommandsQueue incomings, Firerer firererPrefab, DiContainer container) : base(incomings)
        {
            this.firererPrefab = firererPrefab;
            this.container = container;
        }

        protected override void OnCommand(CharacterSpawn command)
        {
            // TODO: :D
            var position = command.Team == Team.TeamA ? new Vector3(0, 0, 23f) : new Vector3(0, 0, -21f);

            var characterGo = container.InstantiatePrefab(firererPrefab);
            var character = characterGo.GetComponent<Firerer>();
            var controller = character.GetComponent<FirererController>();

            var repository = container.ResolveId<CharacterStatus>(command.Connection);
            repository.Character = character;
            repository.Controller = controller;
        }
    }
}
