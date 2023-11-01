using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("Weapons")]
    public Weapon weapon;

    [Header("References")]
    [SerializeField] private Camera cam;
    public Animator animator;
    [SerializeField] private Recoil recoil;
    [SerializeField] private PauseMenu menu;

    [Header("Audio")]
    [SerializeField] private GameObject[] hitSounds;

    private bool canAttack;
    private bool reloading;

    private void Awake()
    {
        currentHealth = maxHealth;
        weapon.currentAmmo = weapon.reloadAmount;
        ResetAttack();
        reloading = false;
    }

    private void Update()
    {
        if (!animator.gameObject.activeInHierarchy) return;

        if (canAttack && Input.GetMouseButtonDown(0))
        {
            Attack();
        }
        if (weapon.type == WeaponType.Ranged && weapon.currentAmmo == 0 && Input.GetKeyDown(KeyCode.R) && !reloading)
        {
            animator.SetTrigger("Reload");
            reloading = true;
            Invoke(nameof(Reload), weapon.reloadDuration);

            if (!weapon.useShotgunSounds && weapon.reloadSound != null)
                Instantiate(weapon.reloadSound, transform.position, Quaternion.identity, transform);
            else if (weapon.useShotgunSounds)
                StartCoroutine(ShotgunReload());
        }
    }

    private void Attack()
    {
        if (reloading) return;

        PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
        if (pauseMenu != null && pauseMenu.isPaused) return;

        canAttack = false;
        Invoke(nameof(ResetAttack), weapon.attackSpeed);

        // Melee weapons can damage multiple enemies in a short range
        if (weapon.type == WeaponType.Melee)
        {
            Invoke(nameof(PlayShootSound), weapon.attackDelay);
            animator.SetTrigger("Shoot");
            Debug.Log("Attacking by melee");
            RaycastHit[] hits = Physics.SphereCastAll(
                cam.transform.position, weapon.hitRadius, cam.transform.forward, weapon.range, weapon.whatIsEnemy);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject == gameObject) continue;
                Debug.Log("Hit object \"" + hit.collider.name + "\"");
                StartCoroutine(HitEnemy(hit));
            }
        }
        // Ranged weapons can damage a single enemy in a longer range and have limited ammo
        else if (weapon.type == WeaponType.Ranged && weapon.currentAmmo > 0)
        {
            PlayShootSound();
            animator.SetTrigger("Shoot");
            Invoke(nameof(ShotgunPump), weapon.pumpDelay);
            recoil.FireRecoil();
            Debug.Log("Shooting a bullet");
            weapon.currentAmmo--;
            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, weapon.range, weapon.whatIsEnemy))
            {
                Debug.Log("Hit object \"" + hit.collider.name + "\"");
                StartCoroutine(HitEnemy(hit));
            }
        }
        else if (weapon.type == WeaponType.Ranged && weapon.currentAmmo <= 0)
        {
            if (weapon.noAmmoSound != null) Instantiate(weapon.noAmmoSound, transform.position, Quaternion.identity, transform);
        }
    }

    IEnumerator HitEnemy(RaycastHit hit)
    {
        yield return new WaitForSeconds(weapon.attackDelay);

        EnemyController enemy = hit.collider.gameObject.GetComponentInParent<EnemyController>();
        if (enemy != null) enemy.Damage(weapon.damage);

        BossController boss = hit.collider.gameObject.GetComponentInParent<BossController>();
        if (boss != null) boss.Damage(weapon.damage);

        Vector3 hitDirection = (cam.transform.position - hit.point).normalized;
        if (weapon.type == WeaponType.Ranged)
            Instantiate(weapon.bulletImpact, hit.point, Quaternion.LookRotation(hitDirection, cam.transform.up));

        Rigidbody rb = hit.collider.gameObject.GetComponent<Rigidbody>();
        if (rb != null) rb.AddForceAtPosition(-hitDirection * weapon.rigidbodyForce, hit.point, ForceMode.Impulse);
    }

    private void PlayShootSound()
    {
        if (weapon.shootSound != null) Instantiate(weapon.shootSound, transform.position, Quaternion.identity, transform);
    }

    private void Reload()
    {
        weapon.currentAmmo = weapon.reloadAmount;
        reloading = false;
    }

    public void Damage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, 100);
        int i = Random.Range(0, hitSounds.Length);
        Instantiate(hitSounds[i], transform.position, Quaternion.identity);
        if (currentHealth == 0) Die();
    }

    private void Die()
    {
        Debug.Log("Player died");
        menu.PlayerDied();
    }

    private void ResetAttack()
    {
        canAttack = true;
    }
    
    IEnumerator ShotgunReload()
    {
        for (int i = 0; i < weapon.reloadAmount; i++)
        {
            yield return new WaitForSeconds(weapon.bulletInterval);
            int sound = Random.Range(0, weapon.reloadSounds.Length);
            Instantiate(weapon.reloadSounds[sound], transform.position, Quaternion.identity, transform);
        }
    }

    private void ShotgunPump()
    {
        if (weapon.pumpSounds != null && weapon.pumpSounds.Length > 0)
        {
            int sound = Random.Range(0, weapon.pumpSounds.Length);
            Instantiate(weapon.pumpSounds[sound], transform.position, Quaternion.identity, transform);
        }
    }
}
