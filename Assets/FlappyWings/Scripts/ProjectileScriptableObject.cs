using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile", menuName = "Projectiles")]

public class ProjectileScriptableObject : ScriptableObject{
    public float DamageAmount = 10f;
    public float Cost = 1f;
    public float LifeTime = 2f;
    public float Speed = 15f;
    public float ProjectileRadius = 0.5f;

    //Status effects
    //Thumbnail
    //Time between casts
    //Modifiers
}
