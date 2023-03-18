using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainHubManager : LevelManager
{
    protected override void InitializeLevel()
    {
        GameManager.Instance.UpdateGameState(GameState.Hub);
    }
}
