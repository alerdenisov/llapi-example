using UnityEngine;
using System.Collections;
using Zenject;

namespace LlapiExample
{
    [RequireComponent(typeof(Rigidbody))]
    public class Bullet : NetworkEntity
    {
        [Inject]
        private OutgoingCommandsQueue outgoings;

        [Inject(Id = 0)]
        private CommanderStatus commander;

        private Rigidbody rigidbodyCached;
        protected Rigidbody Rigidbody
        {
            get
            {
                if (!rigidbodyCached) rigidbodyCached = GetComponent<Rigidbody>();
                return rigidbodyCached;
            }
        }

        public AudioSource ImpactPrefab;
        public float Speed;
        public float Damage;

        private void Start()
        {
            Rigidbody.AddForce(transform.forward * Speed, ForceMode.VelocityChange);// * Time.fixedDeltaTime);
        }

        private void OnDestroy()
        {
            var audio = Instantiate(ImpactPrefab.gameObject, transform.position, Quaternion.identity);
            var sound = audio.GetComponent<AudioSource>();
            Destroy(audio, sound.clip.length);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (IsMine)
            {
                var entity = collision.gameObject.GetComponent<VitalEntity>();
                if (entity)
                {
                    entity.DamageReceive(Damage, collision.contacts[0].point, Rigidbody.velocity);
                    var damage = new EntityDamage();
                    damage.amount = Damage;
                    damage.id = entity.Id;
                    outgoings.Enqueue(damage);
                }

                var remove = new EntityRemove();
                remove.id = Id;
                outgoings.Enqueue(remove);
                commander.Destroy(this);
                //Destroy(gameObject);
            }
        }
    }
}