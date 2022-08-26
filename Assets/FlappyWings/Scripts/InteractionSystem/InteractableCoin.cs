using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractableCoin : MonoBehaviour, InteractorInterface{
    [SerializeField] private string _prompt;
    public string InteractionPromp => _prompt;

    public bool Interact (Interactor interactor){
        Debug.Log("Interacting with " + InteractionPromp);
        SceneManager.LoadScene("FlappyWings");
        return true;
    }
}
