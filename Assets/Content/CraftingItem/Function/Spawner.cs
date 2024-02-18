using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private int numberOfObjectsToSpawn = 10;
    [SerializeField] private float spawnRange = 10f;
    [SerializeField] private GameObject[] objectPrefabs;
    [SerializeField] private float CooldownResp;

    [SerializeField] private List<Vector3> SpawnObjectPosition = new List<Vector3>();
    private List<GameObject> spawnObjects = new List<GameObject>();
    [SerializeField] private bool CanBeRespown;


    private void Start()
    {
        CanBeRespown = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && CanBeRespown)
        {
            spawnObjects.Clear();
            SpawnRandomObjects();
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            foreach (GameObject obj in spawnObjects)
            {
                obj.SetActive(false);
            }
            CanBeRespown = false;
            StartCoroutine(RespawnCooldown());
        }
    }

    private IEnumerator RespawnCooldown()
    {
        yield return new WaitForSeconds(CooldownResp);
        CanBeRespown = true;
    }


    private void SpawnRandomObjects()
    {
        if (objectPrefabs.Length == 0)
        {
            Debug.LogWarning("ObjectPrefabs array or ObjectPooling reference is missing.");
            return;
        }

        if (SpawnObjectPosition.Count != 0)
        {
            for (int i = 0; i < numberOfObjectsToSpawn; i++)
            {
                GameObject randomPrefab = objectPrefabs[Random.Range(0, objectPrefabs.Length)];
                spawnObjects.Add(ObjectPooling.Instance.GetPooledObject(randomPrefab.name, SpawnObjectPosition[i], Quaternion.identity));
            }
        }
        else
        {
            for (int i = 0; i < numberOfObjectsToSpawn; i++)
            {
                GameObject randomPrefab = objectPrefabs[Random.Range(0, objectPrefabs.Length)];
                Vector3 randomPosition = GetRandomPosition(transform.position, spawnRange);
                SpawnObjectPosition.Add(randomPosition);
                spawnObjects.Add(ObjectPooling.Instance.GetPooledObject(randomPrefab.name, randomPosition, Quaternion.identity)); 
            }
        }
    }

    private Vector3 GetRandomPosition(Vector3 center, float range)
    {
        Vector2 randomCircle = Random.insideUnitCircle * range;
        Vector3 randomPosition = center + new Vector3(randomCircle.x, 0f, randomCircle.y);
        return randomPosition;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRange);
    }
}