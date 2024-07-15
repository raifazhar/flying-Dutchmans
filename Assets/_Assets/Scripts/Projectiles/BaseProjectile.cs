using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public enum ProjectileOwner {
    Player,
    Enemy,
    Obstacle
}
public abstract class BaseProjectile : MonoBehaviour {

    [SerializeField]protected HittableType[] targets;
    [SerializeField]protected ProjectileOwner owner;

    public virtual int GetDamage() {
        Debug.LogError("BaseProjectile should not be instantiated");
        return 0;
    }
    public virtual HittableType[] GetTargets() {
        return targets;
    }
    public virtual ProjectileOwner GetOwner() {
        return owner;
    }
}
