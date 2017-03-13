using UnityEngine;
using System.Collections;
using System;

namespace LlapiExample
{
    public class StartGame : BaseCommand, ICommand
    {
        public StartGame() : base(CommandIds.Game_Start)
        {

        }

        public object[] Data()
        {
            return new object[0];
        }

        public void Read(object[] buffer, ref int offset)
        {
        }
    }
}