using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class MutantBehaviour : MonoBehaviour
{
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float patrolRadius = 5f;

    private NavMeshAgent agent;
    private Vector3 target;
    private State state = State.Patrol;

    private enum State
    {
        Idle,
        Patrol,
        Chase,
        Attack
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SetRandomDestination();
    }

    private void Update()
    {
        switch (state)
        {
            case State.Idle:
                break;
            case State.Patrol:
                Patrol();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Attack:
                Attack();
                break;
        }
    }

    private void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            SetRandomDestination();
        }
    }

    private void Chase()
    {
        if (target != null)
        {
            agent.SetDestination(target);
            agent.speed = chaseSpeed;
            if (Vector3.Distance(transform.position, target) < 0.4f)
            {
                state = State.Attack;
            }
        }
    }

    private void Attack()
    {
        if (Vector3.Distance(transform.position, target) > 1f)
        {
            state = State.Chase;
        }
    }

    public void DetectPlayer(Transform Player)
    {
        target = Player.position;
        state = State.Chase;
    }

    private void SetRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas);

        if (Vector3.Distance(transform.position, hit.position) < 8f)
        {
            SetRandomDestination();
        }
        else
        {
            target = hit.position;
            agent.SetDestination(target);
            agent.speed = patrolSpeed;
        }
    }
}