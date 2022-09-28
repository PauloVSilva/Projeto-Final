using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnerObject{
    public GameObject prefab;
    public int weight;
    public SpawnerObject(GameObject _prefab, int _weight){
        prefab = _prefab;
        weight = _weight;
    }
}

public class Spawner : MonoBehaviour{
    public SpawnerObject[] spawnerObjects;
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
            if(spawnerObjects.Length > 0 && spawnerEnabled){
                SpawnEntity();
            }
        }
    }

    private void SpawnEntity(){
        //ObjectPooler.instance.SpawnFromPool(PickRandomObject().prefab, this.transform.position, this.transform.rotation, this.gameObject);
        Instantiate(PickRandomObject().prefab, this.transform.position, this.transform.rotation);
        //ObjectPooler.instance.SpawnFromPool(projectileToCast.projectileModel, castPoint.position, castPoint.rotation, this.gameObject);
        //Instantiate(objectsToBeSpawned[Random.Range(0, objectsToBeSpawned.Length)], RandomNewSpawnPosition(), transform.rotation);
    }

    private SpawnerObject PickRandomObject(){
        int totalWeight = 0;
        foreach(SpawnerObject spawnerObject in spawnerObjects){
            totalWeight += spawnerObject.weight;
        }

        SpawnerObject objectToSpawn = null;

        int randomNumber = Random.Range(0, totalWeight);

        foreach(SpawnerObject spawnerObject in spawnerObjects){
            if(randomNumber < spawnerObject.weight){
                objectToSpawn = spawnerObject;
                break;
            }
            randomNumber -= spawnerObject.weight;
        }

        return objectToSpawn;
        
    }

    private Vector3 RandomNewSpawnPosition(){
        float randomPosX = Random.Range(-spawnRange, spawnRange);
        float randomPosZ = Random.Range(-spawnRange, spawnRange);
        return new Vector3(this.transform.position.x + randomPosX, this.transform.position.y, this.transform.position.z + randomPosZ);
    }
}
