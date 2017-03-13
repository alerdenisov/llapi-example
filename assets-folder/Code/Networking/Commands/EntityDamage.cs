using UnityEngine;
using System.Collections;
using System;

namespace LlapiExample
{
    public class EntityDamage : BaseCommand, ICommand
    {
        public Guid id;
        public float amount;

        public EntityDamage() : base(CommandIds.Entity_Damage)
        {

        }

        public object[] Data()
        {
            return new object[] {
                amount,
                id.ToString()
            };
        }

        public void Read(object[] buffer, ref int position)
        {
            amount = (float)buffer[++position];
            id = new Guid((string)buffer[++position]);
        }
    }
}