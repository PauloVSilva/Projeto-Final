using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Example:
/// public class GameManager: PersistentSingleton<GameManager> { ... }
/// 
/// The difference between Singleton and PersistentSingleton is that PersistentSingleton will not be destroyed when loading a new scene
/// </summary>


public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        if (Instance == null)
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
