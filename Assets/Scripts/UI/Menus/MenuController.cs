using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using TMPro;
using UnityEngine.InputSystem.DualShock;
using DG.Tweening;

public abstract class MenuController : MonoBehaviour
{
    public Menu menu;
    [SerializeField] private GameObject menuContainer;
    [SerializeField] private GameObject footer;
    [SerializeField] private InputSystemUIInputModule inputSystemUIInputModule;
    [SerializeField] private Button firstSelected;

    private List<CanvasButtonDisplay> canvasButtonsList = new List<CanvasButtonDisplay>();
    private List<GameObject> footerButtons = new List<GameObject>();

    [SerializeField] protected PlayerInput playerInput;
    [SerializeField] protected Device playerDevice;

    [SerializeField] protected ButtonType[] buttonTypes;
    [SerializeField] protected TextMeshProUGUI menuName;
    [SerializeField] protected TextMeshProUGUI playerControllingMenu;
    [SerializeField] protected TabGroup tabGroup;

    protected virtual void Start()
    {
        CreateEmptyFooterButtons();
    }


    public void Open()
    {
        Open(null);
    }

    public void Open(PlayerInput _playerInput)
    {
        if(_playerInput != null) AssignPlayerToMenu(_playerInput);

        StartCoroutine(OpenDelay());
        IEnumerator OpenDelay()
        {
            menuContainer.SetActive(true);

            yield return new WaitForEndOfFrame();

            GainControl();

            SetUpFooterButtons();
            SetUpTabs();
            SubscribeToInputActions();
        }
    }

    public void Close()
    {
        StartCoroutine(CloseDelay());
        IEnumerator CloseDelay()
        {
            yield return new WaitForEndOfFrame();

            UnsubscribeFromInputActions();
            menuContainer.SetActive(false);
        }
    }

    public void GainControl()
    {
        firstSelected.Select();
    }
    
    protected void Back()
    {
        //this method calls a method that calls Close()
        //pretty messy I know
        CanvasManager.Instance.CloseMenu();
    }

    private void CreateEmptyFooterButtons()
    {
        if(buttonTypes.Count() < 1) return;

        for(int i = 0; i < buttonTypes.Count(); i++)
        {
            for(int j = 0; j < CanvasManager.Instance.canvasButtonsList.Count(); j++)
            {
                if(buttonTypes[i] == CanvasManager.Instance.canvasButtonsList[j].buttonType)
                {
                    canvasButtonsList.Add(CanvasManager.Instance.canvasButtonsList[j]);
                    break;
                }
            }
            GameObject ButtonDisplay = Instantiate(CanvasManager.Instance.buttonDisplayPrefab);
            ButtonDisplay.transform.SetParent(footer.transform, false);
            footerButtons.Add(ButtonDisplay);
        }
    }

    private void SetUpFooterButtons()
    {
        if(playerInput == null) return;

            for (int i = 0; i < footerButtons.Count(); i++)
            {
                footerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = canvasButtonsList[i].buttonString;
                footerButtons[i].GetComponentInChildren<Image>().sprite = canvasButtonsList[i].buttonSprite[(int)playerDevice];
            }

    }

    private void SetUpTabs()
    {
        if (tabGroup == null || playerInput == null) return;

            for (int i = 0; i < tabGroup.buttonTypes.Count(); i++)
            {
                for (int j = 0; j < CanvasManager.Instance.canvasButtonsList.Count(); j++)
                {
                    if (tabGroup.buttonTypes[i] == CanvasManager.Instance.canvasButtonsList[j].buttonType)
                    {
                        tabGroup.tabLabels[i].sprite = CanvasManager.Instance.canvasButtonsList[j].buttonSprite[(int)playerDevice];
                        break;
                    }
                }
            }
    }

    private void SubscribeToInputActions()
    {
        if(playerInput == null) return;

        //InputActionAsset playerActions = playerInput.actions;

        playerInput.TryGetComponent(out CharacterManager _characterManager);
        _characterManager.playerInputHandler.DisableActions(true);

        for (int i = 0; i < buttonTypes.Count(); i++)
        {
            if(buttonTypes[i].ToString() == "Back")
                playerInput.actions["Back"].performed += PlayerPressedBackButton;

            if(tabGroup != null)
            {
                playerInput.actions["PreviousTab"].performed += PlayerPressedPreviousTabButton;
                playerInput.actions["NextTab"].performed += PlayerPressedNextTabButton;
            }
        }
    }

    private void UnsubscribeFromInputActions()
    {
        if(playerInput == null) return;

        playerInput.TryGetComponent(out CharacterManager _characterManager);

        _characterManager.playerInputHandler.DisableActions(false);

        playerInput.actions["Back"].performed -= PlayerPressedBackButton;
        playerInput.actions["PreviousTab"].performed -= PlayerPressedPreviousTabButton;
        playerInput.actions["NextTab"].performed -= PlayerPressedNextTabButton;
    }

    private void AssignPlayerToMenu(PlayerInput _playerInput)
    {
        playerInput = _playerInput;
        inputSystemUIInputModule.actionsAsset = playerInput.actions;

        playerInput.TryGetComponent(out CharacterManager characterManager);
        Debug.Log(characterManager.playerDevice);
        playerDevice = characterManager.playerDevice;

        //_playerInput.InputSystemUIInputModule = inputSystemUIInputModule;
        //playerInput.GetComponent<PlayerInputHandler>().OnPlayerPressedBackButton += PlayerPressedBackButton;
    }

    protected void PlayerPressedBackButton(InputAction.CallbackContext context)
    {
        if(CanvasManager.Instance.currentMenu == this)
            CanvasManager.Instance.CloseMenu();
    }

    protected void PlayerPressedPreviousTabButton(InputAction.CallbackContext context)
    {
        if(CanvasManager.Instance.currentMenu == this)
            tabGroup?.SelectPreviousTab();
    }

    protected void PlayerPressedNextTabButton(InputAction.CallbackContext context)
    {
        if(CanvasManager.Instance.currentMenu == this)
            tabGroup?.SelectNextTab();
    }
}
