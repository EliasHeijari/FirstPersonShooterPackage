using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    public static Player Instance;
    public WeaponHandling WeaponHandling{get; private set;}

    private int health = 100;

    public int Health {
        get{ return health; }
        set { health = value; }
    }

    private void Start() {
        if (Instance == null){
            Instance = this;
        }
        else if(Instance != this){
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    private void Awake(){
        WeaponHandling = GetComponent<WeaponHandling>();
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Health = 0;
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player Died");
    }
}
