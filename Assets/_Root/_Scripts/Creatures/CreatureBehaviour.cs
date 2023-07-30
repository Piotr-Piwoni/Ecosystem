using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Ecosystem.Creatures
{
    [RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody))]
    public abstract class CreatureBehaviour : MonoBehaviour
    {
        [Range(0f, 30f)] [SerializeField] protected float m_FOVDistance = 14f;
        [Range(0f, 30f)] [SerializeField] protected float m_WanderingDistance = 17f;
        [Range(0f, 30f)] [SerializeField] protected float m_FoodTrackingDistance = 30f;
        [SerializeField] protected float m_ArrivalThreshold = 0.1f;
        [SerializeField] protected bool m_PersistentGizmos;

        protected NavMeshAgent m_Agent;
        protected CreatureStates m_States;
        protected Vector3 m_NewRandomPos;
        protected bool m_HasGeneratedRandomPos;

        public CreatureNeedsAndTraits m_NeedsAndTraits { get; private set; }
        public Transform m_Target;

        protected virtual void Awake()
        {
            m_Agent = GetComponent<NavMeshAgent>();
            m_NeedsAndTraits = GetComponent<CreatureNeedsAndTraits>();
            m_Target = null;
        }

        protected virtual void Start() => m_States = CreatureStates.Wandering;

        protected virtual void Update()
        {
            switch (m_States)
            {
                case CreatureStates.Wandering:
                    HandleWandering();
                    break;
                case CreatureStates.Following:
                    HandleFollowing();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (m_Target != null && transform.position.IsTargetOutOfRange(m_Target.position, m_FoodTrackingDistance))
                m_Target = null;

            //Debug.Log($"Current state is {m_States}");
        }

        protected abstract void HandleWandering();

        protected abstract void HandleFollowing();

        // Generates a random position within a radius of the creature.
        protected Vector3 GenerateRandomPosition()
        {
            var position = transform.position;
            var randomDirection = Random.insideUnitSphere;

            Vector3 randomPos = position + randomDirection * m_WanderingDistance;

            if (!NavMesh.SamplePosition(randomPos, out var hit, m_WanderingDistance, NavMesh.AllAreas))
                return transform.position; // Default to current position if a valid random position couldn't be found.

            randomPos = hit.position;
            randomPos.y = position.y;

            return randomPos;
        }

        #region Gizmos

        private void GizmosToDraw()
        {
            var position = transform.position;

            // Gizmos for the agent's FOV.
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(position, m_FOVDistance);

            // Gizmos for where the agent's new random positions can be generated.
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(position, m_WanderingDistance);

            // Gizmos for the agent's random position indicator.
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(m_NewRandomPos, .5f);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(position, m_FoodTrackingDistance);
        }

        private void OnDrawGizmos()
        {
            if (m_PersistentGizmos)
                GizmosToDraw();
        }

        private void OnDrawGizmosSelected()
        {
            if (!m_PersistentGizmos)
                GizmosToDraw();
        }

        #endregion

        protected enum CreatureStates
        {
            Wandering = 1,
            Following = 2
        }
    }
}