using UnityEngine;
using UnityEngine.AI;
using System.Collections;

namespace LlapiExample
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    public class FirererController : MonoBehaviour
    {
        #region Dependencies
        private NavMeshAgent cachedAgent;
        protected NavMeshAgent Agent
        {
            get
            {
                if (!cachedAgent)
                    cachedAgent = GetComponent<NavMeshAgent>();
                return cachedAgent;
            }
        }
        private Animator cachedAnimator;
        protected Animator Animator
        {
            get
            {
                if (!cachedAnimator)
                    cachedAnimator = GetComponent<Animator>();
                return cachedAnimator;
            }
        }
        #endregion

        public Vector3 TargetPosition;
        public float MaxSpeed;

        private void Awake()
        {
            TargetPosition = transform.position;
            Agent.speed = MaxSpeed;
        }

        private void Update()
        {
            Animator.SetFloat("Speed", Agent.velocity.magnitude / MaxSpeed);
        }

        public void Move(Vector3 position)
        {
            Agent.destination = position;
        }

        public void Translate(Vector3 position)
        {
            transform.position = position;
        }

        private void OnDrawGizmos()
        {
            var tmpColor = Gizmos.color;
            Gizmos.color = new Color(0f, 1f, 0f, 0.55f);
            Gizmos.DrawSphere(TargetPosition, 0.2f);

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