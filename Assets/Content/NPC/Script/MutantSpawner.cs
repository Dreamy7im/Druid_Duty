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
        // Sprawdzamy czy mamy jakieœ mutanty w liœcie
        if (MutantList.Length == 0)
        {
            Debug.LogError("MutantList is empty! Add mutants to spawn.");
            return;
        }

        // Losujemy pozycjê spawnu na NavMeshu
        Vector3 randomPosition = RandomNavmeshLocation(transform.position, SpawnRange);

        // Losujemy index mutantu z listy
        int randomMutantIndex = Random.Range(0, MutantList.Length);

        // Tworzymy now¹ instancjê losowego mutanta na wylosowanej pozycji
        GameObject mutant = Instantiate(MutantList[randomMutantIndex], randomPosition, Quaternion.identity);
        mutant.GetComponent<MutantBehaviour>().spawner = transform;
        mutant.GetComponent<MutantBehaviour>().PatrolRange = SpawnRange;
    }

    private Vector3 RandomNavmeshLocation(Vector3 origin, float distance)
    {
        // Losujemy punkt na powierzchni NavMesh w okreœlonym promieniu od podanego punktu
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        randomDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, distance, NavMesh.AllAreas);
        return navHit.position;
    }

    void OnDrawGizmosSelected()
    {
        // Display the explosion radius when selected
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, SpawnRange);
    }
}
