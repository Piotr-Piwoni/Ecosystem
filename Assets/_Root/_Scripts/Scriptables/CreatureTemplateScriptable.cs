using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ecosystem.Scriptables
{
    [CreateAssetMenu(fileName = "Creature", menuName = "Ecosystem/Creature Template", order = 0)]
    public class CreatureTemplateScriptable : ScriptableObject
    {
        [SerializeField] private Needs m_Needs = new() { m_Hunger = 40f, m_Energy = 40f };
        public Needs m_BaseNeeds => m_Needs;

        [SerializeField] private List<Traits> m_Traits = new() { Traits.Nothing };
        public List<Traits> m_BaseTraits => m_Traits;
    }

    [Serializable]
    public struct Needs
    {
        public float m_Hunger;
        public float m_Energy;
    }

    [Serializable]
    public enum Traits
    {
        Nothing = 0,
        Foody = 1
    }
}