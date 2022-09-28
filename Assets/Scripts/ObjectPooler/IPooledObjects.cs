using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IPooledObjects : MonoBehaviour{
    protected bool disableCoroutineIsRunning;
    public virtual void OnObjectSpawn(){
    }

    protected virtual IEnumerator DisableObject(float time){
        disableCoroutineIsRunning = true;
        yield return new WaitForSeconds(time);
        this.gameObject.SetActive(false);
    }

    protected virtual void DisableCoroutine(float time){
        StopCoroutine(DisableObject(time));
        disableCoroutineIsRunning = false;
    }

    protected virtual void OnDisable(){
        DisableCoroutine(0f);
    }
    
}
