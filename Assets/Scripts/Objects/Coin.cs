using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : Item, IPooledObjects{
    [SerializeField] private int value;

    public int Value => value;

    public void OnObjectSpawn(){ //replaces Start()
        InitializeItemVariables();

        this.transform.parent = ObjectPooler.instance.transform;

        isPooled = true;
    }
}
