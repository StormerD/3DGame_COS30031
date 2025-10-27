using UnityEngine;

[RequireComponent(typeof(PlayerLooter), typeof(PlayerWeaponHandlerBase))]
public class PlayerDataTracker : MonoBehaviour
{
    // Players "register" with the game manager when they Start(). If another player already exists, the one with
    // the higher registrationPriority will get kept while the other one will get disabled.
    public int registrationPriority = 0;
    private static bool exists = false;
    private PlayerLooter looter;
    private PlayerWeaponHandlerBase weaponHandler;
    
    void Awake()
    {
        if (!exists) DontDestroyOnLoad(gameObject); // the first player created will persist across scenes
        else exists = true;

        looter = GetComponent<PlayerLooter>();
        weaponHandler = GetComponent<PlayerWeaponHandlerBase>();
    }

    void OnEnable()
    {
        if (GameManager.instance == null) Debug.LogWarning("GameManager is null; unable to register player.");
        else
        {
            GameManager.instance.RegisterPlayer(this);
        }
    }

    void OnDestroy()
    {
        if (GameManager.instance == null) Debug.LogWarning("GameManager is null; unable to unregister player.");
        else
        {
            GameManager.instance.UnregisterPlayer(this);
        }
    }

    public CurrencyValues GetSaveableCurrency() => looter.GetSaveableCurrency();
    public string GetEquippedWeapon() => weaponHandler.GetEquippedWeapon();
}