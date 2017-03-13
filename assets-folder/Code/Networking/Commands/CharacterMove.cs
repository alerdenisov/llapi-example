using UnityEngine;
using System.Collections;
using System;
using Zenject;

namespace LlapiExample
{
    public class CharacterMove : BaseCommand, ICommand
    {
        public Vector3 position;
        public Vector3 destination;

        [Inject(Id = 0)] private CommanderStatus status;

        public CharacterMove() : base(CommandIds.Character_Move)
        {

        }

        public object[] Data()
        {
            return new object[] {
                status.Character.transform.position,
                status.Character.Agent.destination
            };
        }

        public void Read(object[] buffer, ref int position)
        {
            this.position = (Vector3)buffer[++position];
            destination =   (Vector3)buffer[++position];

        }
    }
}