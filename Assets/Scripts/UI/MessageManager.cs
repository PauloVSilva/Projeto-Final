using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageManager : MonoBehaviour{
    public static MessageManager instance;
    [SerializeField] private string[] greetMessages;
    [SerializeField] private string[] pauseMessages;

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
            return newMessage.Replace(oldPart, newPart);
        }
        return originalMessage;
    }

    public string GetGreetMessage(string playerIndex){
        string newMessage = greetMessages[UnityEngine.Random.Range(0, greetMessages.Length - 1)];
        return StringEditor(newMessage, "$index", playerIndex);
    }

    public string GetPauseMessage(string playerIndex){
        string newMessage = pauseMessages[UnityEngine.Random.Range(0, greetMessages.Length - 1)];
        return StringEditor(newMessage, "$index", playerIndex);
    }
}
