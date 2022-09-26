using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingPanel : MonoBehaviour{
    [SerializeField] private Camera mainCam;
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private TextMeshProUGUI damageText;

    private void Start(){
        mainCam = Camera.main;
        Destroy(gameObject, 1f);
    }

    public void SetDamageAmount(float _damage){
        damageText.text = _damage.ToString();
    }

    public void SetDamageAmount(string _damage){
        damageText.text = _damage;
    }

    private void LateUpdate(){
        var rotation = mainCam.transform.rotation;
        transform.LookAt(transform.position + rotation * Vector3.forward, rotation * Vector3.up);
    }

}
