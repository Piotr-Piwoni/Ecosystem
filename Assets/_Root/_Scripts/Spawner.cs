using UnityEngine;
using Random = UnityEngine.Random;

namespace Ecosystem
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private Vector3 m_BoxAreaSize;
        [SerializeField] private Transform m_GroundHeightReference;
        [SerializeField] private GameObject m_Spawnable;
        [SerializeField] private int m_NumberToSpawn = 10;

        private void Start()
        {
            for (int i = 0; i < m_NumberToSpawn; i++)
                SpawnObject();
        }

        private void SpawnObject()
        {
            var spawnPosition = GenerateRandomPosition();
            Instantiate(m_Spawnable, spawnPosition, Quaternion.identity).transform.SetParent(transform);
        }

        private Vector3 GenerateRandomPosition()
        {
            var spawnAreaCenter = transform.position;
            var randomOffset = new Vector3(
                Random.Range(-m_BoxAreaSize.x / 2f, m_BoxAreaSize.x / 2f),
                Random.Range(-m_BoxAreaSize.y / 2f, m_BoxAreaSize.y / 2f),
                Random.Range(-m_BoxAreaSize.z / 2f, m_BoxAreaSize.z / 2f)
            );

            var newPosition = spawnAreaCenter + randomOffset;
            
            if (m_GroundHeightReference != null)
                newPosition.y = m_GroundHeightReference.position.y;
            
            return newPosition;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0f, 0.7f, 0.7f, 0.45f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(Vector3.zero, m_BoxAreaSize);
        }
    }
}