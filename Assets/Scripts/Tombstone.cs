using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tombstone : MonoBehaviour{
    void Start(){
        float delay = transform.parent.GetComponent<CharacterStats>().timeToRespawn;
        Destroy(gameObject, delay);
    }
}
