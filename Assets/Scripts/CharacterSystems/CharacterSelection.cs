using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelection : MonoBehaviour{
    public CharacterStatsScriptableObject Character;
    public GameObject characterObject;

    private void Awake() {
        characterObject = null;
        SpawnCharacter();
    }

    public void SpawnCharacter(){
        int index = GameManager.instance.spawnPoints.Length;
        int randomIndex = UnityEngine.Random.Range(0, index);

        GetComponent<CharacterStats>().SetStats();
        characterObject = GameObject.Instantiate(Character.characterModel[0], GameManager.instance.spawnPoints[randomIndex].transform.position, transform.rotation, this.transform);
        GetComponent<CharacterEvents>().SetEvents();
        transform.parent = GameManager.instance.transform;
    }
}
