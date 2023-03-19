using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPanelsContainer;
    public List<PlayerUIPanel> playerUIPanels = new List<PlayerUIPanel>();

    private void Start()
    {
        GameManager.Instance.OnGameStateChanged += AdaptToGameState;
        GameManager.Instance.OnPlayerJoinedGame += PlayerJoinedGame;
        GameManager.Instance.OnPlayerLeftGame += PlayerLeftGame;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameStateChanged -= AdaptToGameState;
        GameManager.Instance.OnPlayerJoinedGame -= PlayerJoinedGame;
        GameManager.Instance.OnPlayerLeftGame -= PlayerLeftGame;
    }


    private void AdaptToGameState(GameState gameState)
    {
        playerPanelsContainer.SetActive(gameState != GameState.MainMenu);
    }

    private void PlayerJoinedGame(PlayerInput playerInput)
    {
        for(int i = 0; i < playerUIPanels.Count; i++)
        {
            if(playerUIPanels[i].player == null)
            {
                playerUIPanels[i].AssignPlayer(playerInput);
                return;
            }
        }
    }

    private void PlayerLeftGame(PlayerInput playerInput)
    {
        playerUIPanels[playerInput.playerIndex].UnassignPlayer();
    }
}
