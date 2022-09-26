using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageFeedback : MonoBehaviour{
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private GameObject floatingPanelPrefab;
    [SerializeField] public float lastDamageTaken;
    [SerializeField] private Transform castPoint;

    private void Start(){
        SubscribeToEvents();
    }

    private void SubscribeToEvents(){
        healthSystem.OnDamaged += DisplayDamageTaken;
    }

    private void DisplayDamageTaken(float _damageTaken){
        lastDamageTaken = _damageTaken;
        
        GameObject floatingPanel = Instantiate(floatingPanelPrefab, castPoint.position, castPoint.rotation);
        floatingPanel.GetComponent<FloatingPanel>().SetDamageAmount(lastDamageTaken);
    }

}
