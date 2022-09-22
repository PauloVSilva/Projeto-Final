using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class MiniGameOptionsMenu : MonoBehaviour{
    public static MiniGameOptionsMenu instance;
    [SerializeField] private GameObject miniGameOptionsMenuUI;
    [SerializeField] private MiniGameScriptableObject miniGame;
    [SerializeField] private List<MiniGameGoalScriptableObject> miniGameGoalsList;
    [SerializeField] private MiniGameGoalScriptableObject displayedMiniGameGoal;

    [SerializeField] private TextMeshProUGUI miniGameName;

    [SerializeField] private Image goalSprite;
    [SerializeField] private TextMeshProUGUI goalName;
    [SerializeField] private TextMeshProUGUI goalDescription;
    [SerializeField] private TextMeshProUGUI goalKeyword;
    [SerializeField] private TextMeshProUGUI goalAmount;

    [SerializeField] private int miniGameGoalAmount;
    [SerializeField] private int miniGameIndex;

    [SerializeField] private Image mapSprite;
    [SerializeField] private TextMeshProUGUI mapName; 
    [SerializeField] private TextMeshProUGUI mapDescription;
    [SerializeField] private Button firstSelected;

    private void Awake(){
        if(instance == null){
            instance = this;
        }
        else if(instance != null){
            Destroy(gameObject);
        }
    }

    public void SetMiniGame(MiniGameScriptableObject _miniGame){
        miniGameIndex = 0;

        miniGameName.text = _miniGame.miniGameName;
        miniGameGoalsList = _miniGame.miniGamesGoalsAvaliable.ToList();

        displayedMiniGameGoal = miniGameGoalsList[miniGameIndex];
        miniGameGoalAmount = 1 * displayedMiniGameGoal.goalMultiplier;

        firstSelected.Select();
        UpdateMenu();
    }

    public void NextGoal(){
        if(miniGameIndex < miniGameGoalsList.Count - 1){
            miniGameIndex++;
        }
        else{
            miniGameIndex = 0;
        }
        displayedMiniGameGoal = miniGameGoalsList[miniGameIndex];
        miniGameGoalAmount = displayedMiniGameGoal.goalMultiplier;
        UpdateMenu();
    }

    public void PreviousGoal(){
        if(miniGameIndex > 0){
            miniGameIndex--;
        }
        else{
            miniGameIndex = miniGameGoalsList.Count - 1;
        }
        displayedMiniGameGoal = miniGameGoalsList[miniGameIndex];
        miniGameGoalAmount = displayedMiniGameGoal.goalMultiplier;
        UpdateMenu();
    }

    public void IncreaseGoalAmount(){
        miniGameGoalAmount += displayedMiniGameGoal.goalMultiplier;
        UpdateMenu();
    }

    public void DecreaseGoalAmount(){
        miniGameGoalAmount -= displayedMiniGameGoal.goalMultiplier;
        if(miniGameGoalAmount < displayedMiniGameGoal.goalMultiplier){
            miniGameGoalAmount = displayedMiniGameGoal.goalMultiplier;
        }
        UpdateMenu();
    }

    public void UpdateMenu(){
        goalSprite.sprite = displayedMiniGameGoal.goalSprite;
        goalName.text = displayedMiniGameGoal.goalName.ToString();
        goalDescription.text = displayedMiniGameGoal.goalDescription.ToString();
        goalKeyword.text = displayedMiniGameGoal.goalKeyword.ToString();
        goalAmount.text = miniGameGoalAmount.ToString();
    }

    public void ConfirmSettings(){
        Debug.Log("Going to " + miniGameName.text);
        GameManager.instance.LoadMiniGame(miniGameName.text);
    }

    public MiniGameGoalScriptableObject GetMiniGameGoal(){
        return displayedMiniGameGoal;
    }

    public int GetMiniGameGoalAmount(){
        return miniGameGoalAmount;
    }

}
