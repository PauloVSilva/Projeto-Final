using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FloatingHealthBar : MonoBehaviour{
    [SerializeField] private Camera mainCam;
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private Slider healthBar;
    private float displayTime;

    private void Awake(){
        InitializeVariables();
    }

    private void Start(){
        InitializeOthers();
    }

    private void InitializeVariables(){
        healthBar.minValue = 0;
        healthBar.maxValue = 1;
        displayTime = 0;
    }

    private void InitializeOthers(){
        mainCam = Camera.main;
    }

    public void SetMaxHealth(float _maxHealth){
        healthBar.maxValue = _maxHealth;
    }

    public void UpdateHealthBar(float _health){
        uiPanel.SetActive(true);
        healthBar.value = _health;
        displayTime = 1;
    }

    private void Update(){
        if(displayTime > 0){
            displayTime -= Time.deltaTime;
        }
        if(displayTime <= 0){
            uiPanel.SetActive(false);
        }
        
        var rotation = mainCam.transform.rotation;
        transform.LookAt(transform.position + rotation * Vector3.forward, rotation * Vector3.up);
    }

}
