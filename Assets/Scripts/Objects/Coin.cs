using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : Item, IPooledObjects{
    [SerializeField] private CoinScriptableObject coin;
    [SerializeField] private int value;
    public int Value => value;

    private void Start(){
        SetScriptableObjectVariables();
    }

    protected override void SetScriptableObjectVariables(){
        item = coin;
        
        base.SetScriptableObjectVariables();

        value = coin.value;
    }

    public void OnObjectSpawn(){ //replaces Start()
        InitializeItemVariables();

        this.transform.parent = ObjectPooler.instance.transform;

        isPooled = true;
    }

    public void PickedUp(GameObject _gameObject){
        _gameObject.GetComponent<CharacterManager>().IncreaseScore(value);
        Despawn();
    }
}
