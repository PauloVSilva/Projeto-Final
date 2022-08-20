using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour{
    public GameObject[] playerUIPanels;
    public GameObject[] joinMessages;

    private void Start(){
        GameManager.instance.PlayerJoinedGame += PlayerJoinedGame;
        GameManager.instance.PlayerLeftGame += PlayerLeftGame;
    }

    void PlayerJoinedGame(PlayerInput playerInput){
        ShowUIPanel(playerInput);
    }

    void PlayerLeftGame(PlayerInput playerInput){
        HideUIPanel(playerInput);
    }

    void ShowUIPanel(PlayerInput playerInput){
        playerUIPanels[playerInput.playerIndex].SetActive(true);
        playerUIPanels[playerInput.playerIndex].GetComponent<PlayerUIPanel>().AssignPlayer(playerInput.playerIndex);
        joinMessages[playerInput.playerIndex].SetActive(false);
    }

    void HideUIPanel(PlayerInput playerInput){
        playerUIPanels[playerInput.playerIndex].SetActive(false);
        joinMessages[playerInput.playerIndex].SetActive(true);
    }
}
