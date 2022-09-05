using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class PlayerStatManager : MonoBehaviour{
    //VARIABLES
    public enum playerColor{blue, red, green, yellow}
    public playerColor thisPlayerColor;
    public int score;
    public int kills;
    public int deaths;
    private bool infinityLives;
    private int extraLives;
    private int ammo;
    private float timeToRespawn;

    //EVENTS
    public event Action<int> OnScoreChanged;
    public event Action<int> OnKillsChanged;
    public event Action<int> OnDeathsChanged;
    public event Action<float> OnHealthUpdated;    

    //STATIC EVENTS THAT WILL BE REPLACED SOON
    public static event Action<GameObject> OnPlayerDied;
    public static event Action<GameObject> OnPlayerKilled;
    public static event Action<GameObject> OnPlayerReborn;

    private void Awake() {
        thisPlayerColor = playerColor.blue;
        score = 0;
        kills = 0;
        deaths = 0;
        infinityLives = true;
        extraLives = 1;
        ammo = 0;
        timeToRespawn = 3f;
    }

    private void Start(){
        SubscribeToEvents();
        OnPlayerReborn?.Invoke(gameObject);
    }

    private void SubscribeToEvents(){
        gameObject.transform.GetChild(0).GetComponent<HealthSystem>().OnEntityDied += PlayerDied;
        gameObject.transform.GetChild(0).GetComponent<HealthSystem>().OnEntityScoredKill += PlayedScoredKill;
        gameObject.transform.GetChild(0).GetComponent<HealthSystem>().OnEntityBorn += PlayerIsBorn;
        gameObject.transform.GetChild(0).GetComponent<HealthSystem>().OnEntityHealthUpdated += PlayerHealthUpdated;
    }

    public void FilterCollision(GameObject player, GameObject gameObject){
        if(gameObject.CompareTag("Coin")){
            if(gameObject.GetComponent<Coin>().canBePickedUp){
                IncreaseScore(gameObject.GetComponent<Coin>().value);
                Destroy(gameObject);
            }
        }
        if(gameObject.CompareTag("Instadeath")){
            player.GetComponent<HealthSystem>().Kill();
        }
    }

    public void IncreaseScore(int value){
        score += value;
        OnScoreChanged?.Invoke(score);
    }

    private void PlayerHealthUpdated(float currentHealth){
        OnHealthUpdated?.Invoke(currentHealth);
    }

    public void PlayerDied(GameObject player){
        deaths++;
        OnDeathsChanged?.Invoke(deaths);
        OnPlayerDied?.Invoke(player);
        if(extraLives > 0 || infinityLives){
            StartCoroutine(TimerToRespawn(timeToRespawn));
        }
        gameObject.GetComponent<PlayerInput>().actions.Disable();
        player.transform.position = GameManager.instance.spawnPoints[0].transform.position;
        player.SetActive(false);
    }

    IEnumerator TimerToRespawn(float delay){
        yield return new WaitForSeconds(delay);
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        RespawnPlayer(); 
    }

    public void RespawnPlayer(){
        gameObject.transform.GetChild(0).GetComponent<HealthSystem>().Spawn();
        gameObject.GetComponent<PlayerInput>().actions.Enable();
        OnPlayerReborn?.Invoke(gameObject);
        if(!infinityLives){
            extraLives--;
        }
    }

    public void PlayerIsBorn(GameObject player){
        OnHealthUpdated?.Invoke(player.GetComponent<HealthSystem>().currentHealth);
    }

    public void PlayedScoredKill(GameObject player){
        OnPlayerKilled?.Invoke(player);
        player.GetComponent<PlayerStatManager>().kills++;
        player.GetComponent<PlayerStatManager>().UpdateKills();
    }

    public void UpdateKills(){
        OnKillsChanged?.Invoke(kills);
    }

    public void ResetScores(){
        score = 0;
        kills = 0;
        deaths = 0;
        infinityLives = true;
        extraLives = 1;
        ammo = 0;
        timeToRespawn = 3f;
    }
}
