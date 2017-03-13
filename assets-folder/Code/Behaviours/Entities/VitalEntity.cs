using UnityEngine;
using System.Collections;

namespace LlapiExample
{
    public abstract class VitalEntity : NetworkEntity
    {
        public abstract float CurrentHealth { get; }
        public abstract float MaxHealth { get; }
        public abstract void DamageReceive(float damage, Vector3 point, Vector3 force);
    }
}