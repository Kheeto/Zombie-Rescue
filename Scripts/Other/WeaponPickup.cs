using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [Header("Weapon Pickup")]
    [SerializeField] private float maxDistance = 2f;

    [Header("References")]
    [SerializeField] private GameObject enabledWeapon;
    [SerializeField] private GameObject disabledWeapon;
    [SerializeField] private Weapon newWeapon;
    [SerializeField] private PlayerCombat player;

    private void Update()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < maxDistance)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (enabledWeapon != null) enabledWeapon.SetActive(true);
                if (disabledWeapon != null) disabledWeapon.SetActive(false);
                if (newWeapon != null) player.weapon = newWeapon;
                player.animator = enabledWeapon.GetComponent<Animator>();
                Instantiate(newWeapon.pickupSound, player.transform.position, Quaternion.identity, player.transform);
                Destroy(gameObject);
            }
        }
    }
}
