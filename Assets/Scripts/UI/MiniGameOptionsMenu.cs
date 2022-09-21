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
    [SerializeField] private List<MiniGameGoalScriptableObject> miniGameGoalsList;
    [SerializeField] private MiniGameGoalScriptableObject displayedMiniGameGoal;

    [SerializeField] private TextMeshProUGUI miniGameName;

    [SerializeField] private Image goalSprite;
    [SerializeField] private TextMeshProUGUI goalName;
    [SerializeField] private TextMeshProUGUI goalDescription;
    [SerializeField] private TextMeshProUGUI goalKeyword;
    [SerializeField] private TextMeshProUGUI goalAmount;

    [SerializeField] private int miniGamegoalAmount;
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

    public void SetMiniGameName(string _name){
        miniGameName.text = _name;
    }

    public void SetMiniGameGoalsList(List<MiniGameGoalScriptableObject> _miniGameGoalsList){
        miniGameIndex = 0;
        miniGamegoalAmount = 0;
        miniGameGoalsList = _miniGameGoalsList.ToList();
        displayedMiniGameGoal = miniGameGoalsList[miniGameIndex];
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
        miniGamegoalAmount = 0;
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
        miniGamegoalAmount = 0;
        UpdateMenu();
    }

    public void IncreaseGoalAmount(){
        miniGamegoalAmount = miniGamegoalAmount + 1 * displayedMiniGameGoal.goalMultiplier;
        UpdateMenu();
    }

    public void DecreaseGoalAmount(){
        miniGamegoalAmount = miniGamegoalAmount - 1 * displayedMiniGameGoal.goalMultiplier;
        if(miniGamegoalAmount < 0){
            miniGamegoalAmount = 0;
        }
        UpdateMenu();
    }

    public void UpdateMenu(){
        goalSprite.sprite = displayedMiniGameGoal.goalSprite;
        goalName.text = displayedMiniGameGoal.goalName.ToString();
        goalDescription.text = displayedMiniGameGoal.goalDescription.ToString();
        goalKeyword.text = displayedMiniGameGoal.goalKeyword.ToString();
        goalAmount.text = miniGamegoalAmount.ToString();
    }

    public void ConfirmSettings(){
        Debug.Log("Going to minigame");
    }

}
