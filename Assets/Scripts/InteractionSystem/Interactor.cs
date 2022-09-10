using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour{
    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private float _interactionPointRadius;
    [SerializeField] private LayerMask _interactableMask;
    [SerializeField] private InteractionPromptUI _interactionPromptUI;

    private readonly Collider[] _collider = new Collider[3];
    [SerializeField] private int _numFound;

    private InteractorInterface _interactable;

    private void Awake() {
        _interactionPoint = gameObject.transform;
    }

    private void Start(){
        SubscribeToEvents();
    }

    private void SubscribeToEvents(){
        //INPUT EVENTS
        gameObject.transform.parent.GetComponent<PlayerInputHandler>().OnCharacterInteractWithObject += OnInteractWithObject;
    }

    private void Update(){
        _numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionPointRadius, _collider, _interactableMask);
        
        if (_numFound > 0){
            _interactable = _collider[0].GetComponent<InteractorInterface>();
            if (_interactable != null){
                if (!_interactionPromptUI.isDisplayed) {
                    _interactionPromptUI.SetUp(_interactable.InteractionPromp);
                }
            }
        }
        else{
            if (_interactable != null) _interactable = null;
            if (_interactionPromptUI.isDisplayed) _interactionPromptUI.ClosePanel();
        }
    }

    //public void KeyIsPressed(float context){
    //    if(context == 1f && _interactable != null){
    //        //Debug.Log("Input detected");
    //        _interactable.Interact(this);
    //    }
    //}

    public void OnInteractWithObject(InputAction.CallbackContext context){
        if(context.performed && _interactable != null){
            //interactor.KeyIsPressed(context.ReadValue<float>());
            _interactable.Interact(this);
        }
    }

    private void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_interactionPoint.position, _interactionPointRadius);
    }
}
