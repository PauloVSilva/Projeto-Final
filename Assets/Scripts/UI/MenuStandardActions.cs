using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuStandardActions : MonoBehaviour{
    [SerializeField] private GameObject multiplayerEventSystem;
    private void OnEnable() {
        multiplayerEventSystem.SetActive(true);
        GameManager.instance.joinAction.Disable();
        GameManager.instance.leaveAction.Disable();
    }

    private void OnDisable(){
        multiplayerEventSystem.SetActive(false);
        GameManager.instance.joinAction.Enable();
        GameManager.instance.leaveAction.Enable();
    }
}
