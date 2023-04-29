using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class CharacterSelectionMenu : MenuController
{
    private AudioSource audioSource;
    public AudioClip kidsCheering;


    private CharacterStatsScriptableObject displayedCharacter;
    private int index;
    private string greetMessage;
    [SerializeField] private List<CharacterStatsScriptableObject> characterList = new List<CharacterStatsScriptableObject>();
    [SerializeField] private Image characterSprite;
    [SerializeField] private TextMeshProUGUI characterName;

    protected override void Start()
    {
        base.Start();

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;

        ListenToPlayerJoined();
    }

    private void ListenToPlayerJoined()
    {
        GameManager.Instance.OnPlayerJoinedGame += NewPlayerJoined;
    }

    public void NewPlayerJoined(PlayerInput _playerInput)
    {
        CanvasManager.Instance.OpenMenu(menu, _playerInput);

        audioSource.PlayOneShot(kidsCheering);

        InitializeMenuVariables();

        GameManager.Instance.UpdateGameState(GameState.Paused);
    }

    private void InitializeMenuVariables()
    {
        greetMessage = MessageManager.Instance.GetGreetMessage(playerInput.playerIndex + 1);
        playerControllingMenu.text = greetMessage;
        
        index = 0;
        displayedCharacter = characterList[index];

        UpdateCharacter();
    }

    private void UpdateCharacter()
    {
        characterSprite.sprite = displayedCharacter.characterSkin[0].characterSprite;
        characterName.text = displayedCharacter.characterSkin[0].characterName;
    }


    #region BUTTONS
    public void NextCharacter()
    {
        index = (index + 1) % characterList.Count;

        displayedCharacter = characterList[index];
        UpdateCharacter();
    }

    public void PreviousCharacter()
    {
        index--;
        if(index < 0) index = characterList.Count - 1;

        displayedCharacter = characterList[index];
        UpdateCharacter();
    }

    public void ConfirmCharacter()
    {
        CharacterStatsScriptableObject selectedCharacter = displayedCharacter;

        playerInput.TryGetComponent(out CharacterManager _characterManager);

        _characterManager.SpawnCharacter(selectedCharacter);
        
        Back();
    }
    #endregion BUTTONS
}
