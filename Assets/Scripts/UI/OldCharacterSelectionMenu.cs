using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class OldCharacterSelectionMenu : MonoBehaviour{
    [SerializeField] private GameObject characterSelectionMenuUI;
    [SerializeField] private CharacterSelection characterSelection;
    [SerializeField] private List<CharacterStatsScriptableObject> characterList = new List<CharacterStatsScriptableObject>();
    [SerializeField] private CharacterStatsScriptableObject displayedCharacter;
    [SerializeField] private int index;
    [SerializeField] private Image characterSprite;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private Button firstSelected;

    private void Awake(){
        index = 0;
    }

    private void Start() {
        displayedCharacter = characterList[index];
        UpdateCharacter();
        firstSelected.Select();
        characterSelectionMenuUI.SetActive(true);
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
        characterSelectionMenuUI.SetActive(false);
    }
}
