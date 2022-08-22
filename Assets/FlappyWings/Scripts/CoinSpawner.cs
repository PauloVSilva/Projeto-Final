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
                Instantiate(coins[Random.Range(0, coins.Length)], RandomNewSpawnPosition(), transform.rotation);
            }
        }
    }

    Vector3 RandomNewSpawnPosition(){
        float randomX = Random.Range(XMin, XMax);
        float randomZ = Random.Range(ZMin, ZMax);
        return new Vector3(randomX, this.transform.position.y, randomZ);
    }
}
