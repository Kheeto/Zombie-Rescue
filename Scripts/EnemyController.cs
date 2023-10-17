using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float speed = 3f;

    [Space(10)]
    [SerializeField] private float spotRange = 15f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private LayerMask visionMask;

    [Header("References")]
    [SerializeField] private Transform player;

    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
    }

    private void Update()
    {
        if (IsPlayerVisible())
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance < attackRange) AttackPlayer();
            FollowPlayer();
        }
    }

    private void FollowPlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        Debug.Log("Attacking player");
    }

    /// <summary>
    /// Checks if the player is directly visible by this enemy to make sure there isn't a wall in between
    /// </summary>
    private bool IsPlayerVisible()
    {
        Vector3 direction = player.position - transform.position;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, direction, out hit, spotRange, visionMask)) {
            return hit.collider.transform == player;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, spotRange);
    }
}
