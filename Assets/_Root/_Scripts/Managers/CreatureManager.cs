using System.Collections.Generic;
using System.Linq;
using Ecosystem.Creatures;
using UnityEngine;

namespace Ecosystem.Managers
{
    public class CreatureManager : MonoBehaviour
    {
        [SerializeField] private Transform m_CreatureParent;
        [SerializeField] private Spawner m_FoodSpawner;
        [Range(0f, 1f)] [SerializeField] private float m_HungerThreshold = 0.5f;

        public static List<CreatureBehaviour> m_Creatures { get; private set; }

        private void Start()
        {
            m_Creatures = new List<CreatureBehaviour>();

            // Find all objects with the CreatureAgent component and add them to the m_Creatures list.
            var creatureBehaviours = FindObjectsOfType<CreatureBehaviour>();
            foreach (var creatureBehaviour in creatureBehaviours)
            {
                creatureBehaviour.transform.SetParent(m_CreatureParent);
                m_Creatures.Add(creatureBehaviour);
            }
        }

        private void Update()
        {
            foreach (var creature in m_Creatures)
                FindClosestFood(creature);
        }

        private void FindClosestFood(CreatureBehaviour creature)
        {
            // Only start finding food if the creatures doesn't have a target and their hunger is at or below their threshold.
            if (creature.m_Target != null ||
                !(creature.m_NeedsAndTraits.m_CurrentCreatureNeeds.m_Hunger
                  <= creature.m_NeedsAndTraits.m_BaseNeeds.m_Hunger * m_HungerThreshold)) return;
            
            Transform closestFood = null;
            var closestDistance = Mathf.Infinity;

            // Foreach food object found, check if they both active and the closest to the creature.
            foreach (var food in m_FoodSpawner.m_SpawnedObjectPool.Where(food => food.gameObject.activeSelf)
                         .Where(food =>
                             creature.transform.position.IsTargetInRange(food.position, closestDistance)))
            {
                closestFood = food;
                // Keep the value updated with distance to the closest "Food" object.
                closestDistance = Vector3.Distance(creature.transform.position, food.position);
            }

            // Assign the closest food as the target.
            creature.m_Target = closestFood;

            // Logic for finding the closest target near the creature.
        }
    }
}