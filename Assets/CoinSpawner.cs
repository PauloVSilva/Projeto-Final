using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour{
    public GameObject[] coins;

    public float XMin, XMax, ZMin, ZMax;

    Vector3 spawnPosition;

    [SerializeField] float minSpawnInterval = 1f, maxSpawnInterval = 3f;
    [SerializeField] bool spawnerEnabled = true;
    
    private void Start(){
        StartCoroutine(SpawnCoins());
    }

    IEnumerator SpawnCoins(){
        while (spawnerEnabled){
            yield return new WaitForSeconds(Random.Range(minSpawnInterval, maxSpawnInterval));
            if(GameManager.instance.playerList.Count > 1){
                Instantiate(coins[0], RandomNewSpawnPosition(), transform.rotation);
            }
        }
    }

    Vector3 RandomNewSpawnPosition(){
        float randomX = Random.Range(XMin, XMax);
        float randomZ = Random.Range(ZMin, ZMax);
        return new Vector3(randomX, 0.5f, randomZ);
    }
}
