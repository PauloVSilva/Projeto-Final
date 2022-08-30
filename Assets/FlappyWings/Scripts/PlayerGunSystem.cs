using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGunSystem : MonoBehaviour{
    [SerializeField] private int maxAmmo = 20;
    [SerializeField] private int ammo = 6;
    [SerializeField] private int extraAmmo = 0;
    [SerializeField] private float timeBetweenShots = 1f;

    [SerializeField] private Transform castPoint;

    private bool shooting = false;

    private PlayerControls playerControls;

    private void Awake(){
        playerControls = new PlayerControls();
    }

    private void OnEnable(){
        playerControls.Enable();
    }

    private void OnDisable(){
        playerControls.Disable();
    }

    private void Update(){
        //bool isGunShootingHeldDown = playerControls.PressTrigger.ReadValue<float>() > 0.1f;

        //if(!shooting && isGunShootingHeldDown){
        //    shooting = true;
        //    print("shooting");
        //}
    }
}
