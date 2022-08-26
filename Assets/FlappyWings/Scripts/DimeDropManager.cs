using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class DimeDropManager : MonoBehaviour{
    //INSTANCES
    public static DimeDropManager instance = null;

    public enum gameState{
        preparation,
        gameIsRunning,
        gameIsOver
    }
    [SerializeField] private gameState thisGameState = gameState.preparation;

    public float countDown = 10;
    public int goal = 100000;

    private void Awake(){
        if (instance == null){
            instance = this;
        }
        else if (instance != null){
            Destroy(gameObject);
        }

        GameManager.instance.SetSpawnPoint();
        foreach(var player in GameManager.instance.playerList){
            player.transform.parent.position = GameManager.instance.spawnPoints[0].transform.position;
        }

        //GameManager.instance.joinAction.Disable();
        //GameManager.instance.leaveAction.Disable();
    }

    private void Update(){
        if((int)thisGameState == 0){
            Preparation();
        }

        if((int)thisGameState == 1){
            GameIsRunning();
        }

        if((int)thisGameState == 2){
            GameIsOver();
        }

    }

    private void Preparation(){
        if(countDown > 0){
            countDown -= 1 * Time.deltaTime;
        }
        else {
            thisGameState++;
            Debug.Log("Preparation time ended");
        }
    }

    private void GameIsRunning(){
        foreach(var player in GameManager.instance.playerList){
            if (player.transform.parent.GetComponent<PlayerController>().score >= goal){
                Debug.Log("Player " + player.transform.parent.GetComponent<PlayerController>().thisPlayerColor.ToString() + " is the winner");
                thisGameState++;
            }
        }
    }

    private void GameIsOver(){
        GameManager.instance.ReturnToMainHub();
    }

}
