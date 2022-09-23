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
    [SerializeField] private CharacterSelection characterSelection;
    [SerializeField] private List<CharacterStatsScriptableObject> characterList = new List<CharacterStatsScriptableObject>();
    [SerializeField] private CharacterStatsScriptableObject displayedCharacter;
    [SerializeField] private int index;
    [SerializeField] private Image characterSprite;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private string greetMessage;

    private void Start(){
        greetMessage = "Welcome player $index!";
        ListenToPlayerJoined();
    }

    public string StringEditor(string originalMessage, string oldPart, string newPart){
        string newMessage = originalMessage;
        if(newMessage.Contains(oldPart)){
            Debug.Log("old part found");
            newMessage = newMessage.Replace(oldPart, newPart);
            return newMessage;
        }
        Debug.Log("old part not found");
        return originalMessage;
    }

    private void ListenToPlayerJoined(){
        GameManager.instance.OnPlayerJoinedGame += PlayerJoined;
    }

    private void PlayerJoined(PlayerInput _playerInput){
        firstSelected.Select();
        playerInput = _playerInput;
        inputSystemUIInputModule.actionsAsset = playerInput.actions;
        //_playerInput.InputSystemUIInputModule = inputSystemUIInputModule;
        playerControllingMenu.text = StringEditor(greetMessage, "$index", (_playerInput.playerIndex + 1).ToString());
        MenuOpened();
        CanvasManager.instance.SwitchMenu(Menu.CharacterSelectionMenu);
    }

    private void MenuOpened(){
        index = 0;
        characterSelection = playerInput.GetComponent<CharacterSelection>();
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
        characterSelection.SpawnCharacter(selectedCharacter);
    }

    public void CancelSelection(){
        GameManager.instance.UnregisterPlayer(playerInput);
    }

}
