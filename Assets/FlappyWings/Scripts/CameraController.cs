using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour{
    Vector3 gizmoPos, gizmoPosMin, gizmoPosMax;
    public Vector3 fixedOffset, dynamicOffset;
    public float smoothSpeed;

    void Start(){
        //StartCoroutine(CameraStartDelay());
    }

    void FixedUpdate(){
        if (GameManager.instance.playerList.Count == 1){
            Vector3 desiredPosition = GameManager.instance.playerList[0].transform.GetChild(0).position + fixedOffset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
        else if (GameManager.instance.playerList.Count > 1){
            Vector3 desiredPosition = FindCentroid() + fixedOffset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition + FindDynamicOffset(), smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
            gizmoPos = FindCentroid();
            gizmoPosMin = FindMinPos();
            gizmoPosMax = FindMaxPos();
        }
    }

    Vector3 FindCentroid(){
        Vector3 centerPos = new Vector3(0, 0, 0);
        foreach(var player in GameManager.instance.playerList){
            centerPos += player.transform.GetChild(0).position;
        }
        centerPos /= GameManager.instance.playerList.Count;

        return centerPos;
    }

    Vector3 FindMinPos(){
        Vector3 minPos = new Vector3(0, 0, 0);
        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float minZ = float.MaxValue;
        foreach(var player in GameManager.instance.playerList){
            if(player.transform.parent.transform.position.x < minX){
                minX = player.transform.parent.transform.position.x;
            }
            if(player.transform.parent.transform.position.y < minY){
                minY = player.transform.parent.transform.position.y;
            }
            if(player.transform.parent.transform.position.z < minZ){
                minZ = player.transform.parent.transform.position.z;
            }
        }
        return new Vector3(minX, minY, minZ);
    }

    Vector3 FindMaxPos(){
        Vector3 maxPos = new Vector3(0, 0, 0);
        float maxX = float.MinValue;
        float maxY = float.MinValue;
        float maxZ = float.MinValue;
        foreach(var player in GameManager.instance.playerList){
            if(player.transform.parent.transform.position.x > maxX){
                maxX = player.transform.parent.transform.position.x;
            }
            if(player.transform.parent.transform.position.y > maxY){
                maxY = player.transform.parent.transform.position.y;
            }
            if(player.transform.parent.transform.position.z > maxZ){
                maxZ = player.transform.parent.transform.position.z;
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
