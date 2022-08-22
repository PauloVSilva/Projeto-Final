using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour{
    public GameObject[] objectsToBeSpawned;
    public LayerMask layerMask;

    public float XMin, XMax, ZMin, ZMax;

    Vector3 spawnPosition;
    //Vector3 gizmoPos;

    [SerializeField] float minSpawnInterval = 1f, maxSpawnInterval = 3f;
    [SerializeField] bool spawnerEnabled = true;
    
    private void Start(){
        StartCoroutine(SpawnCoins());
    }

    IEnumerator SpawnCoins(){
        while (spawnerEnabled){
            yield return new WaitForSeconds(Random.Range(minSpawnInterval, maxSpawnInterval));

            if(GameManager.instance.playerList.Count > 0){
                Instantiate(objectsToBeSpawned[Random.Range(0, objectsToBeSpawned.Length)], RandomNewSpawnPosition(), transform.rotation);
            }

            //Vector3 chosenPos = RandomNewSpawnPosition();
            //if(Helper.SafeSpawnPoint(chosenPos, layerMask, 1f)){
            //    Instantiate(coins[Random.Range(0, coins.Length)], chosenPos, transform.rotation);
            //}


        }
    }

    Vector3 RandomNewSpawnPosition(){
        float randomX = Random.Range(XMin, XMax);
        float randomZ = Random.Range(ZMin, ZMax);
        return new Vector3(this.transform.position.x + randomX, this.transform.position.y, this.transform.position.z + randomZ);
    }
}
