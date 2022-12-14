using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
public enum MachineState{fullyFunctional, malfunctioning, broken}

public class VendingMachine : DestructibleObject, InteractorInterface{
    [SerializeField] ItemScriptableObject itemSold;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material[] materials;
    [SerializeField] int costToBuy = 350;
    [SerializeField] Transform dropPoint;
    [SerializeField] MachineState machineState;

    protected override void InitializeComponents(){
        base.InitializeComponents();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    protected override void InitializeVariables(){
        itemSold = inventory.GetItemOnSlot(0);
        
        if(machineState == MachineState.fullyFunctional && itemSold != null){
            machinePrompt = "Buy " + itemSold.itemName + " for " + costToBuy + " gold";
            meshRenderer.materials[0] = materials[(int)machineState];
        }
        else if(machineState == MachineState.malfunctioning){
            Malfunction();
        }
        else if(machineState == MachineState.broken){
            Break();
        }
    }

    protected override void ObjectTookDamage(float _damage){
        base.ObjectTookDamage(_damage);

        if(healthSystem.CurrentHealth < MaxHealth * 0.8 && machineState == MachineState.fullyFunctional){
            Malfunction();
        }
        if(healthSystem.CurrentHealth < MaxHealth * 0.4 && machineState == MachineState.malfunctioning){
            Break();
        }
    }

    protected void Malfunction(){
        machineState = MachineState.malfunctioning;
        StartCoroutine(MalfunctionBehaviour());
        machinePrompt = "Buy *&%$#@# for @(#¨(";
        meshRenderer.materials[0] = materials[(int)machineState];
    }

    protected void Break(){
        machineState = MachineState.broken;
        machinePrompt = "Out of order D:";
        meshRenderer.materials[0] = materials[(int)machineState];
    }



    IEnumerator MalfunctionBehaviour(){
        if(machineState == MachineState.malfunctioning){
            yield return new WaitForSeconds(3f);
            int randomNumber = UnityEngine.Random.Range(0, 10); //0 to 9
            if(randomNumber == 0){
                if(inventory.GetItemOnSlot(0) != null){
                    inventory.DropItem(0, dropPoint);
                }
            }
            StartCoroutine(MalfunctionBehaviour());
        }
    }


    //REQUIRED BY INTERACTOR INTERFACE
    [SerializeField] private string machinePrompt;
    [SerializeField] public string PromptString => machinePrompt;
    
    public bool Interact(Interactor interactor){
        if(machineState == MachineState.fullyFunctional){
            if(interactor.characterManager.score > costToBuy){
                interactor.characterManager.score -= costToBuy;
                inventory.DropItem(0, dropPoint);
                Debug.Log("bought");
                return true;
            }
        }
        if(machineState == MachineState.malfunctioning){
            Debug.Log("Machine is malfunctioning");
        }
        if(machineState == MachineState.broken){
            Debug.Log("Machine is broken");
        }
        return false;
    }
    
}
