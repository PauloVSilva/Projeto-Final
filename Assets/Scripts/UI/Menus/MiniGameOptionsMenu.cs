using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class MiniGameOptionsMenu : MenuBase{
    public static MiniGameOptionsMenu instance;
    private List<MiniGameGoalScriptableObject> miniGameGoalsList;
    private MiniGameGoalScriptableObject displayedMiniGameGoal;
    private int miniGameGoalAmount;
    private int miniGameIndex;
    [SerializeField] private Image goalSprite;
    [SerializeField] private TextMeshProUGUI goalName;
    [SerializeField] private TextMeshProUGUI goalDescription;
    [SerializeField] private TextMeshProUGUI goalKeyword;
    [SerializeField] private TextMeshProUGUI goalAmount;


    private void Awake(){
        if(instance == null){
            instance = this;
        }
        else if(instance != null){
            Destroy(gameObject);
        }
    }

    private void Start(){
        base.CreateFooterButtons();
    }

    public void MenuOpened(PlayerInput _playerInput, MiniGameScriptableObject _miniGame){
        base.AssignPlayerToMenu(_playerInput);
        InitializeMenu(_miniGame);
    }

    public void InitializeMenu(MiniGameScriptableObject _miniGame){
        miniGameIndex = 0;

        menuName.text = _miniGame.miniGameName;
        miniGameGoalsList = _miniGame.miniGamesGoalsAvaliable.ToList();

        displayedMiniGameGoal = miniGameGoalsList[miniGameIndex];
        miniGameGoalAmount = 1 * displayedMiniGameGoal.goalMultiplier;

        UpdateMenu();

        base.SetUpCanvasButtons();
        CanvasManager.instance.OpenMenu(Menu.MiniGameSetupMenu);
        StartCoroutine(PauseMenu.instance.PauseDelay());
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
        //goalDescription.text = displayedMiniGameGoal.goalDescription.ToString();
        goalDescription.text = MessageManager.instance.StringEditor(displayedMiniGameGoal.goalDescription.ToString(), "$VALUE", miniGameGoalAmount.ToString());
        goalKeyword.text = displayedMiniGameGoal.goalKeyword.ToString();
        goalAmount.text = miniGameGoalAmount.ToString();
    }

    public void ConfirmSettings(){
        CanvasManager.instance.CloseMenu();
        PauseMenu.instance.Resume();
        LevelLoader.instance.LoadLevel(menuName.text);
    }

    public void CancelSelection(){
        CanvasManager.instance.CloseMenu();
        PauseMenu.instance.Resume();
    }

    public MiniGameGoalScriptableObject GetMiniGameGoal(){
        return displayedMiniGameGoal;
    }

    public string GetMiniGameGoalDescription(){
        return goalDescription.text;
    }

    public int GetMiniGameGoalAmount(){
        return miniGameGoalAmount;
    }

}
