using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour{
    public static MainMenu instance = null;
    [SerializeField] private Button firstSelected;

    private void Awake(){
        if(instance == null){
            instance = this;
        }
        else if(instance != null){
            Destroy(gameObject);
        }
    }

    public void Initialize(){
        firstSelected.Select();
    }
}
