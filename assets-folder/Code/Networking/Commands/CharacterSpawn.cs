using UnityEngine;
using System.Collections;
using System;

namespace LlapiExample
{
    public class CharacterSpawn : BaseCommand, ICommand
    {
        public CharacterSpawn() : base(CommandIds.Character_Spawn)
        {
            //team = status.Team;
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