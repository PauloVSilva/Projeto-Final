using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractableToy : MonoBehaviour, InteractorInterface{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private MiniGameScriptableObject miniGame;
    [SerializeField] private List<MiniGameGoalScriptableObject> miniGameGoalsList = new List<MiniGameGoalScriptableObject>();
    [SerializeField] private string toyName;

    private void Awake(){
        toyName = miniGame.miniGameName;
        toyPrompt = "Play " + toyName + "!";
        miniGameGoalsList = miniGame.miniGamesGoalsAvaliable.ToList();
    }

    private void Start(){
        interactionPromptUI.SetPrompt(toyPrompt);
    }

    private void Update(){
        transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));
    }


    //REQUIRED BY INTERACTOR INTERFACE
    [SerializeField] private string toyPrompt;
    [SerializeField] private InteractionPromptUI interactionPromptUI;
    [SerializeField] public string PromptString => toyPrompt; //property that returns string
    [SerializeField] public InteractionPromptUI PromptUI => interactionPromptUI; //property that returns InteractionPromptUI
    
    public bool Interact(Interactor interactor){
        if(GameManager.instance.playerList.Count > 1){
            MiniGameOptionsMenu.instance.MenuOpened(interactor.GetComponentInParent<PlayerInput>(), miniGame);
        }
        else{
            Debug.Log("Game requires at least 2 players");
        }
        return true;
    }
}
