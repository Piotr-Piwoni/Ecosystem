using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Ecosystem
{
    public class CreatureAgent : MonoBehaviour
    {
        [SerializeField] private Transform m_Target;
        [Range(0f, 30f)] [SerializeField] private float m_FOVDistance = 10f;
        [Range(0f, 30f)] [SerializeField] private float m_TrackingDistance = 20f;
        [SerializeField] private bool m_PersistentGizmos;

        private NavMeshAgent m_Agent;
        private CreatureStates m_States;
        private Vector3 m_NewRandomPos;
        private bool m_HasGeneratedRandomPos;

        private void Start()
        {
            m_Agent = GetComponent<NavMeshAgent>();
            m_States = CreatureStates.Following;
        }

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
            
            //Debug.Log($"Current state is {m_States}");
        }
        
        private void HandleWandering()
        {
            // If the target active and is within range, stop wandering and follow the target.
            if (m_Target != null && IsTargetInRange(m_Target.position, m_FOVDistance))
            {
                m_Agent.ResetPath(); // Stop the agent from moving.
                m_States = CreatureStates.Following;
                return;
            }

            // If this is the first time the method is called or the random position is farther
            // away then the 1.5x tracking distance, generate a new random position.
            if (!m_HasGeneratedRandomPos || IsTargetOutOfRange(m_NewRandomPos, m_TrackingDistance * 1.5f))
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
            if (IsTargetInRange(m_Target.position, m_FOVDistance))
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

        private bool IsTargetInRange(Vector3 targetPos, float range)
        {
            var distanceToTarget = Vector3.Distance(transform.position, targetPos);

            return distanceToTarget <= range;
        }
        
        private bool IsTargetOutOfRange(Vector3 targetPos, float range)
        {
            var distanceToTarget = Vector3.Distance(transform.position, targetPos);

            return distanceToTarget > range;
        }

        private Vector3 GenerateRandomPosition()
        {
            var position = transform.position;
            var randomDirection = Random.insideUnitSphere;

            Vector3 randomPos = position + randomDirection * m_TrackingDistance;
            randomPos.y = position.y;

            return randomPos;
        }

        private void OnTriggerEnter(Collider other)
        {
            // Check if the agent interacted with a "Food" object.
            if (!other.CompareTag($"Food")) return;
            // If so, deactivate it and remove the object.
            Debug.Log("Eat: Hunger +1");
            other.gameObject.SetActive(false);
            Destroy(other.gameObject, 2f);
            
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
            Gizmos.DrawWireSphere(position, m_TrackingDistance);

            // Gizmos for the agent's random position indicator.
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(m_NewRandomPos, .5f);
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