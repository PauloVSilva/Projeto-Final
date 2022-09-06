using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour{
    public CharacterStatsScriptableObject Character;

    [SerializeField] public enum Animal{hedgehog, pangolin, threeBandedArmadillo}

    [SerializeField] public enum TeamColor{blue, red, green, yellow}

    //VARIABLES THAT WILL COME FROM SCRIPTABLE OBJECT
    [SerializeField] public Animal animal;
    [SerializeField] private float maxHealth;
    [SerializeField] private float healthRegenRate;
    [SerializeField] private float movSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float maxStamina;
    [SerializeField] private float staminaRegenRate;
    [SerializeField] private float jumpStrength;
    [SerializeField] private int totalJumps;

    public float MaxHealth => maxHealth;
    public float HealthRegenRate => healthRegenRate;
    public float MovSpeed => movSpeed;
    public float SprintSpeed => sprintSpeed;
    public float MaxStamina => maxStamina;
    public float StaminaRegenRate => staminaRegenRate;
    public float JumpStrength => jumpStrength;
    public int TotalJumps => totalJumps;

    //OTHER VARIABLES THAT WILL BE USEFUL INGAME
    [SerializeField] public TeamColor teamColor;
    [SerializeField] public int score;
    [SerializeField] public int kills;
    [SerializeField] public int deaths;
    [SerializeField] public bool unlimitedLives;
    [SerializeField] public int totalLives;
    [SerializeField] public float timeToRespawn;

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
        totalJumps = Character._totalJumps;
    }

    private void InitializeInternalVariables(){
        teamColor = TeamColor.blue;
        score = 0;
        kills = 0;
        deaths = 0;
        unlimitedLives = true;
        totalLives = 0;
        timeToRespawn = 3f;
    }

    public void ResetScores(){
        GetScriptableObjectVariables();
        InitializeInternalVariables();
    }

    public void SetTeam(TeamColor _teamColor){
        teamColor = _teamColor;
    }

    public void IncreaseScore(int _score){
        score += _score;
    }

    public void IncreaseKills(){
        kills++;
    }

    public void IncreaseDeaths(){
        deaths++;
    }

    public void DecreaseLives(){
        totalLives--;
    }

    public void DecreaseLives(int amount){
        totalLives -= amount;
        if(totalLives < 0){
            totalLives = 0;
        }
    }

    public bool CanRespawn(){
        if(totalLives > 0 || unlimitedLives){
            return true;
        }
        else{
            return false;
        }
    }
}
