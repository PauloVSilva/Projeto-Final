using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CharacterStat : MonoBehaviour{
    public CharacterStatScriptableObject Character;

    [SerializeField] public enum Animal{hedgehog, pangolin, threeBandedArmadillo}

    [SerializeField] public enum PlayerColor{blue, red, green, yellow}

    //VARIABLES THAT WILL COME FROM SCRIPTABLE OBJECT
    [SerializeField] private Animal animal;
    [SerializeField] private float maxHealth;
    [SerializeField] private float healthRegenRate;
    [SerializeField] private float movSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float maxStamina;
    [SerializeField] private float staminaRegenRate;
    [SerializeField] private float jumpStrength;
    [SerializeField] private int extraJumps;

    //OTHER VARIABLES THAT WILL BE USEFUL INGAME
    [SerializeField] public PlayerColor playerColor;
    [SerializeField] public int score;
    [SerializeField] public int kills;
    [SerializeField] public int deaths;
    [SerializeField] public bool unlimitedLives;
    [SerializeField] public int extraLives;
    [SerializeField] public float timeToRespawn;

    //EVENTS
    public event Action<int> OnScoreChanged;
    public event Action<int> OnKillsChanged;
    public event Action<int> OnDeathsChanged;
    public event Action<float> OnHealthUpdated; 
    public event Action<float> OnWasHealed;
    public event Action<float> OnWasDamaged;

    private void Awake(){
        GetScriptableObjectVariables();
    }

    private void Start(){
        InitializeInternalVariables();
    }

    private void GetScriptableObjectVariables(){
        animal = (Animal)Character._animal;
        maxHealth = Character._maxHealth;
        healthRegenRate = Character._healthRegenRate;
        movSpeed = Character._movSpeed;
        sprintSpeed = Character._sprintSpeed;
        maxStamina = Character._maxStamina;
        staminaRegenRate = Character._staminaRegenRate;
        jumpStrength = Character._jumpStrength;
        extraJumps = Character._extraJumps;
    }

    private void InitializeInternalVariables(){
        playerColor = PlayerColor.blue;
        score = 0;
        kills = 0;
        deaths = 0;
        unlimitedLives = true;
        extraLives = 0;
        timeToRespawn = 3f;
    } 
}
