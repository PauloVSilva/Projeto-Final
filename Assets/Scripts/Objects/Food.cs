using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Item, IPooledObjects{
    [SerializeField] private FoodScriptableObject food;
    [SerializeField] private int value;
    public int Value => value;

    private void Start(){
        SetScriptableObjectVariables();
    }

    protected override void SetScriptableObjectVariables(){
        item = food;
        
        base.SetScriptableObjectVariables();

        value = food.value;
    }

    public void OnObjectSpawn(){ //replaces Start()
        InitializeItemVariables();

        this.transform.parent = ObjectPooler.instance.transform;

        isPooled = true;
    }

    public void PickedUp(GameObject _gameObject){
        _gameObject.GetComponent<HealthSystem>().Heal(value);
        Despawn();
    }
}
