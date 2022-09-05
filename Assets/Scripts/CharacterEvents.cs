using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEvents : MonoBehaviour{
    
    //EVENTS
    //public event System.Action<int> OnScoreChanged;
    //public event System.Action<int> OnKillsChanged;
    //public event System.Action<int> OnDeathsChanged;
    //public event System.Action<float> OnHealthUpdated; 
    //public event System.Action<float> OnWasHealed;
    //public event System.Action<float> OnWasDamaged;

    private void Awake() {
        
    }

    private void Start() {
        SubscribeToEvents();
    }

    private void SubscribeToEvents(){
        //HEALTH EVENTS
    }

    

    //public void FilterCollision(GameObject player, GameObject gameObject){
    //    if(gameObject.CompareTag("Coin")){
    //        if(gameObject.GetComponent<Coin>().canBePickedUp){
    //            IncreaseScore(gameObject.GetComponent<Coin>().value);
    //            Destroy(gameObject);
    //        }
    //    }
    //    if(gameObject.CompareTag("Instadeath")){
    //        player.GetComponent<HealthSystem>().Kill();
    //    }
    //}

}
