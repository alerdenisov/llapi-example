using UnityEngine;
using System.Collections.Generic;

public class Crate : BaseEntity
{
    public override float CurrentHealth
    {
        get
        {
            return health;
        }
    }

    public override float MaxHealth
    {
        get { return 60f; }
    }

    private float health;

    [SerializeField]
    private List<Rigidbody> demolitionParts;
    [SerializeField]
    private List<Rigidbody> allParts;

    private const float partsPerDamage = 0.25f;

    private void Start()
    {
        health = MaxHealth;
    }

    public override void DamageReceive(float damage, Vector3 point, Vector3 force)
    {
        health -= damage;
        if (health > 0)
        {
            var demolitionCount = Mathf.FloorToInt(damage * partsPerDamage);
            while(demolitionCount > 0)
            {
                var part = demolitionParts[Random.Range(0, demolitionParts.Count)];
                demolitionParts.Remove(part);
                allParts.Remove(part);
                Demolition(part, force * 2.5f, point);
                demolitionCount--;
            }
        }
        else
        {
            foreach(var part in allParts)
            {
                Demolition(part, force, point);
            }

            Destroy(gameObject);
        }
    }

    private void Demolition(Rigidbody part, Vector3 force, Vector3 point)
    {
        part.transform.parent = null;
        part.GetComponent<BoxCollider>().enabled = true;
        part.isKinematic = false;
        part.useGravity = true;
        part.gameObject.layer = 11;// "Debris";
        part.AddForceAtPosition(force, point, ForceMode.Impulse);
        Destroy(part.gameObject, Random.Range(1.5f, 2f));
    }
}
