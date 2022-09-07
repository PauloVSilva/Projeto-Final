using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class SharpshooterManager : MonoBehaviour{
    //INSTANCES
    public static SharpshooterManager instance = null;

    //ENUMS
    public enum GameState{preparation, gameSetUp, gameIsRunning, gameOverSetUp, gameOver}
    public enum GameGoal{killCount, lastStanding}

    //MINIGAME VARIABLES
    [SerializeField] private GameObject[] spawnersList;
    [SerializeField] private List<PlayerInput> playersAlive = new List<PlayerInput>();
    [SerializeField] private GameState gameState;
    [SerializeField] private GameGoal gameGoal;
    [SerializeField] private float countDown;
    [SerializeField] private int killCountGoal;

    //EVENTS
    public static event Action<float> OnCountDownTicks;

    private void Awake(){
        if (instance == null){
            instance = this;
        }
        else if (instance != null){
            Destroy(gameObject);
        }

        GameManager.instance.joinAction.Disable();
        GameManager.instance.leaveAction.Disable();
        GameManager.instance.SetSpawnPoint();
        spawnersList = GameObject.FindGameObjectsWithTag("Spawner");
    }

    private void Start(){
        gameState = GameState.preparation;
        gameGoal = GameGoal.lastStanding;
        countDown = 10;
        killCountGoal = 10;
        //move players to spawn
        foreach(var playerInput in GameManager.instance.playerList){
            playerInput.transform.GetComponent<CharacterEvents>().ResetScores();
            playerInput.transform.GetChild(0).position = GameManager.instance.spawnPoints[0].transform.position;
            playerInput.GetComponent<PlayerInput>().actions.Disable();
            playerInput.GetComponent<PlayerInput>().actions["Jump"].Enable();
        
            playerInput.GetComponent<CharacterEvents>().OnPlayerScoredKill += VerifyWinCondition;
            playerInput.GetComponent<CharacterEvents>().OnPlayerDied += VerifyWinCondition;
            playerInput.GetComponent<CharacterEvents>().OnPlayerBorn += VerifyWinCondition;

            if(gameGoal == GameGoal.lastStanding){
                playerInput.transform.GetComponent<CharacterStats>().totalLives = 3;
                playerInput.transform.GetComponent<CharacterStats>().unlimitedLives = false;
            }
        }
        StartCoroutine(Preparation());
    }

    private void OnDisable() {
        foreach(var playerInput in GameManager.instance.playerList){
            playerInput.GetComponent<CharacterEvents>().OnPlayerScoredKill -= VerifyWinCondition;
            playerInput.GetComponent<CharacterEvents>().OnPlayerDied -= VerifyWinCondition;
            playerInput.GetComponent<CharacterEvents>().OnPlayerBorn -= VerifyWinCondition;
        }
    }

    IEnumerator Preparation(){
        yield return new WaitForSeconds(1f);
        if(countDown > 0){
            countDown--;
            OnCountDownTicks?.Invoke(countDown);
            StartCoroutine(Preparation());
        }
        else {
            Debug.Log("Preparation time ended");
            gameState++;
            GameSetUp();
        }
    }

    private void GameSetUp(){
        foreach(var playerInput in GameManager.instance.playerList){
            playersAlive.Add(playerInput);
            playerInput.GetComponent<PlayerInput>().actions["Movement"].Enable();
            playerInput.GetComponent<PlayerInput>().actions["Sprint"].Enable();
            playerInput.GetComponent<PlayerInput>().actions["Jump"].Enable();
            playerInput.GetComponent<PlayerInput>().actions["Dash"].Enable();
            playerInput.GetComponent<PlayerInput>().actions["Interact"].Enable();
            playerInput.GetComponent<PlayerInput>().actions["CockHammer"].Enable();
            playerInput.GetComponent<PlayerInput>().actions["PressTrigger"].Enable();
            playerInput.GetComponent<PlayerInput>().actions["ReloadWeapon"].Enable();
        }
        gameState++;
    }

    private void VerifyWinCondition(GameObject player){
        if(gameGoal == GameGoal.lastStanding){
            if(!player.transform.parent.GetComponent<CharacterStats>().CanRespawn()){
                playersAlive.Remove(player.transform.parent.GetComponent<PlayerInput>());
            }
            if (playersAlive.Count == 1){
                Debug.Log("Player " + playersAlive[0].transform.GetComponent<CharacterStats>().animal.ToString() + " is the winner");
                gameState++;
                GameOverSetUp();
            }
        }
        if(gameGoal == GameGoal.killCount){
            if (player.transform.parent.GetComponent<CharacterStats>().kills >= killCountGoal){
                Debug.Log("Player " + player.transform.parent.GetComponent<CharacterStats>().animal.ToString() + " is the winner");
                gameState++;
                GameOverSetUp();
            }
        }
    }

    private void GameOverSetUp(){
        countDown = 10;
        gameState++;
        StartCoroutine(GameOver());
    }

    IEnumerator GameOver(){
        yield return new WaitForSeconds(1f);
        if(countDown > 0){
            countDown--;
            OnCountDownTicks?.Invoke(countDown);
            StartCoroutine(GameOver());
        }
        else {
            Debug.Log("Returning to MainHub");
            GameManager.instance.ReturnToMainHub();
        }
    }
}
