using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class MiniGameOptionsMenu : MenuController
{
    private List<MiniGameGoalScriptableObject> miniGameGoalsList;
    private MiniGameGoalScriptableObject displayedMiniGameGoal;
    private int miniGameGoalAmount;
    private int miniGameIndex;

    [Header("Goal")]
    [Space(5)]
    [SerializeField] private Image gameIcon;

    [SerializeField] private Image goalSprite;
    [SerializeField] private TextMeshProUGUI goalName;
    [SerializeField] private TextMeshProUGUI goalDescription;
    [SerializeField] private TextMeshProUGUI goalKeyword;
    [SerializeField] private TextMeshProUGUI goalAmount;

    [Header("Buttons")]
    [Space(5)]
    [SerializeField] private Button nextGoalButton;
    [SerializeField] private Button previousGoalButton;


    public void SetUpMenu(MiniGameScriptableObject _miniGame)
    {
        SetMiniGame(_miniGame);
        GameManager.Instance.UpdateGameState(GameState.Paused);
    }

    private void SetMiniGame(MiniGameScriptableObject _miniGame)
    {
        InitializeMenuVariables(_miniGame);
    }

    private void InitializeMenuVariables(MiniGameScriptableObject _miniGame)
    {
        miniGameIndex = 0;

        menuName.text = _miniGame.miniGameName;
        gameIcon.sprite = _miniGame.miniGameSprite;

        miniGameGoalsList = _miniGame.miniGamesGoalsAvaliable.ToList();

        nextGoalButton.gameObject.SetActive(miniGameGoalsList.Count != 1);
        previousGoalButton.gameObject.SetActive(miniGameGoalsList.Count != 1);

        displayedMiniGameGoal = miniGameGoalsList[miniGameIndex];
        miniGameGoalAmount = 1 * displayedMiniGameGoal.goalMultiplier;

        UpdateMenu();
    }

    private void UpdateMenu()
    {
        goalSprite.sprite = displayedMiniGameGoal.goalSprite;
        goalName.text = displayedMiniGameGoal.goalName.ToString();
        //goalDescription.text = displayedMiniGameGoal.goalDescription.ToString();
        goalDescription.text = MessageManager.Instance.StringEditor(displayedMiniGameGoal.goalDescription.ToString(), "$VALUE", miniGameGoalAmount.ToString());
        goalKeyword.text = displayedMiniGameGoal.goalKeyword.ToString();
        goalAmount.text = miniGameGoalAmount.ToString();
    }

    
    #region BUTTONS
    public void NextGoal()
    {
        if(miniGameIndex < miniGameGoalsList.Count - 1)
        {
            miniGameIndex++;
        }
        else
        {
            miniGameIndex = 0;
        }
        displayedMiniGameGoal = miniGameGoalsList[miniGameIndex];
        miniGameGoalAmount = displayedMiniGameGoal.goalMultiplier;
        UpdateMenu();
    }

    public void PreviousGoal()
    {
        if(miniGameIndex > 0)
        {
            miniGameIndex--;
        }
        else
        {
            miniGameIndex = miniGameGoalsList.Count - 1;
        }
        displayedMiniGameGoal = miniGameGoalsList[miniGameIndex];
        miniGameGoalAmount = displayedMiniGameGoal.goalMultiplier;
        UpdateMenu();
    }

    public void IncreaseGoalAmount()
    {
        miniGameGoalAmount += displayedMiniGameGoal.goalMultiplier;
        UpdateMenu();
    }

    public void DecreaseGoalAmount()
    {
        miniGameGoalAmount -= displayedMiniGameGoal.goalMultiplier;
        if(miniGameGoalAmount < displayedMiniGameGoal.goalMultiplier){
            miniGameGoalAmount = displayedMiniGameGoal.goalMultiplier;
        }
        UpdateMenu();
    }

    public void ConfirmSettings()
    {
        Back();

        MiniGameManager.Instance.SetMiniGame(displayedMiniGameGoal);
        MiniGameManager.Instance.SetGoalAmount(miniGameGoalAmount);
        MiniGameManager.Instance.SetGoalDescription(goalDescription.text);

        LevelLoader.Instance.LoadLevel(menuName.text);
    }

    public void CancelSelection()
    {
        Back();
    }
    #endregion BUTTONS
}
