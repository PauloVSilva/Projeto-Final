using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelManager : MonoBehaviour{
    [SerializeField] protected Pool[] objectsToPool;
    private bool objectsPooled = false;
    private void Awake(){
        InitializeSingletonInstance();
    }

    private void Start(){
        GameManager.instance.SetSpawnPoint();
        GameManager.instance.FullyResetPlayers();
        InitializeLevel();
        while(!objectsPooled){
            AddObjectsToPool();
        }
    }

    private void OnDestroy(){
        if(objectsToPool.Length > 0){
            foreach(Pool pool in objectsToPool){
                ObjectPooler.instance.RemovePool(pool);
            }
        }
    }

    private void AddObjectsToPool(){
        if(ObjectPooler.instance == null){
            return;
        }
        if(objectsToPool.Length > 0){
            foreach(Pool pool in objectsToPool){
                ObjectPooler.instance.AddPool(pool);
            }
        }
        else{
            Debug.Log("LEVEL HAS NO OBJECTS TO POOL!!!");
        }
        objectsPooled = true;
    }

    protected abstract void InitializeSingletonInstance();

    protected abstract void InitializeLevel();
}
