using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface InteractorInterface{
    public string PromptString {get;}
    public bool Interact(Interactor interactor);
}
