using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

public enum Animal{hedgehog, pangolin, threeBandedArmadillo}

public enum CharacterState { Alive, Dead }

public class CharacterManager : MonoBehaviour{
    #region "COMPONENTS"
    [Space(5)]
    [Header("Components")]
    [SerializeField] public PlayerInput playerInput;
    [SerializeField] public PlayerInputHandler playerInputHandler;
    [SerializeField] public CharacterHealthSystem characterHealthSystem;
    [SerializeField] public MovementSystem characterMovementSystem;
    [SerializeField] public CharacterInventory characterInventory;
    [SerializeField] public CharacterWeaponSystem characterWeaponSystem;
    [SerializeField] public Interactor characterInteractor;
    [SerializeField] public CharacterDisplay characterItemsDisplay;
    #endregion "COMPONENTS"

    [SerializeField] public CharacterStatsScriptableObject Character;
    [SerializeField] public CharacterSkin characterSkin;
    [SerializeField] public GameObject characterObject;
    [SerializeField] public GameObject characterTombstone;
    [SerializeField] private Color[] lightColors;
    [SerializeField] private Color[] UIColors;

    #region "STATS"
    [Space(5)]
    [Header("Stats")]
    public Device playerDevice;
    public Color lightColor;
    public Color UIColor;
    public CharacterState characterState;
    public int score;
    public int kills;
    public int deaths;
    public bool unlimitedLives;
    public int totalLives;
    public float timeToRespawn;
    public bool actionsAreBlocked;
    #endregion "STATS"

    #region "EVENTS"
    public event Action<CharacterState> OnCharacterStateChanged;
    public event Action<GameObject, int> OnPlayerScoreChanged;
    public event Action<GameObject> OnPlayerScoredKill;
    public event Action<GameObject> OnPlayerDied;
    public event Action<GameObject> OnPlayerBorn;
    public event Action<float> OnPlayerWasDamaged;
    public event Action<float> OnPlayerWasHealed;
    public event Action<float, float> OnPlayerHealthUpdated;
    public event Action<Weapon> OnPlayerShotWeapon; 
    public event Action<Weapon> OnPlayerReloadedWeapon;
    public event Action<Weapon> OnPlayerPickedUpWeapon;
    public event Action OnPlayerDroppedWeapon;
    public event Action OnPlayerStatsReset;
    public event Action OnCharacterChosen;
    #endregion "EVENTS"

    #region "Unity Callbacks"
    private void Awake()
    {
        InitializeComponents();
        InitializePlayerVariables();
        SetupControllerLights();
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (IsBlocked()) return;

        if (other.gameObject.GetComponent<Item>() != null && other.gameObject.GetComponent<Item>().CanBePickedUp)
        {
            //Debug.Log("Collided");
            if (other.gameObject.GetComponent<Coin>())
            {
                if (characterInventory.AddToInventory(other.gameObject.GetComponent<Item>().item))
                {
                    other.GetComponent<Coin>().PickedUp(this.gameObject);
                }
            }

            if (other.gameObject.GetComponent<Food>())
            {
                other.GetComponent<Food>().PickedUp(this.gameObject);
            }

            if (other.gameObject.GetComponent<Weapon>() && other.gameObject.GetComponent<Weapon>().ammo > 0 && other.gameObject.GetComponent<Weapon>().totalAmmo > 0)
            {
                if (!IsArmed())
                {
                    characterInventory.PickWeapon(other.gameObject);
                }
            }
        }
    }
    #endregion "Unity Callbacks"

    private void InitializeComponents(){
        characterObject = null;
        characterTombstone = null;

        playerInput = GetComponent<PlayerInput>();
        playerInputHandler = GetComponent<PlayerInputHandler>();

        characterHealthSystem = GetComponent<CharacterHealthSystem>();
        characterMovementSystem = GetComponent<MovementSystem>();
        characterInventory = GetComponent<CharacterInventory>();
        characterWeaponSystem = GetComponent<CharacterWeaponSystem>();
        characterInteractor = GetComponent<Interactor>();
    }

    private void InitializePlayerVariables(){
        lightColor = lightColors[playerInput.playerIndex];
        UIColor = UIColors[playerInput.playerIndex];

        characterState = CharacterState.Alive;

        score = 0;
        kills = 0;
        deaths = 0;
        unlimitedLives = true;
        totalLives = 0;
        timeToRespawn = 3f;

        actionsAreBlocked = false;
    }

    private void SetupControllerLights()
    {
        var device = playerInput.devices[0];

        if (device.GetType() == typeof(DualShock4GamepadHID))
        {
            playerDevice = Device.DualShock;
            DualShock4GamepadHID dualshock4 = (DualShock4GamepadHID)device;
            dualshock4.SetLightBarColor(lightColor);
        }
        else if (device.GetType() == typeof(DualSenseGamepadHID))
        {
            playerDevice = Device.DualShock;
            DualSenseGamepadHID dualsense = (DualSenseGamepadHID)device;
            dualsense.SetLightBarColor(lightColor);
        }
        else
        {
            playerDevice = Device.Keyboard;
        }
    }

    private void SubscribeToEvents()
    {
        GameManager.Instance.OnGameStateChanged += AdaptToGameState;
    }

    private void UnsubscribeFromEvents()
    {
        GameManager.Instance.OnGameStateChanged += AdaptToGameState;
    }

    private void AdaptToGameState(GameState gameState)
    {
        if (gameState == GameState.Paused)
        {
            playerInputHandler.PlayerOpenedMenu();
        }
        else
        {
            playerInputHandler.PlayerClosedMenu();
        }
    }


    public void UpdateCharacterState(CharacterState _characterState)
    {
        characterState = _characterState;

        OnCharacterStateChanged?.Invoke(characterState);
    }

    public void ResetStats()
    {
        InitializePlayerVariables();
    }

    public void SpawnCharacter(CharacterStatsScriptableObject _character)
    {
        Character = _character;

        ReplaceCharacter();

        characterTombstone = _character.tombstone;

        characterHealthSystem.Initialize();
        characterMovementSystem.Initialize();

        transform.parent = GameManager.Instance.transform;

        playerInput.SwitchCurrentActionMap("Player");
    }

    public void ReplaceCharacter()
    {
        if(characterObject != null) Destroy(characterObject);

        int index = (int)MiniGameManager.Instance.miniGame + 1;

        int skinsCount = Character.characterSkin.Length;

        if (index > skinsCount) characterSkin = Character.characterSkin[0];
        else characterSkin = Character.characterSkin[index];

        characterObject = Instantiate(characterSkin.characterModel, transform.position, transform.rotation, transform);
        characterItemsDisplay = characterObject.GetComponent<CharacterDisplay>();
        characterWeaponSystem.SetGunPosition(characterItemsDisplay.gunPosition);

        OnCharacterChosen?.Invoke();
    }

    public void RefreshStatsUponRespawning()
    {
        characterHealthSystem.Initialize();
        characterMovementSystem.Initialize();
    }


    public void BeginRespawnProcess()
    {
        if(CanRespawn())
        {
            StartCoroutine(RespawnCharacterDelay());
            IEnumerator RespawnCharacterDelay()
            {
                yield return new WaitForSeconds(timeToRespawn);
                RespawnCharacter();
            }
        }
        characterObject.SetActive(false);
        BlockActions();
    }

    public void RespawnCharacter()
    {
        LevelManager.Instance.currentLevel.SpawnPlayerRandomly(playerInput);

        characterObject.SetActive(true);
        UnblockActions();
        RefreshStatsUponRespawning();
    }

    public void BlockActions(){
        actionsAreBlocked = true;
        playerInputHandler.DisableActions();
    }

    public void UnblockActions(){
        actionsAreBlocked = false;
        playerInputHandler.RestoreActions();
    }





    public void IncreaseLives(int _amount){
        totalLives += _amount;
    }

    public void SetUnlimitedLives(){
        totalLives = -1;
        unlimitedLives = true;
    }

    public bool IsBlocked(){
        return actionsAreBlocked;
    }

    public bool CanRespawn(){
        if(totalLives > 0 || unlimitedLives){
            return true;
        }
        else{
            return false;
        }
    }

    public bool CanMove(){
        if(actionsAreBlocked) return false;
        if(GameManager.Instance.GameState == GameState.Paused) return false;
        
        return true;
    }

    public bool CanUseFireGun(){
        if(actionsAreBlocked) return false;
        if(!IsArmed()) return false;

        return true;
    }

    public bool IsArmed(){
        return characterInventory.IsArmed();
    }

    #region "Methods that invoke events"
    public void FullReset(){
        //reset all base stats
        //clear inventory
        //move player to default spawnpoint
        //set character active
        //unblock actions
        //reset health and movement systems
        //send that to the UI
        characterInventory.ClearInventory();
        ResetStats();

        //this.transform.position = GameManager.Instance.spawnPoints[0].transform.position;
        LevelManager.Instance.currentLevel.SpawnPlayerRandomly(playerInput);

        characterObject.SetActive(true);
        UnblockActions();
        characterHealthSystem.Initialize();
        characterMovementSystem.Initialize();
        OnPlayerStatsReset?.Invoke();
    }

    public void SetLimitedLives(int _amount){
        totalLives = _amount;
        unlimitedLives = false;
        OnPlayerStatsReset?.Invoke();
    }

    public void IncreaseScore(int value){
        score += value;
        OnPlayerScoreChanged?.Invoke(gameObject, score);
    }

    public void DecreaseScore(int value){
        score = Math.Max(score -= value, 0);
    }

    public void PlayerScoredKill(GameObject character){
        kills++;

        InvokeOnPlayerScoredKill(character);
    }

    public void PlayerDied(GameObject character){
        deaths++;

        if (!unlimitedLives) totalLives--;

        BeginRespawnProcess();

        characterInventory.DropAllInventory();

        GameObject tombstone = Instantiate(characterTombstone, character.transform.position, Quaternion.Euler(0, 0, 0));
        Destroy(tombstone, timeToRespawn);

        InvokeOnPlayerDied(character);
    }

    public void InvokeOnPlayerScoredKill(GameObject character)
    {
        OnPlayerScoredKill?.Invoke(character);
    }

    public void InvokeOnPlayerDied(GameObject character)
    {
        OnPlayerDied?.Invoke(character);
    }

    public void InvokeOnPlayerBorn(GameObject character){
        OnPlayerBorn?.Invoke(character);
    }

    public void InvokeOnPlayerWasDamaged(float _damageAmount){
        OnPlayerWasDamaged?.Invoke(_damageAmount);
    }

    public void InvokeOnPlayerWasHealed(float _healAmount){
        OnPlayerWasHealed?.Invoke(_healAmount);
    }

    public void InvokeOnPlayerHealthUpdated(float _currentHealth, float _maxHealth){
        OnPlayerHealthUpdated?.Invoke(_currentHealth, _maxHealth);
    }

    public void InvokeOnPlayerShotWeapon(Weapon _weapon){
        OnPlayerShotWeapon?.Invoke(_weapon);
    }

    public void InvokeOnPlayerReloadedWeapon(Weapon _weapon){
        OnPlayerReloadedWeapon?.Invoke(_weapon);
    }

    public void InvokeOnPlayerPickedUpWeapon(Weapon _weapon){
        OnPlayerPickedUpWeapon?.Invoke(_weapon);
    }

    public void InvokeOnPlayerDroppedWeapon(){
        OnPlayerDroppedWeapon?.Invoke();
    }
    #endregion "Methods that invoke events"
}
