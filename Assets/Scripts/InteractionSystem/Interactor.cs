using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour{
    public CharacterManager characterManager;
    private InteractionPromptUI interactionPromptUI;
    private InteractorInterface interactable;
    [SerializeField] private Transform interactionPoint;
    [SerializeField] private float interactionPointRadius;
    [SerializeField] private LayerMask interactableMask;

    private readonly Collider[] _collider = new Collider[3];
    [SerializeField] private int numFound;


    private void Start(){
        characterManager = GetComponent<CharacterManager>();
        interactionPromptUI = GetComponentInChildren<InteractionPromptUI>();
        interactionPoint = gameObject.transform;
        GetComponent<PlayerInputHandler>().OnCharacterInteractWithObject += OnInteractWithObject;
    }

    private void Update(){
        numFound = Physics.OverlapSphereNonAlloc(interactionPoint.position, interactionPointRadius, _collider, interactableMask);
        
        if (numFound > 0){
            interactable = _collider[0].GetComponent<InteractorInterface>();
            if (interactable != null && !interactionPromptUI.isDisplayed){
                interactionPromptUI.SetPrompt(characterManager.playerInput, interactable.PromptString);
                interactionPromptUI.OpenPanel();
            }
        }
        else{
            if(interactable != null){
                interactable = null;
                interactionPromptUI.ClosePanel();
                interactionPromptUI.ClearPrompt();
            }
        }
    }

    public void OnInteractWithObject(InputAction.CallbackContext context){
        if(context.performed && interactable != null){
            interactable.Interact(this);
        }
    }

    private void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(interactionPoint.position, interactionPointRadius);
    }
}
