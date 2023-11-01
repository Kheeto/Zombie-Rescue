using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class Weapon : ScriptableObject
{
    [Header("Settings")]
    public WeaponType type;
    public int damage;
    public float attackSpeed;
    public float attackDelay;
    public float range;
    public LayerMask whatIsEnemy;
    public float rigidbodyForce;

    [Header("Melee Weapon")]
    public float hitRadius;

    [Header("Ranged Weapon")]
    public int currentAmmo;
    public int reloadAmount;
    public float reloadDuration;

    [Header("References")]
    public GameObject shootSound;
    public GameObject noAmmoSound;
    public GameObject reloadSound;
    public GameObject pickupSound;

    [Header("Shotgun Reload")]
    public bool useShotgunSounds;
    public float bulletInterval;
    public float pumpDelay;
    public GameObject[] pumpSounds;
    public GameObject[] reloadSounds;

    [Header("Bullet impact")]
    public GameObject bulletImpact;
}

public enum WeaponType
{
    Melee,
    Ranged
}