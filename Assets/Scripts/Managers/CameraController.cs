using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private Vector3 gizmoPos;
    private Vector3 gizmoPosMin;
    private Vector3 gizmoPosMax;

    [SerializeField] private Vector3 fixedOffset;

    public float smoothSpeed;

    public List<GameObject> objectsTracked = new List<GameObject>();


    public int currentPlayerIndex;
    public InputAction resetFocus;
    public InputAction focusOnPreviousPlayer;
    public InputAction focusOnNextPlayer;

    public InputAction zoomIn;
    public InputAction zoomOut;

    public InputAction hover;
    public Vector3 moveInput;
    public float moveSpeed = 10;

    public InputAction rotate;
    public Vector3 rotateInput;
    public float rotateSpeed = 30;

    public InputAction enableCanvas;
    public bool canvasEnabled = true;
    public Animator myAnimator;
    public GameObject altTitle;

    private void Start()
    {
        currentPlayerIndex = 0;
        CanvasManager.Instance.gameObject.SetActive(canvasEnabled);
        if (altTitle != null) altTitle.SetActive(!canvasEnabled);

        SetupSpecialActions();
        SubscribeToEvents();
    }

    private void FixedUpdate()
    {
        if (objectsTracked.Count == 1)
        {
            Vector3 desiredPosition = objectsTracked[0].transform.position + fixedOffset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
        else if (objectsTracked.Count > 1)
        {
            Vector3 desiredPosition = FindCentroid() + fixedOffset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition + FindDynamicOffset(), smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
            //gizmoPos = FindCentroid();
            //gizmoPosMin = FindMinPos();
            //gizmoPosMax = FindMaxPos();
        }
        else
        {
            Vector3 desiredPosition = new Vector3(0, 0, 0) + fixedOffset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
    }

    private void Update()
    {
        Vector3 newPosition = transform.position;
        newPosition.x += moveInput.x * moveSpeed * Time.deltaTime;
        newPosition.y += moveInput.y * moveSpeed * Time.deltaTime;
        newPosition.z += moveInput.z * moveSpeed * Time.deltaTime;

        transform.Rotate(rotateInput.x * rotateSpeed * Time.deltaTime, rotateInput.y * rotateSpeed * Time.deltaTime, rotateInput.z * rotateSpeed * Time.deltaTime); 
        
        transform.position = newPosition;
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void SetupSpecialActions()
    {
        resetFocus.performed += context => ResetFocus();
        focusOnPreviousPlayer.performed += context => FocusOnPreviousPlayer();
        focusOnNextPlayer.performed += context => FocusOnNextPlayer();

        zoomIn.performed += context => ZoomIn();
        zoomOut.performed += context => ZoomOut();

        hover.performed += MoveCamera;
        hover.canceled += StopMove;

        rotate.performed += RotateCamera;
        rotate.canceled += StopRotate;

        enableCanvas.performed += EnableCanvas;

        //resetFocus.Enable();
        //focusOnPreviousPlayer.Enable();
        //focusOnNextPlayer.Enable();
        //zoomIn.Enable();
        //zoomOut.Enable();
        //hover.Enable();
        //rotate.Enable();
        //enableCanvas.Enable();
    }


    private void ResetFocus()
    {
        ResetTrackedList();

        foreach (PlayerInput playerInput in GameManager.Instance.playerList)
        {
            AddCharacter(playerInput.transform.gameObject);
        }

        Vector3 eulerRotation = new Vector3(45f, 0f, 0f);
        transform.rotation = Quaternion.Euler(eulerRotation);
        fixedOffset = new Vector3(0, 5, -5);
    }

    private void FocusOnPreviousPlayer()
    {
        currentPlayerIndex -= 1;
        if (currentPlayerIndex == -1) currentPlayerIndex = GameManager.Instance.playerList.Count - 1;

        foreach (PlayerInput playerInput in GameManager.Instance.playerList)
        {
            UntrackPlayer(playerInput);
        }

        objectsTracked.Clear();

        AddCharacter(GameManager.Instance.playerList[currentPlayerIndex].gameObject);
    }

    private void FocusOnNextPlayer()
    {
        currentPlayerIndex += 1;
        if (currentPlayerIndex == GameManager.Instance.playerList.Count) currentPlayerIndex = 0;

        foreach (PlayerInput playerInput in GameManager.Instance.playerList)
        {
            UntrackPlayer(playerInput);
        }

        objectsTracked.Clear();

        AddCharacter(GameManager.Instance.playerList[currentPlayerIndex].gameObject);
    }

    private void ZoomIn()
    {
        fixedOffset.y--;
        fixedOffset.z++;
    }

    private void ZoomOut()
    {
        fixedOffset.y++;
        fixedOffset.z--;
    }

    private void MoveCamera(InputAction.CallbackContext context)
    {
        Debug.Log("Move");

        if (objectsTracked.Count > 0)
        {
            foreach (PlayerInput playerInput in GameManager.Instance.playerList)
            {
                UntrackPlayer(playerInput);
            }

            objectsTracked.Clear();
        }

        moveInput = context.ReadValue<Vector3>();
    }

    private void StopMove(InputAction.CallbackContext context)
    {
        moveInput = new Vector3(0, 0, 0);
    }

    private void RotateCamera(InputAction.CallbackContext context)
    {
        Debug.Log("Rotate");

        if (objectsTracked.Count > 0)
        {
            foreach (PlayerInput playerInput in GameManager.Instance.playerList)
            {
                UntrackPlayer(playerInput);
            }

            objectsTracked.Clear();
        }

        rotateInput = context.ReadValue<Vector3>();
    }

    private void StopRotate(InputAction.CallbackContext context)
    {
        rotateInput = new Vector3(0, 0, 0);
    }

    private void EnableCanvas(InputAction.CallbackContext context)
    {
        canvasEnabled = !canvasEnabled;

        CanvasManager.Instance.gameObject.SetActive(canvasEnabled);
        if (altTitle != null) altTitle.SetActive(!canvasEnabled);
        if (myAnimator != null) myAnimator.SetBool("Travel", !canvasEnabled);
    }



    private void SubscribeToEvents()
    {
        GameManager.Instance.OnPlayerJoinedGame += TrackPlayer;
        GameManager.Instance.OnPlayerLeftGame += UntrackPlayer;

        MiniGameManager.OnPlayerWins += TrackWinner;
    }

    private void UnsubscribeFromEvents()
    {
        GameManager.Instance.OnPlayerJoinedGame -= TrackPlayer;
        GameManager.Instance.OnPlayerLeftGame -= UntrackPlayer;

        MiniGameManager.OnPlayerWins -= TrackWinner;
    }


    public void ResetTrackedList()
    {
        foreach (PlayerInput playerInput in GameManager.Instance.playerList)
        {
            UntrackPlayer(playerInput);
        }

        objectsTracked.Clear();

        foreach (PlayerInput playerInput in GameManager.Instance.playerList)
        {
            TrackPlayer(playerInput);
        }
    }

    private void TrackWinner(PlayerInput _playerInput)
    {
        foreach (PlayerInput playerInput in GameManager.Instance.playerList)
        {
            UntrackPlayer(playerInput);
        }

        objectsTracked.Clear();

        AddCharacter(_playerInput.transform.gameObject);
    }

    private void TrackPlayer(PlayerInput playerInput)
    {
        playerInput.TryGetComponent(out CharacterManager _characterManager);

        _characterManager.OnPlayerBorn += AddCharacter;
        _characterManager.OnPlayerDied += RemoveCharacter;
    }

    private void UntrackPlayer(PlayerInput playerInput)
    {
        playerInput.TryGetComponent(out CharacterManager _characterManager);

        _characterManager.OnPlayerBorn -= AddCharacter;
        _characterManager.OnPlayerDied -= RemoveCharacter;

        RemoveCharacter(_characterManager.transform.gameObject);
    }

    private void AddCharacter(GameObject character)
    {
        objectsTracked.Add(character);
    }

    private void RemoveCharacter(GameObject character)
    {
        objectsTracked.Remove(character);
    }

    Vector3 FindCentroid(){
        Vector3 centerPos = new Vector3(0, 0, 0);
        foreach(var player in objectsTracked){
            centerPos += player.transform.position;
        }
        centerPos /= objectsTracked.Count;

        return centerPos;
    }

    Vector3 FindMinPos(){
        Vector3 minPos = new Vector3(0, 0, 0);
        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float minZ = float.MaxValue;
        foreach(var player in objectsTracked){
            if(player.transform.position.x < minX){
                minX = player.transform.position.x;
            }
            if(player.transform.position.y < minY){
                minY = player.transform.position.y;
            }
            if(player.transform.position.z < minZ){
                minZ = player.transform.position.z;
            }
        }
        return new Vector3(minX, minY, minZ);
    }

    Vector3 FindMaxPos(){
        Vector3 maxPos = new Vector3(0, 0, 0);
        float maxX = float.MinValue;
        float maxY = float.MinValue;
        float maxZ = float.MinValue;
        foreach(var player in objectsTracked){
            if(player.transform.position.x > maxX){
                maxX = player.transform.position.x;
            }
            if(player.transform.position.y > maxY){
                maxY = player.transform.position.y;
            }
            if(player.transform.position.z > maxZ){
                maxZ = player.transform.position.z;
            }
        }
        return new Vector3(maxX, maxY, maxZ);
    }

    Vector3 FindDynamicOffset()
    {
        float distanceX, distanceZ;
        
        distanceX = FindMaxPos().x - FindMinPos().x;
        distanceZ = FindMaxPos().z - FindMinPos().z;
        return new Vector3(0, (distanceX + distanceZ) * 0.35f, (distanceX + distanceZ) * -0.35f);
    }

    private void OnDrawGizmos(){
        Gizmos.DrawCube(gizmoPos, new Vector3(1, 1, 1));
        Gizmos.DrawCube(gizmoPosMin, new Vector3(1, 1, 1));
        Gizmos.DrawCube(gizmoPosMax, new Vector3(1, 1, 1));
    }
}
