using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheVoid : MonoBehaviour{
    private void OnTriggerStay(Collider other){
        if(other.gameObject.GetComponent<HealthSystem>() != null){
            other.GetComponent<HealthSystem>().Kill();
        }
    }
}
