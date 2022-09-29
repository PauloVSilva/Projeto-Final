using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractionPromptUI : MonoBehaviour{
    [SerializeField] private Camera mainCam;
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private TextMeshProUGUI promptText;
    public bool isDisplayed = false;

    private void Start(){
        mainCam = Camera.main;
        uiPanel.SetActive(false);
    }

    private void LateUpdate(){
        if(mainCam == null){
            mainCam = Camera.main;
        }
        var rotation = mainCam.transform.rotation;
        transform.LookAt(transform.position + rotation * Vector3.forward, rotation * Vector3.up);
    }

    public void SetPrompt(string _promptText){
        promptText.text = _promptText;
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
