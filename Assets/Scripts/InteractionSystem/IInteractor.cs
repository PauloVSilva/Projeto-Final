using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractor
{
    public string PromptString {get;}
    public bool Interact(Interactor interactor);
}
