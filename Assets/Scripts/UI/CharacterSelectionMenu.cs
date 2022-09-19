using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class CharacterSelectionMenu : MonoBehaviour{
    [SerializeField] public GameObject characterSelectionMenuUI;
    [SerializeField] public CharacterStatsScriptableObject[] characterList;
    [SerializeField] public CharacterStatsScriptableObject displayedCharacter;
    [SerializeField] public CharacterStatsScriptableObject selectedCharacter;
    [SerializeField] private Image characterSprite;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] public Button firstSelected;

    private void Start() {
        displayedCharacter = characterList[0];
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
        displayedCharacter = characterList[1];
        UpdateCharacter();
    }

    public void PreviousCharacter(){
        displayedCharacter = characterList[0];
        UpdateCharacter();
    }

    private void UpdateCharacter(){
        characterSprite.sprite = displayedCharacter.sprite[0];
        characterName.text = displayedCharacter.characterName.ToString();
    }

    public void ConfirmCharacter(){
        selectedCharacter = displayedCharacter;
        characterSelectionMenuUI.SetActive(false);
        this.transform.parent.parent.GetComponent<CharacterSelection>().SpawnCharacter(selectedCharacter);
    }
}
