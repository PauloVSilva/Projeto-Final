using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachine : DestructibleObject, InteractorInterface{
    [SerializeField] ItemScriptableObject itemSold;
    [SerializeField] int costToBuy = 1;
    [SerializeField] Transform dropPoint;

    protected override void Start(){
        itemSold = inventory.GetItemOnSlot(0);
        
        machinePrompt = "Buy " + itemSold.itemName + " for " + costToBuy;

        healthSystem.OnDeath += ObjectDestroyed;
        healthSystem.OnDamaged += ObjectTookDamage;
        interactionPromptUI.SetPrompt(machinePrompt);
    }


    //REQUIRED BY INTERACTOR INTERFACE
    [SerializeField] private string machinePrompt;
    [SerializeField] private InteractionPromptUI interactionPromptUI;
    [SerializeField] public string PromptString => machinePrompt;
    [SerializeField] public InteractionPromptUI PromptUI => interactionPromptUI;
    public bool Interact(Interactor interactor){
        Debug.Log(interactor.transform.parent.GetComponent<CharacterStats>().score);
        if(interactor.transform.parent.GetComponent<CharacterStats>().score > costToBuy){
            interactor.transform.parent.GetComponent<CharacterStats>().score -= costToBuy;
            inventory.DropItem(0, dropPoint);
            Debug.Log("bought");
        }
        return true;
    }
    
}
