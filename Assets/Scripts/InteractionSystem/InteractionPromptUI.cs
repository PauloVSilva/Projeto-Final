using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class InteractionPromptUI : MonoBehaviour{
    //ACTION BUTTONS ON SCREEN
    [SerializeField] private ButtonType[] buttonLabel;

    [SerializeField] private Camera mainCam;
    [SerializeField] private GameObject uiPanel;

    [SerializeField] private Image promptImage;
    [SerializeField] private TextMeshProUGUI promptText;
    public bool isDisplayed = false;

    private void Start()
    {
        //mainCam = Camera.main;
        mainCam = GameManager.Instance.mainCamera;
        uiPanel.SetActive(false);
    }

    private void LateUpdate()
    {
        if(mainCam == null)
        {
            mainCam = Camera.main;
        }

        var rotation = mainCam.transform.rotation;
        transform.LookAt(transform.position + rotation * Vector3.forward, rotation * Vector3.up);
    }

    public void SetPrompt(string prompText)
    {
        promptText.text = prompText;

        promptImage.sprite = null;
        promptImage.enabled = false;
    }

    public void SetPrompt(PlayerInput playerInput, string _promptText)
    {
        promptText.text = _promptText;

        CanvasButtonDisplay canvasButtonDisplay = new CanvasButtonDisplay();

        for(int i = 0; i < CanvasManager.Instance.canvasButtonsList.Count; i++)
        {
            if(buttonLabel[0] == CanvasManager.Instance.canvasButtonsList[i].buttonType)
            {
                canvasButtonDisplay = CanvasManager.Instance.canvasButtonsList[i];
                break;
            }
        }

        promptImage.sprite = canvasButtonDisplay.buttonSprite[(int)playerInput.GetComponent<CharacterManager>().playerDevice];
        promptImage.enabled = true;
    }

    public void ClearPrompt()
    {
        promptText.text = null;
    }

    public void OpenPanel()
    {
        isDisplayed = true;
        uiPanel.SetActive(isDisplayed);
    }

    public void ClosePanel()
    {
        isDisplayed = false;
        uiPanel.SetActive(isDisplayed);
    }

}
