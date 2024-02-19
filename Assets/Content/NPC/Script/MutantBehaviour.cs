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
        state = State.Patrol; 
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
        agent.SetDestination(target);

        if (Vector3.Distance(transform.position, target) < minDistanceToTarget)
        {
            StartCoroutine(WaitForNewTarget());
        }
    }

    private void FindNewTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * PatrolRange;
        randomDirection += spawner.position;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, PatrolRange, NavMesh.AllAreas);

        float distanceToNewTarget = Vector3.Distance(navHit.position, transform.position);

        if (distanceToNewTarget < 5f)
        {
            FindNewTarget();
            return; 
        }

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
        
    }
}