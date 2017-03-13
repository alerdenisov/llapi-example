using UnityEngine;
using System.Collections;
using System;

namespace LlapiExample
{
    public enum PrefabType : byte {
        Character,
        Crate,
        Bullet
    }

    public class EntityCreate : BaseCommand, ICommand
    {
        public Guid id;
        public PrefabType prefab;
        public Vector3 position;
        public Vector3 forward;

        public EntityCreate() : base(CommandIds.Entity_Create)
        {

        }

        public object[] Data()
        {
            return new object[] {
                (byte)prefab,
                id.ToString(),
                position,
                forward
            };
        }

        public void Read(object[] buffer, ref int position)
        {
            prefab = (PrefabType)(byte)buffer[++position];
            id = new Guid((string)buffer[++position]);
            this.position = (Vector3)buffer[++position];
            forward = (Vector3)buffer[++position];
        }
    }
}