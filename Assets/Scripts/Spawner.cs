using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour{
    public GameObject[] objectsToBeSpawned;
    public float spawnRange;
    public bool spawnerEnabled;
    public float cooldown;
    public float ready;
    
    private void Start(){
        InitializeVariables();
    }

    private void InitializeVariables(){
        cooldown = 1f;
        ready = 0f;
    }

    private void Update(){
        ready -= Time.deltaTime;
        if(ready <= 0){
            ready += cooldown;
            if(objectsToBeSpawned.Length > 0){
                SpawnEntity();
            }
        }
    }

    private void SpawnEntity(){
        if(spawnerEnabled){
            if(GameManager.instance.playerList.Count > 0){
                Instantiate(objectsToBeSpawned[Random.Range(0, objectsToBeSpawned.Length)], RandomNewSpawnPosition(), transform.rotation);
            }
        }
    }

    private Vector3 RandomNewSpawnPosition(){
        float randomPosX = Random.Range(-spawnRange, spawnRange);
        float randomPosZ = Random.Range(-spawnRange, spawnRange);
        return new Vector3(this.transform.position.x + randomPosX, this.transform.position.y, this.transform.position.z + randomPosZ);
    }
}
