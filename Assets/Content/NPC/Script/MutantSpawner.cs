using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MutantSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] MutantList;
    [SerializeField] private float SpawnRange;
    [SerializeField] private int SpawnCount;


    private void Start()
    {
        for (int i = 0; i < SpawnCount; i++)
        {
            SpawnMutant();
        }
    }

    private void SpawnMutant()
    {
        if (MutantList.Length == 0)
        {
            Debug.LogError("MutantList is empty! Add mutants to spawn.");
            return;
        }

        Vector3 randomPosition = RandomNavmeshLocation(transform.position, SpawnRange);

        int randomMutantIndex = Random.Range(0, MutantList.Length);

        GameObject mutant = Instantiate(MutantList[randomMutantIndex], randomPosition, Quaternion.identity);


    }

    private Vector3 RandomNavmeshLocation(Vector3 origin, float distance)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        randomDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, distance, NavMesh.AllAreas);
        return navHit.position;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, SpawnRange);
    }
}
