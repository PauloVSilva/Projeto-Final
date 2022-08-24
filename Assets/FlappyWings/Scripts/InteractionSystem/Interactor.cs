using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour{
    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private float _interactionPointRadius;
    [SerializeField] private LayerMask _interactableMask;
    private readonly Collider[] _collider = new Collider[3];
    [SerializeField] private int _numFound;

    private void Update(){
        _numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionPointRadius, _collider, _interactableMask);
    }

    public void KeyIsPressed(float context){
        if (_numFound < 1) return;

        var interactable = _collider[0].GetComponent<InteractorInterface>();
        if (interactable == null) return;

        //if(Keyboard.current.eKey.wasPressedThisFrame){
        if(context == 1f){
            Debug.Log("Input detected");
            interactable.Interact(this);
        }
    }

    private void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_interactionPoint.position, _interactionPointRadius);
    }
}
