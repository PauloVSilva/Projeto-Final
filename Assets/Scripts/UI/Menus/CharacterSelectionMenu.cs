using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using TMPro;

public class CharacterSelectionMenu : MonoBehaviour{
    //COMMON TO ALL MENUS
    [SerializeField] private Button firstSelected;
    [SerializeField] private TextMeshProUGUI menuName;

    //COMMON TO PLAYER-SPECIFIC MENU
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private InputSystemUIInputModule inputSystemUIInputModule;
    [SerializeField] private TextMeshProUGUI playerControllingMenu;

    //SPECIFIC TO THIS MENU
    [SerializeField] private CharacterManager characterManager;
    [SerializeField] private List<CharacterStatsScriptableObject> characterList = new List<CharacterStatsScriptableObject>();
    [SerializeField] private CharacterStatsScriptableObject displayedCharacter;
    [SerializeField] private int index;
    [SerializeField] private Image characterSprite;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private string greetMessage;

    private void Start(){
        ListenToPlayerJoined();
    }

    private void ListenToPlayerJoined(){
        GameManager.instance.OnPlayerJoinedGame += PlayerJoined;
    }

    private void PlayerJoined(PlayerInput _playerInput){
        firstSelected.Select();
        playerInput = _playerInput;
        inputSystemUIInputModule.actionsAsset = playerInput.actions;
        //_playerInput.InputSystemUIInputModule = inputSystemUIInputModule;
        greetMessage = MessageManager.instance.GetGreetMessage(_playerInput.playerIndex + 1);
        playerControllingMenu.text = greetMessage;
        MenuOpened();
        CanvasManager.instance.SwitchMenu(Menu.CharacterSelectionMenu);
    }

    private void MenuOpened(){
        index = 0;
        characterManager = playerInput.GetComponent<CharacterManager>();
        displayedCharacter = characterList[index];
        UpdateCharacter();
        firstSelected.Select();
    }

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

    private void UpdateCharacter(){
        characterSprite.sprite = displayedCharacter.sprite[0];
        characterName.text = displayedCharacter.characterName.ToString();
    }

    public void ConfirmCharacter(){
        CharacterStatsScriptableObject selectedCharacter = displayedCharacter;
        characterManager.SpawnCharacter(selectedCharacter);
    }

    public void CancelSelection(){
        GameManager.instance.UnregisterPlayer(playerInput);
    }

}
