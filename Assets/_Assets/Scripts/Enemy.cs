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
    [SerializeField] private int maxHealth = 100;
    private int health;

    private State enemyState;

    private void Awake() {
        health = maxHealth;
        enemyState= State.Idle;
    }
    public HittableType GetHittableType() {
        return HittableType.Enemy;
    }

    public void Hit(BaseProjectile projectile) {
        if (projectile.GetOwner() == ProjectileOwner.Enemy)
            return;
        health-=projectile.GetDamage();
        if (health <= 0) {
         enemyState = State.Dead;
        }
    }

    
}
