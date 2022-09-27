using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "CharacterStats")]

public class CharacterStatsScriptableObject : ScriptableObject{
    [Space(5)]
    [Header("Models/Skins")]
    public GameObject[] characterModel;
    public Sprite[] sprite;
    public string characterName;

    [Space(5)]
    [Header("Size")]
    public Vector3 _characterControllerCenter;
    public float _characterControllerRadius;
    public float _characterControllerHeight;

    [Space(5)]
    [Header("Race")]
    public Animal _animal;

    [Space(5)]
    [Header("Health")]
    public float _maxHealth;
    public float _healthRegenRate; 
    
    [Space(5)]
    [Header("Movement")]
    public float _walkSpeed;
    public float _sprintSpeed;
    public float _maxStamina;
    public float _staminaRegenRate;
    public float _jumpStrength;
    public int _totalJumps;
    public int _jumpStaminaCost;
    public int _dashStaminaCost;
    public int _sprintStaminaCost;

} 
