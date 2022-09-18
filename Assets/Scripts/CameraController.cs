using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour{
    Vector3 gizmoPos, gizmoPosMin, gizmoPosMax;
    public Vector3 fixedOffset, dynamicOffset;
    public float smoothSpeed;
    public List<GameObject> objectsTracked = new List<GameObject>();

    public void AddCharacter(GameObject character){
        objectsTracked.Add(character);
    }

    public void AddPlayer(PlayerInput playerInput){
        objectsTracked.Add(playerInput.transform.GetComponent<CharacterSelection>().characterObject.gameObject);
    }

    public void RemoveCharacter(GameObject character){
        objectsTracked.Remove(character);
    }

    public void RemovePlayer(PlayerInput playerInput){
        objectsTracked.Remove(playerInput.transform.GetComponent<CharacterSelection>().characterObject.gameObject);
    }

    void FixedUpdate(){
        if (objectsTracked.Count == 1){
            Vector3 desiredPosition = objectsTracked[0].transform.position + fixedOffset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
        else if (objectsTracked.Count > 1){
            Vector3 desiredPosition = FindCentroid() + fixedOffset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition + FindDynamicOffset(), smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
            //gizmoPos = FindCentroid();
            //gizmoPosMin = FindMinPos();
            //gizmoPosMax = FindMaxPos();
        }
        else{
            Vector3 desiredPosition = new Vector3(0, 0, 0) + fixedOffset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
    }

    Vector3 FindCentroid(){
        Vector3 centerPos = new Vector3(0, 0, 0);
        foreach(var player in objectsTracked){
            centerPos += player.transform.position;
        }
        centerPos /= objectsTracked.Count;

        return centerPos;
    }

    Vector3 FindMinPos(){
        Vector3 minPos = new Vector3(0, 0, 0);
        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float minZ = float.MaxValue;
        foreach(var player in objectsTracked){
            if(player.transform.position.x < minX){
                minX = player.transform.position.x;
            }
            if(player.transform.position.y < minY){
                minY = player.transform.position.y;
            }
            if(player.transform.position.z < minZ){
                minZ = player.transform.position.z;
            }
        }
        return new Vector3(minX, minY, minZ);
    }

    Vector3 FindMaxPos(){
        Vector3 maxPos = new Vector3(0, 0, 0);
        float maxX = float.MinValue;
        float maxY = float.MinValue;
        float maxZ = float.MinValue;
        foreach(var player in objectsTracked){
            if(player.transform.position.x > maxX){
                maxX = player.transform.position.x;
            }
            if(player.transform.position.y > maxY){
                maxY = player.transform.position.y;
            }
            if(player.transform.position.z > maxZ){
                maxZ = player.transform.position.z;
            }
        }
        return new Vector3(maxX, maxY, maxZ);
    }

    Vector3 FindDynamicOffset(){
        float distanceX, distanceZ;
        
        distanceX = FindMaxPos().x - FindMinPos().x;
        distanceZ = FindMaxPos().z - FindMinPos().z;
        return new Vector3(0, (distanceX + distanceZ) * 0.25f, (distanceX + distanceZ) * -0.25f);
    }

    private void OnDrawGizmos(){
        Gizmos.DrawCube(gizmoPos, new Vector3(1, 1, 1));
        Gizmos.DrawCube(gizmoPosMin, new Vector3(1, 1, 1));
        Gizmos.DrawCube(gizmoPosMax, new Vector3(1, 1, 1));
    }
}
