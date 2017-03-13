using UnityEngine;
using System.Collections;
using System;

namespace LlapiExample
{
    public abstract class BaseCommand
    {
        public int Connection { set; get; }
        public byte CommandId { get { return id; } }

        private byte id;

        public BaseCommand(byte id)
        {
            this.id = id;
        }
    }
}