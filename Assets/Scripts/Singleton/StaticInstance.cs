using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base class for other Singleton scripts
public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }
    protected virtual void Awake() => Instance = this as T;
}
