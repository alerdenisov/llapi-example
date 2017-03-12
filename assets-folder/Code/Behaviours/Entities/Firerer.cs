using UnityEngine;
using System.Collections;
using System;

namespace LlapiExample
{
    [RequireComponent(typeof(FirererController))]
    public class Firerer : BaseEntity
    {
        #region Dependencies
        private FirererController cachedController;
        protected FirererController Controller
        {
            get
            {
                if (!cachedController)
                    cachedController = GetComponent<FirererController>();
                return cachedController;
            }
        }
        #endregion

        public override float CurrentHealth
        {
            get
            {
                return health;
            }
        }

        public override float MaxHealth
        {
            get { return 100f; }
        }

        private float health;

        private void Start()
        {
            health = MaxHealth;
        }

        public override void DamageReceive(float damage, Vector3 point, Vector3 force)
        {
            health -= damage;
            if(health > 0)
            {
                Controller.TakenDamage();
            }
            else
            {
                Controller.Die();
            }
        }
    }
}