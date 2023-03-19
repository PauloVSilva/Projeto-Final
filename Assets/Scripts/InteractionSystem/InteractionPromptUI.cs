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

    private void Start(){
        //mainCam = Camera.main;
        mainCam = GameManager.Instance.mainCamera;
        uiPanel.SetActive(false);
    }

    private void LateUpdate(){
        if(mainCam == null){
            mainCam = Camera.main;
        }
        var rotation = mainCam.transform.rotation;
        transform.LookAt(transform.position + rotation * Vector3.forward, rotation * Vector3.up);
    }

    public void SetPrompt(PlayerInput playerInput, string _promptText){
        promptText.text = _promptText;

        CanvasButtonDisplay canvasButtonDisplay = new CanvasButtonDisplay();
        for(int i = 0; i < CanvasManager.Instance.canvasButtonsList.Count; i++){
            if(buttonLabel[0] == CanvasManager.Instance.canvasButtonsList[i].buttonType){
                canvasButtonDisplay = CanvasManager.Instance.canvasButtonsList[i];
                break;
            }
        }
        
        if(playerInput != null){
            //Debug.Log(playerInput.devices[0].GetType().ToString());
            if(playerInput.devices[0].GetType().ToString() == "UnityEngine.InputSystem.DualShock.FastDualShock4GamepadHID"){
                promptImage.sprite = canvasButtonDisplay.buttonSprite[1];
            }
            else{
                promptImage.sprite = canvasButtonDisplay.buttonSprite[0];
            }
        }

    }

    public void ClearPrompt(){
        promptText.text = null;
    }

    public void OpenPanel(){
        if(!isDisplayed){
            uiPanel.SetActive(true);
            isDisplayed = true;
        }
    }

    public void ClosePanel(){
        if(isDisplayed){
            uiPanel.SetActive(false);
            isDisplayed = false;
        }
    }

}
