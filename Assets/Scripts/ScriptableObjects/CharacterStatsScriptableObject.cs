using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "CharacterStats")]

public class CharacterStatsScriptableObject : ScriptableObject{
    [SerializeField] public enum Animal{hedgehog, pangolin, threeBandedArmadillo}

    [Space(5)]
    [Header("Models/Skins")]
    [SerializeField] public GameObject[] characterModel;
    [SerializeField] public Sprite[] sprite;
    [SerializeField] public string characterName;

    [Space(5)]
    [Header("Race")]
    [SerializeField] public Animal _animal;

    [Space(5)]
    [Header("Health")]
    [SerializeField] public float _maxHealth;
    [SerializeField] public float _healthRegenRate; 
    
    [Space(5)]
    [Header("Movement")]
    [SerializeField] public float _movSpeed;
    [SerializeField] public float _sprintSpeed;
    [SerializeField] public float _maxStamina;
    [SerializeField] public float _staminaRegenRate;
    [SerializeField] public float _jumpStrength;
    [SerializeField] public int _totalJumps;

} 
