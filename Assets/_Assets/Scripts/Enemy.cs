using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour,IHittable
{
    public enum State {
    Idle,
    Shooting,
    Dead,
    }
    public static Enemy Instance { get; private set; }
    public event EventHandler OnHealthChanged;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private Transform launchPoint;
    [SerializeField] private GameObject enemyProjectile;
    [SerializeField] private float shootInterval = 2f;
    [SerializeField]private Vector3 launchVector;
    [SerializeField]private float launchSpeed =10f;
    private float shootTimer;
    private int health;

    private State enemyState;


    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
        health = maxHealth;
        enemyState= State.Shooting;
    }

    public void Update() {
        switch (enemyState) {
            case State.Idle:
                break;
            case State.Shooting:
                Shoot();
                break;
            case State.Dead:
                break;
        }
    }

    private void Shoot() {
        if (shootTimer > 0) {
            shootTimer -= Time.deltaTime;
            return;
        }
        shootTimer = shootInterval;
        //Launch the projectile such that it follows a projectile motion trajectory towards player
        GameObject projectile = Instantiate(enemyProjectile, launchPoint.position, launchPoint.rotation);
        projectile.GetComponent<Rigidbody>().velocity = launchVector * launchSpeed;

    }
    public HittableType GetHittableType() {
        return HittableType.Enemy;
    }

    public void Hit(BaseProjectile projectile) {
        health-=projectile.GetDamage();
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
        if (health <= 0) {
         enemyState = State.Dead;
        }
    }

    public float GetHealthNormalized() {
        return (float)health / maxHealth;
    }
    
}
