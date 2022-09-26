using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FloatingHealthBar : MonoBehaviour{
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private Camera mainCam;
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private Slider healthBar;

    private void Start(){
        mainCam = Camera.main;
        healthBar.minValue = 0;
        healthBar.maxValue = healthSystem.MaxHealth;
    }

    public void UpdateHealthBar(float _damage){
    }

}
