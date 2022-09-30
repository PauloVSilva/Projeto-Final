using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : Item{
    [SerializeField] public int value;

    public override void OnObjectSpawn(){ //replaces Start()
        InitializeVariables();
        this.transform.parent = ObjectPooler.instance.transform;
    }

    protected override void MaxAgeReached(){
        this.gameObject.SetActive(false);
    }
}
