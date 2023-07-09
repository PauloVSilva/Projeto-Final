using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPanelsContainer;
    public List<PlayerUIPanel> playerUIPanels = new List<PlayerUIPanel>();

    public bool forceDisable;
    public InputAction disable;

    private void Start()
    {
        disable.performed += context => Disable();
        disable.Enable();

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


    private void Disable()
    {
        forceDisable = !forceDisable;

        playerPanelsContainer.SetActive(forceDisable);
    }


    private void AdaptToGameState(GameState gameState)
    {
        if (forceDisable) return;

        playerPanelsContainer.SetActive(gameState != GameState.MainMenu);

        switch (gameState)
        {
            case GameState.MiniGame:
                DisableUnassigned();
                break;
            case GameState.Hub:
                EnableAll();
                break;
        }
    }


    private void EnableAll()
    {
        foreach (PlayerUIPanel playerUIPanel in playerUIPanels)
        {
            playerUIPanel.gameObject.SetActive(true);
        }
    }

    private void DisableUnassigned()
    {
        foreach (PlayerUIPanel playerUIPanel in playerUIPanels)
        {
            if(playerUIPanel.player == null) playerUIPanel.gameObject.SetActive(false);
        }
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
