using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Ecosystem
{
    public class CreatureSpawningUILogic : MonoBehaviour
    {
        [SerializeField] private Slider m_SpawnAmountSlider;
        [SerializeField] private TMPro.TextMeshProUGUI m_SpawnAmountText;
        [SerializeField] private Button m_SpawnButton;
        [SerializeField] private Spawner m_CreatureSpawner;
        [SerializeField] private GameObject m_CreatureToSpawn;
        

        private void Update() => m_SpawnAmountText.text = m_SpawnAmountSlider.value.ToString(CultureInfo.CurrentCulture);

        public void SpawnCreatures()
        {
            m_CreatureSpawner.m_Spawnable = m_CreatureToSpawn;
            m_CreatureSpawner.m_NumberToSpawn = (int)m_SpawnAmountSlider.value;
            
            m_CreatureSpawner.SpawnObject();

            GameManager.Instance.ChangeState(GameState.Simulation);
            
            m_SpawnButton.interactable = false;
        }
    }
}
