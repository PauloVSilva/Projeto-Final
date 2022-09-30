using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageManager : MonoBehaviour{
    public static MessageManager instance;
    [SerializeField] private string[] greetMessages;
    [SerializeField] private string[] pauseMessages;
    [SerializeField] private string[] killMessages;
    [SerializeField] private string[] playerVictoryMessages;

    private void Awake(){
        if(instance == null){
            instance = this;
        }
        else{
            Destroy(this);
        }
    }

    public string StringEditor(string message, string oldPart, string newPart){
        if(message.Contains(oldPart)){
            message = message.Replace(oldPart, newPart);
        }
        return message;
    }

    public string StringEditor(string message, string oldPart1, string oldPart2, string newPart1, string newPart2){
        if(message.Contains(oldPart1)){
            message = message.Replace(oldPart1, newPart1);
        }
        if(message.Contains(oldPart2)){
            message = message.Replace(oldPart2, newPart2);
        }
        return message;
    } 

    public string GetGreetMessage(int playerIndex){
        string message = greetMessages[UnityEngine.Random.Range(0, greetMessages.Length)];
        return StringEditor(message, "$index", playerIndex.ToString());
    }

    public string GetPauseMessage(int playerIndex){
        string message = pauseMessages[UnityEngine.Random.Range(0, pauseMessages.Length)];
        return StringEditor(message, "$index", playerIndex.ToString());
    }

    public string GetKillMessage(int killerPlayerIndex, int deadPlayerIndex){
        string message = killMessages[UnityEngine.Random.Range(0, killMessages.Length)];
        return StringEditor(message, "$killer", "$dead", killerPlayerIndex.ToString(), deadPlayerIndex.ToString());
    }

    public string GetPlayerVictoryMessage(int victoriusPlayerIndex){
        string message = playerVictoryMessages[UnityEngine.Random.Range(0, playerVictoryMessages.Length)];
        return StringEditor(message, "$index", victoriusPlayerIndex.ToString());
    }
}
