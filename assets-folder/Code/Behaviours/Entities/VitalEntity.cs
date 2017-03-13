using UnityEngine;
using System.Collections;
using Zenject;

namespace LlapiExample
{
    public abstract class VitalEntity : NetworkEntity
    {
        public abstract float CurrentHealth { get; }
        public abstract float MaxHealth { get; }
        public abstract void DamageReceive(float damage, Vector3 point, Vector3 force);

        [Inject]
        private HealthView healthPrefab;

        [Inject]
        private DiContainer container;

        [Inject]
        private Canvas canvas;

        private HealthView healthBar;

        public virtual void Start()
        {
            var instance = container.InstantiatePrefab(healthPrefab);
            instance.transform.SetParent(canvas.transform, false);
            healthBar = instance.GetComponent<HealthView>();
            healthBar.transform.localScale = Vector3.one;
            healthBar.transform.localRotation = Quaternion.identity;
        }

        public virtual void OnDestroy()
        {
            Destroy(healthBar.gameObject);
        }

        public virtual void Update()
        {
            healthBar.WorldPosition = transform.position + Vector3.up * 2f;
            healthBar.Amount = CurrentHealth / MaxHealth;
        }
    }
}