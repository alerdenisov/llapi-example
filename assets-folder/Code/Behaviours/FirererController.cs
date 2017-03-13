using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System;

namespace LlapiExample
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    public class FirererController : MonoBehaviour
    {
        #region Dependencies
        private NavMeshAgent cachedAgent;
        public NavMeshAgent Agent
        {
            get
            {
                if (!cachedAgent)
                    cachedAgent = GetComponent<NavMeshAgent>();
                return cachedAgent;
            }
        }
        private Animator cachedAnimator;
        public Animator Animator
        {
            get
            {
                if (!cachedAnimator)
                    cachedAnimator = GetComponent<Animator>();
                return cachedAnimator;
            }
        }

        public BulletController Bullet;
        public Transform SpineBone;
        public Transform RootBone;
        public Transform WeaponBone;
        #endregion

        public enum StateFlags : byte
        {
            Dead        = 0,
            Alive       = 1 << 0,

            Run         = 1 << 1,
            Attack      = 1 << 2,
            Damage      = 1 << 3
        }


        public bool Dead
        {
            get
            {
                return State == StateFlags.Dead;
            }
            private set
            {
                State = value ? StateFlags.Dead : State;
            }
        }

        public bool IsDamage   { get { return (State & StateFlags.Damage) == StateFlags.Damage; } }
        public bool IsRunning  { get { return (State & StateFlags.Run) == StateFlags.Run; } }
        public bool IsShooting { get { return (State & StateFlags.Attack) == StateFlags.Attack; } } 

        public StateFlags State;

        public Vector3 TargetPosition;
        public float MaxSpeed;


        public Vector3 shootDirection;

        private Quaternion spineOrientation;

        private void Awake()
        {
            TargetPosition = transform.position;
            Agent.speed = MaxSpeed;
            spineOrientation = SpineBone.transform.localRotation;
            shootDirection = Vector3.forward;
            State = StateFlags.Alive;
        }

        private void Update()
        {
            if (Dead)
            {
                // Dead state
                Agent.Stop();
                Animator.SetFloat("Speed", 0f);
                Animator.SetBool("Fire", false);
                Animator.SetBool("Damage", false);
                RootBone.localRotation = Quaternion.identity;
                SpineBone.localRotation = spineOrientation;
            }
            else if (IsDamage)
            {
                // Taken damage state
                // Prevent running when taken damage
                Agent.speed = 0f;
                Animator.SetFloat("Speed", 0f);
            }
            else
            {
                // allow agent to move with max speed
                Agent.speed = MaxSpeed;

                // calculate angle between forward direction and looking direction
                var angle = Vector3.Dot(transform.forward, shootDirection);
                var side = angle < 0f ? -1 : 1;

                // set correct animation speed
                Animator.SetFloat("Speed", (Agent.velocity.magnitude / MaxSpeed) * side);

                if (angle < 0)
                {
                    SpineBone.localRotation = Quaternion.LookRotation(transform.InverseTransformDirection(Quaternion.Euler(0, 180f, 0) * shootDirection), Vector3.up) * spineOrientation;
                    RootBone.localRotation = Quaternion.Euler(0, 180f, 0);
                } else
                {
                    RootBone.localRotation = Quaternion.Euler(0, 0, 0);
                    SpineBone.localRotation = Quaternion.LookRotation(transform.InverseTransformDirection(shootDirection), Vector3.up) * spineOrientation;
                }

                if (IsShooting)
                {
                    // Shooting state
                }
                else if (IsRunning)
                {
                    // Running state
                    var distanceSqr = (Agent.destination - transform.position).sqrMagnitude;
                    if (distanceSqr < 0.1f)
                        State &= ~StateFlags.Run;
                }

            }
        }

        private void FireEvent()
        {
            var angle = Vector3.Angle(transform.forward, shootDirection);
            Instantiate(Bullet.gameObject, WeaponBone.position + shootDirection * 0.65f, Quaternion.LookRotation(shootDirection, Vector3.up));
            Animator.SetBool("Fire", false);
            State &= ~StateFlags.Attack;
        }

        private void DamageEvent()
        {
            Animator.SetBool("Damage", false);
            State &= ~StateFlags.Damage;
        }
        
        public void Move(Vector3 position)
        {
            if (Dead) return;
            State |= StateFlags.Run;
            Agent.destination = position;
        }

        public void Translate(Vector3 position)
        {
            if (Dead) return;
            transform.position = position;
        }

        public void Shoot()
        {
            if (Dead || IsDamage) return;
            State |= StateFlags.Attack;
            Animator.SetBool("Fire", true);
        }

        public void ShootDirection(Vector3 direction)
        {
            if (Dead) return;
            shootDirection = direction;
        }

        public void TakenDamage()
        {
            if (Dead) return;
            State |= StateFlags.Damage;
            Animator.SetBool("Damage", true);
        }

        public void Die()
        {
            if (Dead) return;
            State = StateFlags.Dead;
            Animator.SetBool("Die", true);
        }
        
        private void OnDrawGizmos()
        {
            var tmpColor = Gizmos.color;
            Gizmos.color = new Color(0f, 1f, 0f, 0.55f);
            Gizmos.DrawSphere(Agent.destination, 0.2f);

            Gizmos.color = new Color(0f, 1f, 0f, 1f);
            var start = transform.position;
            foreach(var end in Agent.path.corners)
            {
                Gizmos.DrawLine(start, end);
                start = end;
            }
            Gizmos.DrawLine(start, Agent.destination);

            Gizmos.color = tmpColor;
        }
    }
}