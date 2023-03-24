using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class Level : MonoBehaviour
{
    public List<PlayerSpawn> spawnList = new List<PlayerSpawn>();
    public List<Pool> objectsToPool = new List<Pool>();
    public List<Spawner> spawners = new List<Spawner>();
    public List<Coin> coins = new List<Coin>(); //intended to be used for DimeDrop

    public Camera mainCamera; //not sure if necessary

    [SerializeField] private bool playerSpawnEnabled;

    private void Awake()
    {
        InitializeVariables();

        GameManager.Instance.mainCamera = mainCamera;

        LevelManager.Instance.SetLevel(this);

        FullyResetPlayers();

        RepositionAllPlayers();

        InitializePools();
    }

    private void Start()
    {
    }

    private void OnDestroy()
    {
        ClearPools();
    }


    private void InitializeVariables()
    {
        playerSpawnEnabled = true;
    }


    public void SpawnPlayerRandomly(PlayerInput playerInput)
    {
        int random = Random.Range(0, spawnList.Count);

        spawnList[random].SpawnPlayer(playerInput);
    }

    private void RepositionAllPlayers()
    {
        foreach(PlayerInput playerInput in GameManager.Instance.playerList)
        {
            playerInput.TryGetComponent(out CharacterManager characterManager);
            characterManager.ReplaceCharacter();
        }

        for(int i = 0; i < GameManager.Instance.playerList.Count; i++)
        {
            int index = i % spawnList.Count;

            spawnList[index].SpawnPlayer(GameManager.Instance.playerList[i]);
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



    public void SetSpawnersEnabled(bool _enabled)
    {
        foreach (Spawner spawner in spawners)
        {
            spawner.SetEnabled(_enabled);
        }
    }

    private void InitializePools()
    {
        if (ObjectPooler.Instance == null)
        {
            Debug.Log("Object Pooler doesn't exist");
            return;
        }

        if (objectsToPool.Count < 1)
        {
            Debug.LogWarning("Level has no objects to pool");
            return;
        }

        foreach (Pool pool in objectsToPool)
        {
            ObjectPooler.Instance.AddPool(pool);
        }
    }

    private void ClearPools()
    {
        if (ObjectPooler.Instance == null)
        {
            Debug.Log("Object Pooler doesn't exist");
            return;
        }

        if (objectsToPool.Count < 1)
        {
            Debug.Log("Level has no pools to be clared");
            return;
        }

        foreach (Pool pool in objectsToPool)
        {
            ObjectPooler.Instance.RemovePool(pool);
        }
    }
}
