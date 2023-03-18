using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public abstract class LevelManager : MonoBehaviour
{
    [SerializeField] protected Pool[] objectsToPool;

    public GameObject[] levelSpawnPoints;

    public Camera mainCamera;

    private void Awake(){
        InitializeSingletonInstance();

        GameManager.Instance.spawnPoints = levelSpawnPoints;
        GameManager.Instance.mainCamera = mainCamera;

        FullyResetPlayers();
    }

    private void Start(){
        RepositionAllPlayers();

        InitializeLevel();
        
        if(!AddObjectsToPool()){
            Debug.LogError("Something went wrong. ObjectPooler couldn't be initialized. Return to main menu.");
        }
    }

    private void OnDestroy(){
        ClearPools();
    }


    protected void RepositionAllPlayers()
    {
        foreach (PlayerInput playerInput in GameManager.Instance.playerList)
        {
            int index = playerInput.playerIndex % levelSpawnPoints.Length;
            playerInput.transform.position = levelSpawnPoints[index].transform.position;
        }
    }

    private void FullyResetPlayers()
    {
        if (GameManager.Instance.playerList.Count == 0) return;

        mainCamera.GetComponent<CameraController>().ResetTrackedList();

        foreach (PlayerInput playerInput in GameManager.Instance.playerList)
        {
            playerInput.GetComponent<CharacterManager>().FullReset();
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

    private void ClearPools()
    {
        if (objectsToPool.Length > 0)
        {
            foreach (Pool pool in objectsToPool)
            {
                ObjectPooler.instance.RemovePool(pool);
            }
        }
    }

    protected virtual void InitializeSingletonInstance() { }

    protected abstract void InitializeLevel();
}
