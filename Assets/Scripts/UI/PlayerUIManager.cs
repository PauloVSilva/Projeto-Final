using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class PlayerUIManager : MonoBehaviour{
    public GameObject[] playerUIPanels;

    private void Start(){
        if(GameManager.instance.playerList.Count > 0){
            for (int i = 0; i < GameManager.instance.playerList.Count; i++){
                if(playerUIPanels[i].GetComponent<PlayerUIPanel>().player != null){
                    playerUIPanels[i].GetComponent<PlayerUIPanel>().UnassignPlayer();
                }
            }
            for (int i = 0; i < GameManager.instance.playerList.Count; i++){
                if(playerUIPanels[i].GetComponent<PlayerUIPanel>().player == null){
                    playerUIPanels[i].GetComponent<PlayerUIPanel>().AssignPlayer(GameManager.instance.playerList[i]);
                }
            }
        }
    }

    private void OnEnable(){
        GameManager.instance.OnPlayerJoinedGame += PlayerJoinedGame;
        GameManager.instance.OnPlayerLeftGame += PlayerLeftGame;
    }

    private void OnDisable(){
        GameManager.instance.OnPlayerJoinedGame -= PlayerJoinedGame;
        GameManager.instance.OnPlayerLeftGame -= PlayerLeftGame;
    }

    void PlayerJoinedGame(PlayerInput playerInput){
        //Debug.Log("PlayerJoinedGame");
        for(int i = 0; i < playerUIPanels.Length; i++){
            if(playerUIPanels[i].GetComponent<PlayerUIPanel>().player == null){
                playerUIPanels[i].GetComponent<PlayerUIPanel>().AssignPlayer(playerInput);
                return;
            }
        }
    }

    void PlayerLeftGame(PlayerInput playerInput){
        //Debug.Log("PlayerLeftGame");
        playerUIPanels[playerInput.playerIndex].GetComponent<PlayerUIPanel>().UnassignPlayer();
        //ReorderPanels();
    }

    void ReorderPanels(){
        //Debug.Log("ReorderingPanels");
        for (int i = 0; i < GameManager.instance.playerList.Count; i++){
            playerUIPanels[i].GetComponent<PlayerUIPanel>().AssignPlayer(GameManager.instance.playerList[i]);
        }
        if(playerUIPanels.Length - GameManager.instance.playerList.Count > 0){
            for (int i = GameManager.instance.playerList.Count; i < playerUIPanels.Length; i++){
                playerUIPanels[i].GetComponent<PlayerUIPanel>().UnassignPlayer();
            }
        }
    }
}
