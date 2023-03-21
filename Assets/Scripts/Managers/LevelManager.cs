using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelManager : Singleton<LevelManager>
{
    [field: SerializeField] public Level currentLevel { get; private set; }

    protected override void Awake()
    {
        base.Awake();
    }


    public void SetLevel(Level _level)
    {
        currentLevel = _level;
    }

}
