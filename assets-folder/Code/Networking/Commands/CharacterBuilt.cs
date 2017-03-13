using UnityEngine;
using System.Collections;
using System;

namespace LlapiExample
{
    public class CharacterBuilt : BaseCommand, ICommand
    {
        public CharacterBuilt() : base(CommandIds.Character_Built)
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