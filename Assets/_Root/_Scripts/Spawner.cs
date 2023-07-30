using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
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
        [SerializeField] private float m_ObjectRespawnSeconds = 3f;
        [SerializeField] private bool3 m_RandomRotation;

        public List<Transform> m_SpawnedObjectPool { get; private set; }

        private void Start()
        {
            SpawnObject();
            StartCoroutine(ReActivateSpawnedObject_CO());
        }

        private void SpawnObject()
        {
            m_SpawnedObjectPool = new List<Transform>();

            // For the desired number of objects to spawn, create them with a random
            // position in the box area and add them to the object pool.
            for (int i = 0; i < m_NumberToSpawn; i++)
            {
                var spawnedObject = Instantiate(m_Spawnable, GenerateRandomPosition(), GenerateRandomRotation())
                    .transform;
                spawnedObject.SetParent(transform);
                m_SpawnedObjectPool.Add(spawnedObject);
            }
        }

        private Vector3 GenerateRandomPosition()
        {
            var spawnAreaCenter = transform.position;
            // Calculate the inside area of the box from its center.
            var randomOffset = new Vector3(
                Random.Range(-m_BoxAreaSize.x / 2f, m_BoxAreaSize.x / 2f),
                Random.Range(-m_BoxAreaSize.y / 2f, m_BoxAreaSize.y / 2f),
                Random.Range(-m_BoxAreaSize.z / 2f, m_BoxAreaSize.z / 2f)
            );
            // Create the new position inside the box area.
            var newPosition = spawnAreaCenter + randomOffset;

            // If you're using a ground height reference, use that for the y axis instead.
            if (m_GroundHeightReference != null)
                newPosition.y = m_GroundHeightReference.position.y;

            return newPosition;
        }

        private Quaternion GenerateRandomRotation()
        {
            var randomRotation = Quaternion.identity;

            if (m_RandomRotation.x)
                randomRotation *= Quaternion.Euler(Random.Range(0f, 360f), 0f, 0f);

            if (m_RandomRotation.y)
                randomRotation *= Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            if (m_RandomRotation.z)
                randomRotation *= Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

            return randomRotation;
        }

        private IEnumerator ReActivateSpawnedObject_CO()
        {
            while (true)
            {
                yield return new WaitForSeconds(m_ObjectRespawnSeconds); // Wait for a designated amount of time.

                // For each spawned object check if they are de-activated.
                foreach (var spawnedObject in m_SpawnedObjectPool.Where(spawnedObject =>
                             !spawnedObject.gameObject.activeSelf))
                {
                    // Randomise their position and activate them again if so.
                    spawnedObject.position = GenerateRandomPosition();
                    spawnedObject.gameObject.SetActive(true);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Draws the box area gizmos.
            Gizmos.color = new Color(0f, 0.7f, 0.7f, 0.45f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(Vector3.zero, m_BoxAreaSize);
        }
    }
}