using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelection : MonoBehaviour{

    public CharacterStatsScriptableObject Character;

    public void SpawnCharacter(){
        int index = GameManager.instance.spawnPoints.Length;
        int randomIndex = UnityEngine.Random.Range(0, index);

        Instantiate(GameManager.instance.playerPrefabs[0], GameManager.instance.spawnPoints[randomIndex].transform.position, transform.rotation, this.transform);
        transform.parent = GameManager.instance.transform;
    }

    public void SpawnCharacter(CharacterStatsScriptableObject Character){
        int index = GameManager.instance.spawnPoints.Length;
        int randomIndex = UnityEngine.Random.Range(0, index);

        Instantiate(GameManager.instance.playerPrefabs[0], GameManager.instance.spawnPoints[randomIndex].transform.position, transform.rotation, this.transform);
        transform.parent = GameManager.instance.transform;
    }
}
