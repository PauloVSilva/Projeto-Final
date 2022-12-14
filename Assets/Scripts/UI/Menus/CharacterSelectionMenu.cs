using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using TMPro;

public class CharacterSelectionMenu : MenuController{
    private CharacterManager characterManager;
    private CharacterStatsScriptableObject displayedCharacter;
    private int index;
    private string greetMessage;
    [SerializeField] private List<CharacterStatsScriptableObject> characterList = new List<CharacterStatsScriptableObject>();
    [SerializeField] private Image characterSprite;
    [SerializeField] private TextMeshProUGUI characterName;

    protected override void Start(){
        base.Start();
        ListenToPlayerJoined();
    }

    private void ListenToPlayerJoined(){
        GameManager.instance.OnPlayerJoinedGame += MenuOpened;
    }

    public void MenuOpened(PlayerInput _playerInput){
        base.AssignPlayerToMenu(_playerInput);
        InitializeMenu();
    }

    private void InitializeMenu(){
        greetMessage = MessageManager.instance.GetGreetMessage(playerInput.playerIndex + 1);
        playerControllingMenu.text = greetMessage;
        
        index = 0;
        characterManager = playerInput.GetComponent<CharacterManager>();
        displayedCharacter = characterList[index];

        UpdateCharacter();

        CanvasManager.instance.OpenMenu(this.menu);
        StartCoroutine(PauseMenu.instance.FreezeGameDelay());
    }

    private void UpdateCharacter(){
        characterSprite.sprite = displayedCharacter.sprite[0];
        characterName.text = displayedCharacter.characterName.ToString();
    }


    #region BUTTONS
    public void NextCharacter(){
        if(index < characterList.Count - 1){
            index++;
        }
        else{
            index = 0;
        }
        displayedCharacter = characterList[index];
        UpdateCharacter();
    }

    public void PreviousCharacter(){
        if(index > 0){
            index--;
        }
        else{
            index = characterList.Count - 1;
        }
        displayedCharacter = characterList[index];
        UpdateCharacter();
    }

    public void ConfirmCharacter(){
        CharacterStatsScriptableObject selectedCharacter = displayedCharacter;
        characterManager.SpawnCharacter(selectedCharacter);
        base.Back();
    }
    #endregion BUTTONS
}
