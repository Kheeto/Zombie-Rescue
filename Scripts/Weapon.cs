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
    public float range;
    public LayerMask whatIsEnemy;

    [Header("Melee Weapon")]
    public float hitRadius;

    [Header("Ranged Weapon")]
    public int ammoAmount;
}

public enum WeaponType
{
    Melee,
    Ranged
}