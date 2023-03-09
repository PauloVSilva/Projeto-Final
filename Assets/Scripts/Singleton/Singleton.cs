using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Example:
/// public class GameManager: Singleton<GameManager> { ... }
/// The difference between Singleton and PersistentSingleton is that PersistentSingleton will not be destroyed when loading a new scene
/// </summary>

public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        if (Instance == null)
        {
            base.Awake(); // Recebe o item base
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
