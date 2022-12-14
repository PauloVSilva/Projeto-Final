using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile", menuName = "Projectiles")]

public class ProjectileScriptableObject : ScriptableObject{
    [SerializeField] public GameObject projectileModel;
    [SerializeField] public Sprite sprite;
    [SerializeField] public string projectileName;
    [SerializeField] public CollisionType collisionType;
    [SerializeField] public float damageAmount;
    [SerializeField] public int cost;
    [SerializeField] public float maxAge;
    [SerializeField] public float speed;
}
