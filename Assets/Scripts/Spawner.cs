using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour{
    public GameObject[] objectsToBeSpawned;
    public LayerMask layerMask;

    public float spawnRange;

    Vector3 spawnPosition;
    //Vector3 gizmoPos;

    //[SerializeField] float minSpawnInterval = 1f, maxSpawnInterval = 3f;
    public bool spawnerEnabled = true;
    public float cooldown = 1f;
    public float ready = 1f;
    
    private void Start(){
        //StartCoroutine(SpawnEntity());
    }

    private void Update(){
        ready -= Time.deltaTime;
        if(ready <= 0){
            ready += cooldown;
            SpawnEntity();
        }
    }

    private void SpawnEntity(){
        if(spawnerEnabled){
            if(GameManager.instance.playerList.Count > 0){
                Instantiate(objectsToBeSpawned[Random.Range(0, objectsToBeSpawned.Length)], RandomNewSpawnPosition(), transform.rotation);
            }
        }
    }

    //IEnumerator SpawnEntity(){
    //    while (spawnerEnabled){
    //        yield return new WaitForSeconds(Random.Range(minSpawnInterval, maxSpawnInterval));
    //        if(GameManager.instance.playerList.Count > 0){
    //            Instantiate(objectsToBeSpawned[Random.Range(0, objectsToBeSpawned.Length)], RandomNewSpawnPosition(), transform.rotation);
    //        }
    //        //Vector3 chosenPos = RandomNewSpawnPosition();
    //        //if(Helper.SafeSpawnPoint(chosenPos, layerMask, 1f)){
    //        //    Instantiate(coins[Random.Range(0, coins.Length)], chosenPos, transform.rotation);
    //        //}
    //    }
    //}

    Vector3 RandomNewSpawnPosition(){
        float randomPosX = Random.Range(-spawnRange, spawnRange);
        float randomPosZ = Random.Range(-spawnRange, spawnRange);
        return new Vector3(this.transform.position.x + randomPosX, this.transform.position.y, this.transform.position.z + randomPosZ);
    }
}
