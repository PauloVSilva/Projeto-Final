using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum Menu { MainMenu, ControlsMenu, SettingsMenu, MiniGameSetupMenu, PauseMenu, CharacterSelectionMenu, CreditsMenu }
public enum Device { Keyboard, DualShock, XboxController }
public enum ButtonType { Back, Submit, Navigate, Interact, TabNavigation, PreviousTab, NextTab }
public enum Interactions { Hold, MultiTap, Press, SlowTap, Tap }

[System.Serializable]
public class CanvasButtonDisplay
{
    //public InputActionMap inputAction;
    public ButtonType buttonType;
    public string buttonString;
    public Sprite[] buttonSprite;

}

public class CanvasManager : Singleton<CanvasManager>
{
    [SerializeField] private List<MenuController> menuControllersList;
    [SerializeField] private List<MenuController> allActiveMenus = new List<MenuController>();
    [SerializeField] public MenuController currentMenu;
    [SerializeField] public List<CanvasButtonDisplay> canvasButtonsList;
    [SerializeField] public GameObject buttonDisplayPrefab;

    protected override void Awake()
    {
        base.Awake();

        menuControllersList = GetComponentsInChildren<MenuController>().ToList();
        //menuControllersList.ForEach(x => x.gameObject.SetActive(false)); //x is menuController
        //SwitchMenu(Menu.MiniGameSetupMenu); this line is only used if I want a specific menu to pop up as soon as the scene opens
    }

    public void OpenMenu(Menu _menu)
    {
        OpenMenu(_menu, null);
    }

    public void OpenMenu(Menu _menu, PlayerInput _playerInput)
    {
        MenuController desiredMenu = menuControllersList.Find(x => x.menu == _menu);

        if(desiredMenu != null)
        {
            currentMenu = desiredMenu;
            allActiveMenus.Add(currentMenu);
            currentMenu.Open(_playerInput);
        }
        else
        {
            Debug.LogWarning("Desired menu was not found D:");
        }
    }

    public void SwitchMenu(Menu _menu)
    {
        CloseMenu();
        OpenMenu(_menu);
    }

    public void SwitchMenu(Menu _menu, PlayerInput _playerInput)
    {
        CloseMenu();
        OpenMenu(_menu, _playerInput);
    }

    public void CloseMenu()
    {
        if(currentMenu != null)
        {
            currentMenu.Close();
            allActiveMenus.Remove(currentMenu);
        }

        if(allActiveMenus.Count() > 0)
        {
            currentMenu = allActiveMenus[allActiveMenus.Count() - 1];
            currentMenu.GainControl();
        }
        else
        {
            currentMenu = null;
            GameManager.Instance.RestorePreviousState();
        }
    }
}
