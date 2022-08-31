using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HellProtection : MonoBehaviour{
    private BoxCollider myCollider;
    
    void Awake(){
        myCollider = GetComponent<BoxCollider>();
        myCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Player")){
            print("player detected");
            HealthSystem enemyHealth = other.GetComponent<HealthSystem>();
            enemyHealth.Kill();
        }
    }
}
