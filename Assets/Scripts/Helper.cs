using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper {
    public static bool SafeSpawnPoint(Vector3 pos, LayerMask layerMask, float searchRadius){
        Ray ray = new Ray(new Vector3(pos.x, pos.y + 10, pos.z), Vector3.down);
        RaycastHit hit;

        if(Physics.SphereCast(ray, searchRadius, out hit, Mathf.Infinity, ~layerMask)){
            Debug.Log("I hit " + hit.collider.name);
            return false;
        }
        else {
            Debug.Log("Safe to spawn here");
            return true;
        }
    }
}
