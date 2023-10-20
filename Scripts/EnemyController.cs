using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private float movementSpeed = 3f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackSpeed = 2f;

    [Space(10)]
    [SerializeField] private float spotRange = 15f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private LayerMask visionMask;

    [Header("References")]
    [SerializeField] private Transform player;

    private NavMeshAgent agent;
    private bool canAttack;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = movementSpeed;
        ResetAttack();
        currentHealth = maxHealth;
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
        if (canAttack)
        {
            canAttack = false;
            Invoke(nameof(ResetAttack), attackSpeed);

            player.GetComponent<PlayerCombat>()?.Damage(damage);
        }
    }

    private void ResetAttack()
    {
        canAttack = true;
    }

    public void Damage(int damage)
    {
        Debug.Log("Enemy \"" + transform.name + "\" damaged by " + damage);
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (currentHealth == 0) Die();
    }

    private void Die()
    {
        Debug.Log("Enemy died");
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

        if (Application.isPlaying)
            Gizmos.DrawLine(transform.position, agent.destination);
    }
}
