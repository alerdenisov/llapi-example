using UnityEngine;
using System.Collections;

namespace LlapiExample
{
    [RequireComponent(typeof(Rigidbody))]
    public class Bullet : NetworkEntity
    {
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

        private void OnCollisionEnter(Collision collision)
        {
            if (IsMine)
            {
                var entity = collision.gameObject.GetComponent<VitalEntity>();
                if (entity)
                {
                    entity.DamageReceive(Damage, collision.contacts[0].point, Rigidbody.velocity);
                }
            }

            var audio = Instantiate(ImpactPrefab.gameObject, transform.position, Quaternion.identity);
            var sound = audio.GetComponent<AudioSource>();
            Destroy(audio, sound.clip.length);
            Destroy(gameObject);
        }
    }
}