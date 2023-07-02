using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ecosystem.Managers
{
    public class CreatureManager : MonoBehaviour
    {
        [SerializeField] private Transform m_CreatureParent;
        [SerializeField] private Spawner m_FoodSpawner;

        //public List<CreatureAgent> m_Creatures { get; private set; }
        public List<CreatureAgent> m_Creatures;

        private void Start()
        {
            m_Creatures = new List<CreatureAgent>();

            // Find all objects with the CreatureAgent component and add them to the m_Creatures list.
            var creatureAgents = FindObjectsOfType<CreatureAgent>();
            foreach (var creatureAgent in creatureAgents)
            {
                creatureAgent.transform.SetParent(m_CreatureParent);
                m_Creatures.Add(creatureAgent);
            }
        }

        private void Update()
        {
            foreach (var creature in m_Creatures)
            {
                if (creature.m_Target != null) continue;
                // Find the closest active food object
                Transform closestFood = null;
                var closestDistance = Mathf.Infinity;

                foreach (var food in m_FoodSpawner.m_SpawnedObjectPool.Where(food => food.gameObject.activeSelf)
                             .Where(food =>
                                 creature.transform.position.IsTargetInRange(food.position, closestDistance)))
                {
                    closestFood = food;
                    closestDistance = Vector3.Distance(creature.transform.position, food.position);
                }

                // Assign the closest food as the target
                creature.m_Target = closestFood;
            }
        }
    }
}