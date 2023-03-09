using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class PlayerUIManager : MonoBehaviour{
    public List<PlayerUIPanel> playerUIPanels = new List<PlayerUIPanel>();

    private void OnEnable(){
        GameManager.Instance.OnPlayerJoinedGame += PlayerJoinedGame;
        GameManager.Instance.OnPlayerLeftGame += PlayerLeftGame;
    }

    private void OnDisable(){
        GameManager.Instance.OnPlayerJoinedGame -= PlayerJoinedGame;
        GameManager.Instance.OnPlayerLeftGame -= PlayerLeftGame;
    }

    void PlayerJoinedGame(PlayerInput playerInput){
        for(int i = 0; i < playerUIPanels.Count; i++){
            if(playerUIPanels[i].player == null){
                playerUIPanels[i].AssignPlayer(playerInput);
                return;
            }
        }
    }

    void PlayerLeftGame(PlayerInput playerInput){
        playerUIPanels[playerInput.playerIndex].UnassignPlayer();
    }
}
