using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignForEnemy : MonoBehaviour
{
    [SerializeField] private float Range;
    [SerializeField] private float FieldOfViewAngle;
    [SerializeField] private Transform Player;
    private MutantBehaviour mutantBehaviour;
    public bool FindPlayerObject { get; private set; }


    private void Start()
    {
        mutantBehaviour = GetComponent<MutantBehaviour>();
    }

    private void OnDrawGizmos()
    {
        Vector3 direction = Quaternion.Euler(0, FieldOfViewAngle * 0.5f, 0) * transform.forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, direction.normalized * Range);
        Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0, -FieldOfViewAngle * 0.5f, 0) * transform.forward * Range);
        Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0, FieldOfViewAngle * 0.5f, 0) * transform.forward * Range);

        Collider[] colliders = Physics.OverlapSphere(transform.position, Range);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Vector3 directionToTarget = (collider.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(directionToTarget, transform.forward);
                if (angle < FieldOfViewAngle * 0.5f)
                {
                    FindPlayerObject = true;
                    Player = collider.gameObject.transform;

                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(transform.position, collider.transform.position);
                }
                else
                {
                    FindPlayerObject = true;
                }
            }
        }
    }
}