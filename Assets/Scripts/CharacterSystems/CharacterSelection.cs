using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSelection : MonoBehaviour{

    [SerializeField] private CharacterEvents characterEvents;
    [SerializeField] private CharacterStats characterStats;
    [SerializeField] public CharacterStatsScriptableObject Character;
    [SerializeField] public GameObject characterObject;
    [SerializeField] public event System.Action OnCharacterChosen;

    private void Awake() {
        characterObject = null;
    }

    public void SpawnCharacter(CharacterStatsScriptableObject _character){
        int index = GameManager.instance.spawnPoints.Length;
        int randomIndex = UnityEngine.Random.Range(0, index);

        Character = _character;

        characterStats.SetStats();
        characterObject = GameObject.Instantiate(Character.characterModel[0], GameManager.instance.spawnPoints[randomIndex].transform.position, transform.rotation, this.transform);
        characterEvents.SetEvents();
        transform.parent = GameManager.instance.transform;
        GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
        OnCharacterChosen?.Invoke();
    }
}
