using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawn : MonoBehaviour
{
    public void SpawnPlayer(PlayerInput playerInput)
    {
        playerInput.transform.position = this.transform.position;
        playerInput.transform.rotation = this.transform.rotation;
    }
}
