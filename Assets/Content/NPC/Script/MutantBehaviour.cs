using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MutantBehaviour : MonoBehaviour
{
    private NavMeshAgent agent;
    public float PatrolRange { private get; set; }
    [SerializeField] private float minDistanceToTarget = 0.1f;

    [SerializeField] private float PlayerCheseSpeed;

    public Transform spawner { private get; set; }

    public Vector3 target { private get; set; }

    private enum State
    {
        Patrol,
        Chase,
        Attack
    }
    private State state;

    private void Start()
    {
        PatrolRange = 10f;
        agent = GetComponent<NavMeshAgent>();
        state = State.Patrol; // Ustawiamy stan pocz�tkowy na Patrol
        FindNewTarget();
    }

    private void Update()
    {
        switch (state)
        {
            case State.Patrol:
                PatrolBehaviour();
                break;
            case State.Chase:
                ChaseBehaviour();
                break;
            case State.Attack:
                AttackBehaviour();
                break;
        }
    }

    public void FindPlayer(Transform Player)
    {
        target = Player.transform.position;
        state = State.Chase;
    }

    private void PatrolBehaviour()
    {
        // Ustaw cel nawigacji na target
        agent.SetDestination(target);

        // Sprawd� czy obiekt jest wystarczaj�co blisko celu, je�li tak - znajd� nowy cel
        if (Vector3.Distance(transform.position, target) < minDistanceToTarget)
        {
            StartCoroutine(WaitForNewTarget());
        }
    }

    private void FindNewTarget()
    {
        // Losuj now� pozycj� w obr�bie PatrolRange od spawnera
        Vector3 randomDirection = Random.insideUnitSphere * PatrolRange;
        randomDirection += spawner.position;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, PatrolRange, NavMesh.AllAreas);

        // Sprawdzamy odleg�o�� mi�dzy nowym celem a aktualn� pozycj�
        float distanceToNewTarget = Vector3.Distance(navHit.position, transform.position);

        // Je�li nowy cel jest zbyt blisko, wylosuj nowy cel
        if (distanceToNewTarget < 5f)
        {
            FindNewTarget();
            return; // Wyjd� z funkcji, aby unikn�� ustawiania celu, gdy jest zbyt blisko
        }

        // Ustaw nowy cel
        target = navHit.position;
    }

    private IEnumerator WaitForNewTarget()
    {
        yield return new WaitForSeconds(2f);

        FindNewTarget();
    }

    private void ChaseBehaviour()
    {
        agent.SetDestination(target);
        agent.speed = PlayerCheseSpeed;

        if (Vector3.Distance(transform.position, target) < 1f)
        {
            agent.velocity = Vector3.zero;
        }
    }

    private void AttackBehaviour()
    {
        // Logika ataku na gracza
    }
}