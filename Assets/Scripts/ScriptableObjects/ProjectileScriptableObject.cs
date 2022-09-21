using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile", menuName = "Projectiles")]

public class ProjectileScriptableObject : ScriptableObject{
    [SerializeField] public enum CollisionType{contact, explosive, passThrough}

    [SerializeField] public GameObject characterModel;
    [SerializeField] public Sprite sprite;
    [SerializeField] public string projectileName;
    [SerializeField] public CollisionType collisionType;
    [SerializeField] public float damageAmount;
    [SerializeField] public float cost;
    [SerializeField] public float lifeTime;
    [SerializeField] public float speed;
}
