using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System;
using Zenject;

namespace LlapiExample
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    public partial class Firerer : VitalEntity
    {
        #region Editor Fields
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
        public Transform SpineBone;
        public Transform RootBone;
        public Transform WeaponBone;
        #endregion

        #region Dependencies
        [Inject]
        private DiContainer container;
        [Inject]
        private OutgoingCommandsQueue outgoings;
        #endregion

        public Action OnShoot;
        public Action OnBuild;
        public Action<Vector3, float> OnBuilt;
        public Action<Vector3, Vector3> OnBullet;

        public enum StateFlags : byte
        {
            Dead        = 0,
            Alive       = 1 << 0,

            Run         = 1 << 1,
            Attack      = 1 << 2,
            Damage      = 1 << 3,
            Build       = 1 << 4
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
        public bool IsBuilding { get { return (State & StateFlags.Build) == StateFlags.Build; } }

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
            Animator.SetBool("Fire", false);
            if (IsShooting)
            {
                if (IsMine)
                {
                    var position = WeaponBone.position + shootDirection * 0.65f;
                    var entity = status.SpawnBullet(position, shootDirection);

                    var cmd = new EntityCreate();
                    cmd.prefab = PrefabType.Bullet;
                    cmd.id = entity.Id;
                    cmd.position = position;
                    cmd.forward = shootDirection;
                    outgoings.Enqueue(cmd);
                }

                State &= ~StateFlags.Attack;
            }
            else if(IsBuilding)
            {
                if (IsMine)
                {
                    var position = transform.position + shootDirection * 1.2f + Vector3.up * 1.5f;
                    var entity = status.SpawnCrate(position);

                    var cmd = new EntityCreate();
                    cmd.prefab = PrefabType.Crate;
                    cmd.id = entity.Id;
                    cmd.position = position;
                    outgoings.Enqueue(cmd);
                }

                State &= ~StateFlags.Build;
            }
            // TODO: Authority
        }

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
            if (health > 0)
            {
                TakenDamage();
            }
            else
            {
                Die();
            }
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
            if (!IsShooting && !IsBuilding)
            {
                State |= StateFlags.Attack;
                Animator.SetBool("Fire", true);
                if (OnShoot != null) OnShoot();
            }
        }

        public void Build()
        {
            if (Dead || IsDamage) return;
            if (!IsShooting && !IsBuilding)
            {
                State |= StateFlags.Build;
                Animator.SetBool("Fire", true);
                if (OnBuild != null) OnBuild();
            }
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