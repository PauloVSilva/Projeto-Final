using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHealthSystem : HealthSystem
{
    private CharacterManager characterManager;

    [SerializeField] private GameObject deathVFX;
    [SerializeField] private GameObject damageVFX;

    //VARIABLES THAT WILL COME FROM CHARACTER SCRIPTABLE OBJECT
    public override float MaxHealth {get; protected set;}
    public float HealthRegenRate {get; protected set;}

    //VARIABLES FOR INTERNAL USE
    public override float CurrentHealth {get; protected set;}
    public float HealthRegenCooldown {get; protected set;}
    public bool CanRegenHealth {get; protected set;}
    public override bool IsAlive {get; protected set;}
    public override bool IsInvulnerable {get; protected set;}

    public GameObject lastDamagingPlayer;
    public float lastDamagingPlayerTime;


    private void Awake()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        characterManager = GetComponent<CharacterManager>();
    }

    public void Initialize()
    {
        GetScriptableObjectVariables();
        InitializeVariables();

        StartCoroutine(OnEntityBornDelay());
        IEnumerator OnEntityBornDelay()
        {
            yield return new WaitForEndOfFrame();

            characterManager.InvokeOnPlayerBorn(gameObject);
            characterManager.UpdateCharacterState(CharacterState.Alive);
        }
    }

    private void GetScriptableObjectVariables()
    {
        MaxHealth = characterManager.Character.maxHealth;
        HealthRegenRate = characterManager.Character.healthRegenRate;
    }

    public override void InitializeVariables()
    {
        lastDamagingPlayer = null;

        CurrentHealth = MaxHealth;
        IsAlive = true;
        IsInvulnerable = false;
        SendHealthUpdateEvent();
    }

    private void Update()
    {
        RegenerateHealth();
        LastDamageSourceRetentionTime();
    }

    private void RegenerateHealth()
    {
        if (characterManager.characterState == CharacterState.Dead) return;

        if (IsAlive && CanRegenHealth)
        {
            CurrentHealth = Math.Min(CurrentHealth += HealthRegenRate * Time.deltaTime, MaxHealth);
            SendHealthUpdateEvent();
        }

        if(HealthRegenCooldown > 0)
        {
            HealthRegenCooldown -= Time.deltaTime;
        }

        if(HealthRegenCooldown <= 0)
        {
            HealthRegenCooldown = 0;
            CanRegenHealth = true;
        }
    }

    private void LastDamageSourceRetentionTime()
    {
        lastDamagingPlayerTime = Math.Max(lastDamagingPlayerTime -= Time.deltaTime, 0);

        if(lastDamagingPlayerTime == 0)
        {
            lastDamagingPlayer = null;
        }
    }

    private void SendHealthUpdateEvent()
    {
        characterManager.InvokeOnPlayerHealthUpdated(CurrentHealth, MaxHealth);
    }

    public void TakeDamage(float damageTaken)
    {
        TakeDamage(null, damageTaken);
    }

    public override void TakeDamage(GameObject _damageSource, float damageTaken){
        if (IsInvulnerable) return;

        CurrentHealth = Math.Max(CurrentHealth -= damageTaken, 0);
        
        if(_damageSource != null && _damageSource.CompareTag("Player"))
        {
            lastDamagingPlayer = _damageSource;
            lastDamagingPlayerTime = 3f;
        }
        
        if(CurrentHealth <= 0)
        {
            if(lastDamagingPlayer != null)
            {
                Die(lastDamagingPlayer);
            }
            else
            {
                Die(_damageSource);
            }
        }
        
        characterManager.InvokeOnPlayerWasDamaged(damageTaken);
        HealthRegenCooldown = 1f;
        CanRegenHealth = false;
        SendHealthUpdateEvent();
        
    }

    public override void Heal(float heal)
    {
        CurrentHealth = Math.Min(CurrentHealth += heal, MaxHealth);
        characterManager.InvokeOnPlayerWasHealed(heal);
        SendHealthUpdateEvent();
    }

    public override void Die(GameObject _damageSource)
    {
        IsAlive = false;
        IsInvulnerable = true;
        CurrentHealth = 0;

        characterManager.PlayerDied(gameObject);
        characterManager.UpdateCharacterState(CharacterState.Dead);

        if(_damageSource != null && _damageSource.CompareTag("Player"))
        {
            _damageSource.transform.TryGetComponent(out CharacterManager _characterManager);
            _characterManager.PlayerScoredKill(_damageSource);
        }

        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        GameObject _deathVFX = Instantiate(deathVFX, pos, transform.rotation, null);
        Destroy(_deathVFX, 1f);
    }
}
