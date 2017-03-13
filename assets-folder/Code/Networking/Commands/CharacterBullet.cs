using UnityEngine;
using System.Collections;
using System;

namespace LlapiExample
{
    public class CharacterBullet : BaseCommand, ICommand
    {
        public Vector3 position;
        public Vector3 forward;

        public CharacterBullet() : base(CommandIds.Character_Bullet)
        {
        }

        public object[] Data()
        {
            return new object[]
            {
                position,
                forward
            };
        }

        public void Read(object[] buffer, ref int position)
        {
            this.position = (Vector3)buffer[++position];
            forward = (Vector3)buffer[++position];
        }
    }
}