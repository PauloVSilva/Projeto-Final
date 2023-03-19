using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Item, IPooledObjects{
    [SerializeField] private FoodScriptableObject food;
    [SerializeField] private int value;
    public int Value => value;

    private void Start(){
        GetScriptableObjectVariables();
    }

    protected override void GetScriptableObjectVariables(){
        item = food;
        
        base.GetScriptableObjectVariables();

        value = food.value;
    }

    public void OnObjectSpawn(){ //replaces Start()
        InitializeItemVariables();

        this.transform.parent = ObjectPooler.Instance.transform;

        isPooled = true;
    }

    public void PickedUp(GameObject _gameObject){
        _gameObject.GetComponent<HealthSystem>().Heal(value);
        Despawn();
    }
}
