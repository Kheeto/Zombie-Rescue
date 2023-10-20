using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossController : MonoBehaviour
{
    [SerializeField] private BossState currentState;

    [Header("Stats")]
    [SerializeField] private int maxHealth = 1000;
    [SerializeField] private int currentHealth;
    [SerializeField] private float lookSpeed = 2f;

    [Header("Swing attack")]
    [SerializeField] private int swingDamage = 20;
    [SerializeField] private float swingCooldown = 10f;

    [Header("Smash attack")]
    [SerializeField] private int smashDamage = 100;
    [SerializeField] private float smashCooldown = 10;

    [Header("Flame attack")]
    [SerializeField] private int flameDamage = 10;
    [SerializeField] private float flameCooldown = 20f;

    [Header("References")]
    [SerializeField] private Transform player;

    public enum BossState
    {
        Idle,
        SwingAttack,
        SmashAttack,
        FlameAttack
    }

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        StateMachine();
    }

    private void StateMachine()
    {
        LookAtPlayer();

        switch (currentState)
        {
            case BossState.SwingAttack:
                Swing();
                break;
            case BossState.SmashAttack:
                Smash();
                break;
            case BossState.FlameAttack:
                ShootFlames();
                break;
        }
    }

    private void LookAtPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, lookSpeed * Time.deltaTime);
    }

    /// <summary>
    /// During the swing attack, the boss will swing his arms close to the ground,
    /// the player can try to outrun them or jump to avoid them.
    /// </summary>
    private void Swing()
    {

    }

    /// <summary>
    /// During the smash attack, the boss will smash his arm onto the ground,
    /// killing the player if hit, and shooting rocks that will also damage the player.
    /// </summary>
    private void Smash()
    {

    }

    /// <summary>
    /// During the flame attack, the boss will shoot flames in all directions, damaging the player.
    /// </summary>
    private void ShootFlames()
    {

    }

    public void Damage(int damage)
    {
        Debug.Log("Boss \"" + transform.name + "\" damaged by " + damage);
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (currentHealth == 0) Die();
    }

    private void Die()
    {
        Debug.Log("Boss died");
    }
}
