using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Animal{hedgehog, pangolin, threeBandedArmadillo}
public enum TeamColor{none, blue, red, green, yellow}

public class CharacterStats : MonoBehaviour{
    [SerializeField] private CharacterWeaponSystem characterWeaponSystem;
    public CharacterStatsScriptableObject Character;

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

    //stats useful to determine what actions the player can or can not perform
    [SerializeField] public bool actionsAreBlocked;
    [SerializeField] public bool isArmed;
    [SerializeField] public bool isMountedOnTurret;

    #region "Character attributes that will come from scriptable object"
    [Space(5)]
    [Header("Character Base Stats")]
    //CHARACTER ATTRIBUTES THAT WILL COME FROM SCRIPTABLE OBJECT
    [SerializeField] public Animal animal;
    [SerializeField] private float maxHealth;
    [SerializeField] private float healthRegenRate;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float maxStamina;
    [SerializeField] private float staminaRegenRate;
    [SerializeField] private float jumpStrength;
    [SerializeField] private int totalJumps;
    [SerializeField] private int jumpStaminaCost;
    [SerializeField] private int dashStaminaCost;
    [SerializeField] private int sprintStaminaCost;

    public float MaxHealth => maxHealth;
    public float HealthRegenRate => healthRegenRate;
    public float WalkSpeed => walkSpeed;
    public float SprintSpeed => sprintSpeed;
    public float MaxStamina => maxStamina;
    public float StaminaRegenRate => staminaRegenRate;
    public float JumpStrength => jumpStrength;
    public int TotalJumps => totalJumps;
    public int JumpStaminaCost => jumpStaminaCost;
    public int DashStaminaCost => dashStaminaCost;
    public int SprintStaminaCost => sprintStaminaCost;
    #endregion "Character attributes that will come from scriptable object"

    private void Awake(){
        InitializePlayerVariables();
    }

    private void InitializePlayerVariables(){
        teamColor = TeamColor.none;
        score = 0;
        kills = 0;
        deaths = 0;
        unlimitedLives = true;
        totalLives = 0;
        timeToRespawn = 3f;

        actionsAreBlocked = false;
        isArmed = IsArmed();
        isMountedOnTurret = false;
    }

    public void SetStats(){
        SetCharacter();
        InitializeCharacterVariables();
    }

    private void SetCharacter(){
        Character = GetComponent<CharacterSelection>().Character;
    }

    private void InitializeCharacterVariables(){
        animal = Character._animal;
        maxHealth = Character._maxHealth;
        healthRegenRate = Character._healthRegenRate;
        walkSpeed = Character._walkSpeed;
        sprintSpeed = Character._sprintSpeed;
        maxStamina = Character._maxStamina;
        staminaRegenRate = Character._staminaRegenRate;
        jumpStrength = Character._jumpStrength;
        totalJumps = Character._totalJumps;
        jumpStaminaCost = Character._jumpStaminaCost;
        dashStaminaCost = Character._dashStaminaCost;
        sprintStaminaCost = Character._sprintStaminaCost;
    }

    public void ResetStats(){
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

    public void SetLimitedLives(int _amount){
        totalLives = _amount;
        unlimitedLives = false;
    }

    public void IncreaseLives(int _amount){
        totalLives += _amount;
    }

    public void SetUnlimitedLives(){
        totalLives = -1;
        unlimitedLives = true;
    }




    public bool CanRespawn(){
        if(totalLives > 0 || unlimitedLives){
            return true;
        }
        else{
            return false;
        }
    }

    public bool CanMove(){
        if(actionsAreBlocked) return false;
        
        return true;
    }

    public bool CanUseFireGun(){
        if(actionsAreBlocked) return false;
        if(!isArmed) return false;

        return true;
    }

    public bool IsArmed(){
        if(characterWeaponSystem.GetWeapon() != null){
            return true;
        }
        else{
            return false;
        }
    }

}
