using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class PlayerUIManager : MonoBehaviour{
    public GameObject[] playerUIPanels;
    //public GameObject[] joinMessages;

    private void Awake(){
        if(GameManager.instance.playerList.Count > 0){
            for (int i = 0; i < playerUIPanels.Length; i++){
                playerUIPanels[i].GetComponent<PlayerUIPanel>().UnassignPlayer();
            }
            for (int i = 0; i < GameManager.instance.playerList.Count; i++){
                playerUIPanels[i].GetComponent<PlayerUIPanel>().AssignPlayer(i);
            }
        }
    }

    private void OnEnable(){
        GameManager.instance.PlayerJoinedGame += PlayerJoinedGame;
        GameManager.instance.PlayerLeftGame += PlayerLeftGame;
    }

    private void OnDisable(){
        GameManager.instance.PlayerJoinedGame -= PlayerJoinedGame;
        GameManager.instance.PlayerLeftGame -= PlayerLeftGame;
    }

    void PlayerJoinedGame(PlayerInput playerInput){
        //Debug.Log("PlayerJoinedGame");
        playerUIPanels[playerInput.playerIndex].GetComponent<PlayerUIPanel>().AssignPlayer(playerInput.playerIndex);
    }

    void PlayerLeftGame(PlayerInput playerInput){
        //Debug.Log("PlayerLeftGame");
        playerUIPanels[playerInput.playerIndex].GetComponent<PlayerUIPanel>().UnassignPlayer();
        ReorderPanels();
    }

    void ReorderPanels(){
        //Debug.Log("ReorderingPanels");
        for (int i = 0; i < GameManager.instance.playerList.Count; i++){
            playerUIPanels[i].GetComponent<PlayerUIPanel>().AssignPlayer(i);
        }
        if(playerUIPanels.Length - GameManager.instance.playerList.Count > 0){
            for (int i = GameManager.instance.playerList.Count; i < playerUIPanels.Length; i++){
                playerUIPanels[i].GetComponent<PlayerUIPanel>().UnassignPlayer();
            }
        }
    }

    void ShowUIPanel(PlayerInput playerInput){
        //playerUIPanels[playerInput.playerIndex].SetActive(true);
        playerUIPanels[playerInput.playerIndex].GetComponent<PlayerUIPanel>().AssignPlayer(playerInput.playerIndex);
        //joinMessages[playerInput.playerIndex].SetActive(false);
    }

    void HideUIPanel(PlayerInput playerInput){
        playerUIPanels[playerInput.playerIndex].GetComponent<PlayerUIPanel>().UnassignPlayer();
        //playerUIPanels[playerInput.playerIndex].SetActive(false);
        //joinMessages[playerInput.playerIndex].SetActive(true);
    }
}
