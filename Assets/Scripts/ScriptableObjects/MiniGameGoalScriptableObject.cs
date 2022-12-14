using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New MiniGameGoal", menuName = "MiniGameGoal")]

public class MiniGameGoalScriptableObject : ScriptableObject{
    [SerializeField] public MiniGameScriptableObject parentMiniGame;
    [SerializeField] public MiniGameGoal miniGameGoal;
    [SerializeField] public Sprite goalSprite;
    [SerializeField] public string goalName;
    [SerializeField] public string goalDescription;
    [SerializeField] public string goalKeyword;
    [SerializeField] public int goalMultiplier;
}
