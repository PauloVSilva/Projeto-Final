using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : Item, IPooledObjects
{
    [SerializeField] private CoinScriptableObject coin;
    [SerializeField] private int value;

    [SerializeField] private AudioClip pickUpSFX;
    public int Value => value;

    private void Start()
    {
        GetScriptableObjectVariables();
    }

    protected override void GetScriptableObjectVariables(){
        item = coin;
        
        base.GetScriptableObjectVariables();

        value = coin.value;
    }

    public void OnObjectSpawn(){ //replaces Start()
        InitializeItemVariables();

        this.transform.parent = ObjectPooler.Instance.transform;

        transform.Rotate(0, 0, 0);

        isPooled = true;
    }

    public void PickedUp(GameObject _gameObject)
    {
        //who the fuck had the idea to pass a GO as refe.... oh it was me
        _gameObject.GetComponent<CharacterManager>().IncreaseScore(value);

        //I gotta make this work later
        //audioSource.PlayOneShot(pickUpSFX);

        Despawn();
    }
}
