using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractableToy : MonoBehaviour, IInteractor
{
    [SerializeField] private float _minPlayersRequired;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private MiniGameScriptableObject miniGame;
    [SerializeField] private List<MiniGameGoalScriptableObject> miniGameGoalsList = new List<MiniGameGoalScriptableObject>();
    [SerializeField] private string toyName;

    public float MinPlayersRequired => _minPlayersRequired;

    private void Awake()
    {
        toyName = miniGame.miniGameName;
        toyPrompt = "Play " + toyName;
        miniGameGoalsList = miniGame.miniGamesGoalsAvaliable.ToList();
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));
    }


    //REQUIRED BY INTERACTOR INTERFACE
    [SerializeField] private string toyPrompt;
    [SerializeField] public string PromptString => toyPrompt; //property that returns string
    
    public bool Interact(Interactor interactor)
    {
        if(GameManager.Instance.playerList.Count >= _minPlayersRequired)
        {
            //MiniGameOptionsMenu.instance.MenuOpened(interactor.characterManager.playerInput, miniGame);
            CanvasManager.Instance.OpenMenu(Menu.MiniGameSetupMenu, interactor.characterManager.playerInput);
            CanvasManager.Instance.currentMenu.TryGetComponent(out MiniGameOptionsMenu miniGameOptionsMenu);
            miniGameOptionsMenu.SetUpMenu(miniGame);
        }
        else
        {
            Debug.Log("Game requires a friend ^-^");
        }
        return true;
    }
}
