using UnityEngine;
using System.Collections;
using System;

namespace LlapiExample
{
    public class CharacterSpawn : BaseCommand, ICommand
    {
        private Team team;

        public Team Team { get { return team; } }

        public CharacterSpawn(PlayStatus status) : base(CommandIds.Character_Spawn)
        {
            team = status.Team;
        }

        public object[] Data()
        {
            return new object[] {
                (byte)team
            };
        }

        public void Read(object[] buffer, ref int position)
        {
            team = (Team)(byte)buffer[++position];
        }
    }
}