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

    [SerializeField] private InteractorInterface _interactable;

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
                //if (!_interactionPromptUI.isDisplayed) {
                //    _interactionPromptUI.SetUp(_interactable.InteractionPromp);
                //}
                if(!_interactable.PromptUI.isDisplayed){
                    _interactable.PromptUI.OpenPanel();
                }
            }
        }
        else{
            if(_interactable != null){
                _interactable.PromptUI.ClosePanel();
                _interactable = null;
            }
            //if (_interactable != null) _interactable = null;
            //if (_interactionPromptUI.isDisplayed) _interactionPromptUI.ClosePanel();
        }
    }

    public void OnInteractWithObject(InputAction.CallbackContext context){
        if(context.performed && _interactable != null){
            _interactable.Interact(this);
        }
    }

    private void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_interactionPoint.position, _interactionPointRadius);
    }
}
