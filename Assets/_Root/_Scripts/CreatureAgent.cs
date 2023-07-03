using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Ecosystem
{
    [RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody))]
    public class CreatureAgent : MonoBehaviour
    {
        [Range(0f, 30f)] [SerializeField] private float m_FOVDistance = 14f;
        [Range(0f, 30f)] [SerializeField] private float m_WanderingDistance = 17f;
        [Range(0f, 30f)] [SerializeField] private float m_FoodTrackingDistance = 30f;
        [SerializeField] private bool m_PersistentGizmos;

        private NavMeshAgent m_Agent;
        private CreatureStates m_States;
        private Vector3 m_NewRandomPos;
        private bool m_HasGeneratedRandomPos;

        public CreatureNeedsAndTraits m_NeedsAndTraits { get; private set; }
        public Transform m_Target;

        private void Awake()
        {
            m_Agent = GetComponent<NavMeshAgent>();
            m_NeedsAndTraits = GetComponent<CreatureNeedsAndTraits>();
            m_Target = null;
        }

        private void Start() => m_States = CreatureStates.Wandering;

        private void Update()
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

        private void HandleWandering()
        {
            // If the target active and is within range, stop wandering and follow the target.
            if (m_Target != null && transform.position.IsTargetInRange(m_Target.position, m_FOVDistance))
            {
                m_Agent.ResetPath(); // Stop the agent from moving.
                m_HasGeneratedRandomPos = false;
                m_States = CreatureStates.Following;
                return;
            }

            // If this is the first time the method is called or the random position is farther
            // away then the 1.5x tracking distance, generate a new random position.
            if (!m_HasGeneratedRandomPos ||
                transform.position.IsTargetOutOfRange(m_NewRandomPos, m_WanderingDistance * 1.5f))
            {
                m_NewRandomPos = GenerateRandomPosition();
                m_HasGeneratedRandomPos = true;
                Debug.Log($"Random Position: ({m_NewRandomPos.x}.x,{m_NewRandomPos.z}.z)");
            }
            // If the agent reached the random position, generate a new one.
            else if (transform.position == m_NewRandomPos)
            {
                m_NewRandomPos = GenerateRandomPosition();
                Debug.Log($"Random Position: ({m_NewRandomPos.x}.x,{m_NewRandomPos.z}.z)");
            }

            // Make the agent move towards the new position.
            m_Agent.SetDestination(m_NewRandomPos);
        }

        private void HandleFollowing()
        {
            if (transform.position.IsTargetInRange(m_Target.position, m_FOVDistance))
            {
                //Debug.Log($"{m_Target.name} is in radius.");
                m_Agent.SetDestination(m_Target.position);
            }
            else
            {
                // Debug.Log($"{m_Target.name} is outside radius. Stopping pursuit.");
                m_Agent.ResetPath(); // Stop the agent from moving.

                m_States = CreatureStates.Wandering;
            }
        }

        private Vector3 GenerateRandomPosition()
        {
            var position = transform.position;
            var randomDirection = Random.insideUnitSphere;

            Vector3 randomPos = position + randomDirection * m_WanderingDistance;
            randomPos.y = position.y;

            return randomPos;
        }

        private void OnTriggerEnter(Collider other)
        {
            // Check if the agent interacted with a "Food" object.
            if (!other.CompareTag($"Food")) return;
            
            // If so, deactivate it and fill up the creature's hunger need.
            m_NeedsAndTraits.m_CurrentCreatureNeeds.m_Hunger += 15f;
            other.gameObject.SetActive(false);

            m_Target = null;
            m_States = CreatureStates.Wandering; // Set the agent back to wandering.
        }

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

        private enum CreatureStates
        {
            Wandering = 1,
            Following = 2
        }
    }
}