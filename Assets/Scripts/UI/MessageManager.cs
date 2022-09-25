using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageManager : MonoBehaviour{
    public static MessageManager instance;
    [SerializeField] private string[] greetMessages;
    [SerializeField] private string[] pauseMessages;
    [SerializeField] private string[] killMessages;

    private void Awake(){
        if(instance == null){
            instance = this;
        }
        else{
            Destroy(this);
        }
    }

    public string StringEditor(string originalMessage, string oldPart, string newPart){
        string newMessage = originalMessage;
        if(newMessage.Contains(oldPart)){
            newMessage = newMessage.Replace(oldPart, newPart);
            return newMessage;
        }
        return originalMessage;
    }

    public string StringEditor(string originalMessage, string oldPart1, string oldPart2, string newPart1, string newPart2){
        string newMessage = originalMessage;
        if(newMessage.Contains(oldPart1) && newMessage.Contains(oldPart2)){
            newMessage = newMessage.Replace(oldPart1, newPart1);
            newMessage = newMessage.Replace(oldPart2, newPart2);
            return newMessage;
        }
        return originalMessage;
    } 

    public string GetGreetMessage(int playerIndex){
        string message = greetMessages[UnityEngine.Random.Range(0, greetMessages.Length - 1)];
        return StringEditor(message, "$index", playerIndex.ToString());
    }

    public string GetPauseMessage(int playerIndex){
        string message = pauseMessages[UnityEngine.Random.Range(0, greetMessages.Length - 1)];
        return StringEditor(message, "$index", playerIndex.ToString());
    }

    public string GetKillMessage(int killerPlayerIndex, int deadPlayerIndex){
        string message = killMessages[UnityEngine.Random.Range(0, greetMessages.Length - 1)];
        return StringEditor(message, "$killer", "$dead", killerPlayerIndex.ToString(), deadPlayerIndex.ToString());
    }
}
