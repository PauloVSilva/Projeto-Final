using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractableToy : Item, InteractorInterface{
    [SerializeField] private MiniGameScriptableObject miniGame;
    private List<MiniGameGoalScriptableObject> miniGameGoalsList = new List<MiniGameGoalScriptableObject>();
    private string toyName;
    private string toyPrompt;
    [SerializeField] private InteractionPromptUI interactionPromptUI;
    public string PromptString => toyPrompt;
    public InteractionPromptUI PromptUI => interactionPromptUI;

    protected override void Awake(){
        toyName = miniGame.miniGameName;
        toyPrompt = "Play " + toyName + "!";
        miniGameGoalsList = miniGame.miniGamesGoalsAvaliable.ToList();
    }

    protected void Start(){
        interactionPromptUI.SetPrompt(toyPrompt);
    }

    public bool Interact(Interactor interactor){
        if(GameManager.instance.playerList.Count > 1){
            CanvasManager.instance.SwitchMenu(Menu.MiniGameSetupMenu);
            MiniGameOptionsMenu.instance.SetMiniGame(miniGame);
        }
        else{
            Debug.Log("Game requires at least 2 players");
        }
        return true;
    }

    protected override void MaxAgeReached(){
        Debug.Log("Somehow, an interactable toy aged. Better check if anything is broken.");
    }
}
