using UnityEngine;
using System.Collections;

public abstract class BaseEntity : MonoBehaviour
{
    public abstract float CurrentHealth { get; }
    public abstract float MaxHealth { get; }
    public abstract void DamageReceive(float damage, Vector3 point, Vector3 force);
}
