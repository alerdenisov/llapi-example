using UnityEngine;
using System.Collections;
using System;

namespace LlapiExample
{
    public class EntityRemove : BaseCommand, ICommand
    {
        public Guid id;

        public EntityRemove() : base(CommandIds.Entity_Remove)
        {

        }

        public object[] Data()
        {
            return new object[] {
                id.ToString()
            };
        }

        public void Read(object[] buffer, ref int position)
        {
            id = new Guid((string)buffer[++position]);
        }
    }
}