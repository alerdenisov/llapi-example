using UnityEngine;
using System.Collections;
using System;

namespace LlapiExample
{
    public class CharacterShoot : BaseCommand, ICommand
    {
        public CharacterShoot() : base(CommandIds.Character_Shoot)
        {

        }

        public object[] Data()
        {
            return new object[0];
        }

        public void Read(object[] buffer, ref int position)
        {
        }
    }
}