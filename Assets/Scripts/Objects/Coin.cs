using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : Item, IPooledObjects{
    [SerializeField] public int value;

    public void OnObjectSpawn(){ //replaces Start()
        InitializeItemVariables();

        this.transform.parent = ObjectPooler.instance.transform;

        isPooled = true;
    }
}
