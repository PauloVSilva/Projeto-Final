using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainHubManager : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.UpdateGameState(GameState.Hub);
    }
}
