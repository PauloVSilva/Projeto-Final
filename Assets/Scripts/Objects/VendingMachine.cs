using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachine : DestructibleObject, InteractorInterface{
    [SerializeField] ItemScriptableObject itemSold;
    [SerializeField] int costToBuy = 350;
    [SerializeField] Transform dropPoint;
    [SerializeField] bool isBroken;

    protected override void Start(){
        itemSold = inventory.GetItemOnSlot(0);
        
        if(itemSold != null){
            machinePrompt = "Buy " + itemSold.itemName + " for " + costToBuy;
        }
        else{
            machinePrompt = "Out of order D:";
        }

        healthSystem.OnDeath += ObjectDestroyed;
        healthSystem.OnDamaged += ObjectTookDamage;
        interactionPromptUI.SetPrompt(machinePrompt);
    }

    protected override void ObjectTookDamage(float _damage){
        damageFeedback?.DisplayDamageTaken(_damage);
        if(healthSystem.CurrentHealth < MaxHealth * 0.5){
            Break();
        }
    }

    protected void Break(){
        isBroken = true;
    }


    //REQUIRED BY INTERACTOR INTERFACE
    [SerializeField] private string machinePrompt;
    [SerializeField] private InteractionPromptUI interactionPromptUI;
    [SerializeField] public string PromptString => machinePrompt;
    [SerializeField] public InteractionPromptUI PromptUI => interactionPromptUI;
    public bool Interact(Interactor interactor){
        if(!isBroken){
            if(interactor.transform.parent.GetComponent<CharacterStats>().score > costToBuy){
                interactor.transform.parent.GetComponent<CharacterStats>().score -= costToBuy;
                inventory.DropItem(0, dropPoint);
                Debug.Log("bought");
                return true;
            }
            return false;
        }
        else{
            Debug.Log("Machine is broken");
            return false;
        }
    }
    
}
