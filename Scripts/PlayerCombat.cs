using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("Weapons")]
    [SerializeField] private Weapon weapon;

    [Header("References")]
    [SerializeField] private Camera cam;

    private int currentAmmo;
    private bool canAttack;

    private void Awake()
    {
        currentHealth = maxHealth;
        currentAmmo = weapon.ammoAmount;
        ResetAttack();
    }

    private void Update()
    {
        if (canAttack && Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }

    private void Attack()
    {
        canAttack = false;
        Invoke(nameof(ResetAttack), weapon.attackSpeed);

        // Melee weapons can damage multiple enemies in a short range
        if (weapon.type == WeaponType.Melee)
        {
            Debug.Log("Attacking by melee");
            RaycastHit[] hits = Physics.SphereCastAll(
                cam.transform.position, weapon.hitRadius, cam.transform.forward, weapon.range, weapon.whatIsEnemy);
            foreach (RaycastHit hit in hits)
            {
                Debug.Log("Hit object \"" + hit.collider.name + "\"");
                EnemyController enemy = hit.collider.gameObject.GetComponentInParent<EnemyController>();
                if (enemy != null) enemy.Damage(weapon.damage);

                BossController boss = hit.collider.gameObject.GetComponentInParent<BossController>();
                if (boss != null) boss.Damage(weapon.damage);
            }
        }
        // Ranged weapons can damage a single enemy in a longer range and have limited ammo
        else if (weapon.type == WeaponType.Ranged && currentAmmo > 0)
        {
            Debug.Log("Shooting a bullet");
            currentAmmo--;
            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, weapon.range, weapon.whatIsEnemy))
            {
                Debug.Log("Hit object \"" + hit.collider.name + "\"");
                EnemyController enemy = hit.collider.gameObject.GetComponent<EnemyController>();
                if (enemy) enemy.Damage(weapon.damage);

                BossController boss = hit.collider.gameObject.GetComponentInParent<BossController>();
                if (boss != null) boss.Damage(weapon.damage);
            }
        }
    }

    public void Damage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, 100);
        if (currentHealth == 0) Die();
    }

    private void Die()
    {
        Debug.Log("Player died");
    }

    private void ResetAttack()
    {
        canAttack = true;
    }
}
