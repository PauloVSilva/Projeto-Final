using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelManager : MonoBehaviour{
    [SerializeField] protected Pool[] objectsToPool;
    public GameObject[] levelSpawnPoints;
    public Camera mainCamera;

    private void Awake(){
        InitializeSingletonInstance();
    }

    private void Start(){
        GameManager.instance.spawnPoints = levelSpawnPoints;
        GameManager.instance.mainCamera = mainCamera;
        GameManager.instance.FullyResetPlayers();

        InitializeLevel();
        
        if(!AddObjectsToPool()){
            Debug.LogError("Something went wrong. ObjectPooler couldn't be initialized. Return to main menu.");
        }
    }

    private void OnDestroy(){
        if(objectsToPool.Length > 0){
            foreach(Pool pool in objectsToPool){
                ObjectPooler.instance.RemovePool(pool);
            }
        }
    }

    private bool AddObjectsToPool(){
        if(ObjectPooler.instance == null){
            return false;
        }
        if(objectsToPool.Length == 0){
            Debug.LogWarning("LEVEL HAS NO OBJECTS TO POOL!!!");
            return false;
        }
        foreach(Pool pool in objectsToPool){
            ObjectPooler.instance.AddPool(pool);
        }
        return true;
    }

    protected abstract void InitializeSingletonInstance();

    protected abstract void InitializeLevel();
}
