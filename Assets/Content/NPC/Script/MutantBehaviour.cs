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
        state = State.Patrol; // Ustawiamy stan poczπtkowy na Patrol
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

        // Sprawdü czy obiekt jest wystarczajπco blisko celu, jeúli tak - znajdü nowy cel
        if (Vector3.Distance(transform.position, target) < minDistanceToTarget)
        {
            StartCoroutine(WaitForNewTarget());
        }
    }

    private void FindNewTarget()
    {
        // Losuj nowπ pozycjÍ w obrÍbie PatrolRange od spawnera
        Vector3 randomDirection = Random.insideUnitSphere * PatrolRange;
        randomDirection += spawner.position;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, PatrolRange, NavMesh.AllAreas);

        // Sprawdzamy odleg≥oúÊ miÍdzy nowym celem a aktualnπ pozycjπ
        float distanceToNewTarget = Vector3.Distance(navHit.position, transform.position);

        // Jeúli nowy cel jest zbyt blisko, wylosuj nowy cel
        if (distanceToNewTarget < 5f)
        {
            FindNewTarget();
            return; // Wyjdü z funkcji, aby uniknπÊ ustawiania celu, gdy jest zbyt blisko
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