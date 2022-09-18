using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour{
    public CharacterStatsScriptableObject Character;

    [SerializeField] public enum Animal{hedgehog, pangolin, threeBandedArmadillo}

    [SerializeField] public enum TeamColor{blue, red, green, yellow}

    //PLAYER VARIABLES
    [Space(5)]
    [Header("Player Stats")]
    [SerializeField] public TeamColor teamColor;
    [SerializeField] public int score;
    [SerializeField] public int kills;
    [SerializeField] public int deaths;
    [SerializeField] public bool unlimitedLives;
    [SerializeField] public int totalLives;
    [SerializeField] public float timeToRespawn;
    [SerializeField] public bool isArmed;

    [Space(5)]
    [Header("Character Base Stats")]
    //CHARACTER VARIABLES THAT WILL COME FROM SCRIPTABLE OBJECT
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

    private void Awake(){
        InitializePlayerVariables();
    }

    private void InitializePlayerVariables(){
        teamColor = TeamColor.blue;
        score = 0;
        kills = 0;
        deaths = 0;
        unlimitedLives = true;
        totalLives = 0;
        timeToRespawn = 3f;
        isArmed = false;
    }

    public void SetStats(){
        GetCharacter();
        InitializeCharacterVariables();
    }

    private void GetCharacter(){
        Character = GetComponent<CharacterSelection>().Character;
    }

    private void InitializeCharacterVariables(){
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

    public void ResetScores(){
        InitializePlayerVariables();
        InitializeCharacterVariables();
    }

    public void SetTeam(TeamColor _teamColor){
        teamColor = _teamColor;
    }

    public void IncreaseScore(int _score){
        score += _score;
    }

    public int GetScore(){
        return score;
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