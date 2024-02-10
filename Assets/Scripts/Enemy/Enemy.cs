using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class Enemy : MonoBehaviour, IDamageable
{
    private int health;
    public int damage { get; private set; } = 8;
    public static event EventHandler OnEnemyDie;
    public NavMeshAgent navMeshAgent {get; private set;}
    [SerializeField] private Transform[] patrolPoints;
    public Vector3[] PatrolPoints 
    {
        get
        {
            Vector3[] points = new Vector3[patrolPoints.Length];
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                points[i] = patrolPoints[i].position;
            }
            return points;
        }
    }
    private void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void TakeDamage(int damage){
        health -= damage;
        if (health <= 0)
            health = 0;
            Die();
    }

    private void Die()
    {
        OnEnemyDie?.Invoke(this, EventArgs.Empty);

        // Death Logic
        Destroy(gameObject);
    }


}
