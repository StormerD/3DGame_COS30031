using UnityEngine;

[RequireComponent(typeof(PlayerLooter), typeof(PlayerWeaponHandlerBase))]
public class PlayerDataTracker : MonoBehaviour
{
    // Players "register" with the game manager when they Start(). If another player already exists, the one with
    // the higher registrationPriority will get kept while the other one will get disabled.
    public int registrationPriority = 0;
    [SerializeField] private bool persistAcrossScenes = true;
    private static bool exists = false;
    private PlayerLooter looter;
    private PlayerWeaponHandlerBase weaponHandler;
    private bool hasBeenDisabledAlready = false;
    private bool hasRegisteredAlready = false;
    
    void Awake()
    {
        if (!exists && persistAcrossScenes)
        {
            DontDestroyOnLoad(gameObject); // the first player that has persistAcrossScenes true will persist across scenes
            exists = true;
        }

        looter = GetComponent<PlayerLooter>();
        weaponHandler = GetComponent<PlayerWeaponHandlerBase>();
    }

    void OnEnable()
    {
        if (GameManager.instance == null) Debug.LogWarning("GameManager is null; unable to register player.");
        else if (!hasRegisteredAlready)
        {
            Debug.Log($"{gameObject.name} Registering");
            hasRegisteredAlready = true;
            GameManager.instance.RegisterPlayer(this);
        } else hasBeenDisabledAlready = false; // re-enabling after already registered, so we reset the disable flag
    }
    void OnDisable()
    {
        if (GameManager.instance == null) Debug.LogWarning("GameManager is null; unable to unregister player.");
        else if (hasBeenDisabledAlready || !persistAcrossScenes)
        {
            Debug.Log("Unregistering: " + gameObject.name);
            GameManager.instance.UnregisterPlayer(this);
        }
        else { Debug.Log("Disabled: " + gameObject.name); hasBeenDisabledAlready = true; }
    }

    public CurrencyValues GetSaveableCurrency() => looter.GetSaveableCurrency();
    public string GetEquippedWeapon()
    {
        return weaponHandler.GetEquippedWeapon() ?? "";
    }
}