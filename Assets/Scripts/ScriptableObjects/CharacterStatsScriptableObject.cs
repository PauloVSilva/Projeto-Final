using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "CharacterStats")]

public class CharacterStatsScriptableObject : ScriptableObject{
    [SerializeField] public enum Animal{hedgehog, pangolin, threeBandedArmadillo}

    [Header("Race")]
    [SerializeField] public Animal _animal;

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
