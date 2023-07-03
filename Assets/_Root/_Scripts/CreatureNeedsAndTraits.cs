using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Ecosystem.Managers;
using Ecosystem.Scriptables;
using UnityEngine;

namespace Ecosystem
{
    public class CreatureNeedsAndTraits : MonoBehaviour
    {
        [SerializeField] private CreatureTemplateScriptable m_CreatureInformation;
        [SerializeField] private float m_TickDuration = 2f;

        public Needs m_CurrentCreatureNeeds;
        public List<Traits> m_CurrentCreatureTraits;
        public Needs m_BaseNeeds { get; private set; }

        private void Awake()
        {
            m_CurrentCreatureNeeds = m_CreatureInformation.m_BaseNeeds;
            m_BaseNeeds = m_CreatureInformation.m_BaseNeeds;
            m_CurrentCreatureTraits = m_CreatureInformation.m_BaseTraits;

            StartCoroutine(NeedsDecreasing_CO(NeedsType.Hunger));
        }

        private IEnumerator NeedsDecreasing_CO(NeedsType needType, float decreaseMultiplier = 1f)
        {
            // Define functions to get and set the appropriate need value.
            Func<float> getNeedValue;
            Action<float> setNeedValue;

            switch (needType)
            {
                case NeedsType.Hunger:
                    getNeedValue = () => m_CurrentCreatureNeeds.m_Hunger;
                    setNeedValue = value => m_CurrentCreatureNeeds.m_Hunger = value;
                    break;
                case NeedsType.Energy:
                    getNeedValue = () => m_CurrentCreatureNeeds.m_Energy;
                    setNeedValue = value => m_CurrentCreatureNeeds.m_Energy = value;
                    break;
                default:
                    Debug.LogWarning($"The NeedsType<{needType}> specified does not exist!");
                    yield break;
            }
            
            // The actual start of the coroutine function.
            while (true)
            {
                yield return new WaitForSeconds(m_TickDuration);

                const float baseDecrease = 1f;
                var currentValue = getNeedValue();
                setNeedValue(currentValue - baseDecrease * decreaseMultiplier);
            }
        }
        
        // Enum to represent the different types of needs. 
        private enum NeedsType
        {
            Hunger,
            Energy
        }
    }
}