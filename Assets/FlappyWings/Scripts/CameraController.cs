using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour{
    Transform playerOne;
    Vector3 gizmoPos;
    public Vector3 offset;
    public float smoothSpeed;

    void Start(){
        StartCoroutine(CameraStartDelay());
    }

    void FixedUpdate(){
        if(playerOne != null && GameManager.instance.playerList.Count < 2){
            Vector3 desiredPosition = playerOne.transform.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
        else if (GameManager.instance.playerList.Count >= 2){
            Vector3 desiredPosition = FindCentroid() + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
            gizmoPos = FindCentroid();
        }
    }

    Vector3 FindCentroid(){
        var totalX = 0f;
        var totalY = 0f;
        var totalZ = 0f;

        foreach(var player in GameManager.instance.playerList){
            totalX += player.transform.parent.transform.position.x;
            totalY += player.transform.parent.transform.position.y;
            totalZ += player.transform.parent.transform.position.z;
        }

        var centerX = totalX / GameManager.instance.playerList.Count;
        var centerY = totalY / GameManager.instance.playerList.Count;
        var centerZ = totalZ / GameManager.instance.playerList.Count;

        return new Vector3(centerX, centerY, centerZ);
    }

    private void OnDrawGizmos(){
        Gizmos.DrawCube(gizmoPos, new Vector3(1, 1, 1));
    }

    IEnumerator CameraStartDelay(){
        yield return new WaitForSeconds(0.2f);
        playerOne = GameManager.instance.playerList[0].gameObject.transform;
        transform.position = playerOne.transform.position + offset;
    }
}
