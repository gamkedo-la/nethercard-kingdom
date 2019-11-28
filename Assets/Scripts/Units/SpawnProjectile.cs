using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnProjectile : MonoBehaviour
{
    
    [SerializeField]
    private float attackRate = 1.0f;
    [SerializeField]
    private float initialAttackDelay = 0.5f;
    [SerializeField]
    private GameObject projectilePrefab = null;
    [SerializeField]
    private Transform attackOrigin = null;

    void Awake()
    {
        InvokeRepeating("Attack", initialAttackDelay, attackRate);
    }

    private void Attack()
    {
        GameObject attackProjectile = Instantiate(projectilePrefab, attackOrigin);
    }
}
