using UnityEngine;

namespace Ecosystem.Creatures
{
    public class BunnyBehaviour : CreatureBehaviour
    {
        protected override void HandleWandering()
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
            else if (Vector3.Distance(transform.position, m_NewRandomPos) < m_ArrivalThreshold)
            {
                m_NewRandomPos = GenerateRandomPosition();
                Debug.Log($"Random Position: ({m_NewRandomPos.x}.x,{m_NewRandomPos.z}.z)");
            }

            // Make the agent move towards the new position.
            m_Agent.SetDestination(m_NewRandomPos);
        }

        protected override void HandleFollowing()
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
        
        protected void OnTriggerEnter(Collider other)
        {
            // Check if the agent interacted with a "Food" object and that they are hungry.
            if (!other.CompareTag($"Food") || !m_IsHungry) return;
            
            // If so, deactivate it and fill up the creature's hunger need.
            m_NeedsAndTraits.m_CurrentCreatureNeeds.m_Hunger += 15f;
            other.gameObject.SetActive(false);

            m_Target = null;
            m_States = CreatureStates.Wandering; // Set the agent back to wandering.

        }
    }
}