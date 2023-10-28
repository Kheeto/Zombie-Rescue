using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyState currentState;

    [Header("Stats")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private float lookSpeed = 3f;
    [SerializeField] private float movementSpeed = 3f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackSpeed = 2f;

    [Space(10)]
    [SerializeField] private float spotRange = 15f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float patrolRange = 8f;
    [SerializeField] private float patrolDelay = 1f;
    [Range(0, 1)]
    [SerializeField] private float flankChance = 0.3f;
    [SerializeField] private float flankRange = 5f;
    [Range(0, 1)]
    [SerializeField] private float followPlayerChance = 0.3f;
    [SerializeField] private LayerMask visionMask;

    [Header("References")]
    [SerializeField] private Transform player;

    private NavMeshAgent agent;
    private bool canAttack;
    private bool canPatrol;

    public enum EnemyState
    {
        Idle,
        FollowingPlayer,
        AttackingPlayer,
        FlankingPlayer
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = movementSpeed;
        currentHealth = maxHealth;
        ResetAttack();
        ResetPatrol();
    }

    private void Update()
    {
        LookAtPlayer();
        StateMachine();
    }

    /// <summary>
    /// By default, the NavMesh won't rotate towards the player if he is within the stopping distance.
    /// </summary>
    private void LookAtPlayer()
    {
        if (Vector3.Distance(transform.position, player.position) <= agent.stoppingDistance)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, lookSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Perform different actions based on the current state.
    /// </summary>
    private void StateMachine()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.FollowingPlayer:
                FollowPlayer();
                break;
            case EnemyState.AttackingPlayer:
                AttackPlayer();
                break;
            case EnemyState.FlankingPlayer:
                FlankPlayer();
                break;
        }
    }
    
    /// <summary>
    /// Follow or flank the player if he is visible. Otherwise move randomly around the map.
    /// </summary>
    private void Idle()
    {
        if (IsPlayerVisible())
        {
            agent.SetDestination(player.position);

            // Choose whether to flank or follow directly
            if (Random.Range(0f, 1f) < flankChance)
            {
                MoveRandomly(flankRange);
                currentState = EnemyState.FlankingPlayer;
            }
            else {
                currentState = EnemyState.FollowingPlayer;
            }
            return;
        }

        // Enemies have a chance of following the player even if he is not visible.
        if (Random.Range(0f, 1f) < followPlayerChance)
        {
            agent.SetDestination(player.position);
            return;
        }

        float distance = Vector3.Distance(transform.position, agent.destination);
        // If the enemy has no path to follow, choose a random position
        if ((!agent.hasPath || distance <= agent.stoppingDistance + 0.2f) && canPatrol)
        {
            MoveRandomly(patrolRange);
            Invoke(nameof(ResetPatrol), patrolDelay);
        }
    }

    /// <summary>
    /// Follow the player if he is visible, otherwise idle. Also start attacking if he is close enough.
    /// </summary>
    private void FollowPlayer()
    {
        if (IsPlayerVisible())
        {
            agent.SetDestination(player.position);
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance < attackRange) currentState = EnemyState.AttackingPlayer;
        }
        else
            currentState = EnemyState.Idle;
    }

    /// <summary>
    /// Attack the player if he is close enough, otherwise, follow him if he is visible
    /// or idle if he is not visible.
    /// </summary>
    private void AttackPlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > attackRange)
        {
            if (IsPlayerVisible()) currentState = EnemyState.FollowingPlayer;
            else currentState = EnemyState.Idle;
            return;
        }

        if (canAttack)
        {
            canAttack = false;
            Invoke(nameof(ResetAttack), attackSpeed);

            player.GetComponent<PlayerCombat>()?.Damage(damage);
        }
    }

    /// <summary>
    /// Move towards a random position and then follow the player or flank again.
    /// </summary>
    private void FlankPlayer()
    {
        // Check if the enemy has reached his flanking position
        float distance = Vector3.Distance(transform.position, agent.destination);
        if (distance <= agent.stoppingDistance + 0.2f)
        {
            if (IsPlayerVisible())
                currentState = EnemyState.FollowingPlayer;
            else {
                if (Random.Range(0f, 1f) < flankChance)
                    currentState = EnemyState.Idle;
                else {
                    agent.SetDestination(player.position);
                    currentState = EnemyState.FollowingPlayer;
                }
            }
        }
    }

    /// <summary>
    /// Choose a random point inside of the specified range and set the destination to it
    /// </summary>
    private void MoveRandomly(float range)
    {
        Vector2 randomPos = Random.insideUnitCircle;
        Vector3 finalPos = new Vector3(randomPos.x, 0f, randomPos.y);
        agent.SetDestination(transform.position + finalPos * range);
    }

    private void ResetAttack() { canAttack = true; }

    private void ResetPatrol() { canPatrol = true; }

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
