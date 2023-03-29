using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "CharacterStats")]

[System.Serializable]
public class CharacterSkin
{
    public GameObject characterModel;
    public Sprite characterSprite;
    public string characterName;
}

public class CharacterStatsScriptableObject : ScriptableObject
{
    [Space(5)]
    [Header("Models/Skins")]
    public CharacterSkin[] characterSkin;

    public GameObject tombstone;

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
