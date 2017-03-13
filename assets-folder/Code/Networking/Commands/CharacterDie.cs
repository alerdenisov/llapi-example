using UnityEngine;
using System.Collections;
using System;

namespace LlapiExample
{
    public class CharacterDie : BaseCommand, ICommand
    {
        public CharacterDie() : base(CommandIds.Character_Die)
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