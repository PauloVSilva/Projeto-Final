using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class CharacterSelectionMenu : MonoBehaviour{
    [SerializeField] public GameObject characterSelectionMenuUI;
    [SerializeField] private List<CharacterStatsScriptableObject> characterList = new List<CharacterStatsScriptableObject>();
    [SerializeField] public CharacterStatsScriptableObject displayedCharacter;
    [SerializeField] public int index;
    [SerializeField] private Image characterSprite;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] public Button firstSelected;

    private void Start() {
        index = 0;
        displayedCharacter = characterList[index];
        UpdateCharacter();
        characterSelectionMenuUI.SetActive(true);
        firstSelected.Select();
    }

    private void OnEnable() {
        GameManager.instance.joinAction.Disable();
        GameManager.instance.leaveAction.Disable();
    }

    private void OnDisable() {
        GameManager.instance.joinAction.Enable();
        GameManager.instance.leaveAction.Enable();
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
        this.transform.parent.parent.GetComponent<CharacterSelection>().SpawnCharacter(displayedCharacter);
        characterSelectionMenuUI.SetActive(false);
    }
}
