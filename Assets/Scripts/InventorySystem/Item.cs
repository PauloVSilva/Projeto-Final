using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]

public class Item : MonoBehaviour{
    [SerializeField] public ItemScriptableObject item;
}
