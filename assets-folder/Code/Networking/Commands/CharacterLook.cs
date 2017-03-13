using UnityEngine;
using System.Collections;
using System;
using Zenject;

namespace LlapiExample
{
    public class CharacterLook : BaseCommand, ICommand
    {
        public Vector3 shootDirection;
        [Inject(Id = 0)] private CommanderStatus status;

        public CharacterLook() : base(CommandIds.Character_Look)
        {
        }

        public object[] Data()
        {
            return new object[]
            {
                status.Character.shootDirection
            };
        }

        public void Read(object[] buffer, ref int position)
        {
            shootDirection = (Vector3)buffer[++position];
        }
    }
}