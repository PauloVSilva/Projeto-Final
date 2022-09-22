using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New MiniGame", menuName = "MiniGame")]

public class MiniGameScriptableObject : ScriptableObject{
    [SerializeField] public MiniGame minigame;
    [SerializeField] public Sprite miniGameSprite;
    [SerializeField] public string miniGameName;
    [SerializeField] public MiniGameGoalScriptableObject[] miniGamesGoalsAvaliable;

}
