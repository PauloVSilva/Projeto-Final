using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageFeedback : MonoBehaviour{
    [SerializeField] private GameObject floatingPanelPrefab;
    [SerializeField] private Transform castPoint;

    public void DisplayDamageTaken(float _damageTaken){
        GameObject floatingPanel = Instantiate(floatingPanelPrefab, castPoint.position, castPoint.rotation);
        floatingPanel.GetComponent<FloatingPanel>().SetDamageAmount(_damageTaken);
    }

}
