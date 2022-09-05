using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractableToy : MonoBehaviour, InteractorInterface{
    [SerializeField] private string _prompt;
    [SerializeField] private string _name;
    public string InteractionPromp => _prompt;

    public bool Interact (Interactor interactor){
        //Debug.Log("Interacting with " + InteractionPromp);
        SceneManager.LoadScene(_name);
        return true;
    }
}
