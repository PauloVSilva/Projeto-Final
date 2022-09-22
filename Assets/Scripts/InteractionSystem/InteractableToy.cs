using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractableToy : MonoBehaviour, InteractorInterface{
    [SerializeField] private MiniGameScriptableObject miniGame;
    [SerializeField] private List<MiniGameGoalScriptableObject> miniGameGoalsList = new List<MiniGameGoalScriptableObject>();
    [SerializeField] private string _name;
    [SerializeField] private string _prompt;
    [SerializeField] public string InteractionPromp => _prompt;

    public void Awake(){
        _name = miniGame.miniGameName;
        _prompt = "Play " + _name + "!";
        miniGameGoalsList = miniGame.miniGamesGoalsAvaliable.ToList();
    }

    public bool Interact (Interactor interactor){
        if(GameManager.instance.playerList.Count > 1){
            CanvasManager.instance.SwitchMenu(Menu.MiniGameSetupMenu);
            MiniGameOptionsMenu.instance.SetMiniGame(miniGame);
        }
        else{
            Debug.Log("Game requires at least 2 players");
        }
        return true;
    }
}
