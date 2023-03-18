using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "CharacterStats")]

public class CharacterStatsScriptableObject : ScriptableObject{
    [Space(5)]
    [Header("Models/Skins")]
    public GameObject[] characterModel;
    public GameObject tombstone;
    public Sprite[] sprite;
    public string characterName;

    [Space(5)]
    [Header("Size")]
    public Vector3 characterControllerCenter;
    public float characterControllerRadius;
    public float characterControllerHeight;

    [Space(5)]
    [Header("Race")]
    public Animal animal;

    [Space(5)]
    [Header("Health")]
    public float maxHealth;
    public float healthRegenRate; 
    
    [Space(5)]
    [Header("Movement")]
    public float walkSpeed;
    public float sprintSpeed;
    public float maxStamina;
    public float staminaRegenRate;
    public float jumpStrength;
    public int totalJumps;
    public int jumpStaminaCost;
    public int dashStaminaCost;
    public int sprintStaminaCost;

} 
