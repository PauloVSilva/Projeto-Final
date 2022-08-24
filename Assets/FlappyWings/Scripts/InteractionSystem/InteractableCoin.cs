using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableCoin : MonoBehaviour, InteractorInterface{
    [SerializeField] private string _prompt;
    public string InteractionPromp => _prompt;

    public bool Interact (Interactor interactor){
        Debug.Log("Interacting with coin");
        return true;
    }
}
