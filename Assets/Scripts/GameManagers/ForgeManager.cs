using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// this manager handles the listings on the Forge UI as well as any connections that the
/// forge system needs to make with other systems (currency, weapon equipping, saving / loading data)
/// it doesn't need to be in the scene unless there is a Forge object—the Forge object
/// still needs the Forge.cs script on it, it will call into ForgeManager when needed
/// </summary>
public class ForgeManager : MonoBehaviour
{
    public static ForgeManager instance;

    [Tooltip("Ensure these GameObjects have an IWeapon script attached, with WeaponData correctly set in the IWeapon script")]
    public List<GameObject> availableWeapons;
    public GameObject player;
    [Tooltip("These should be UI elements (like images!) that have an attached ForgeItemButtonScript.")]
    public List<GameObject> forgeWeaponListingPrefabs;
    [Tooltip("This should be the reference to the entire ForgeMenu canvas object that will be turned on / off.")]
    public GameObject forgeMenu;
    [Tooltip("This should be the actual scrollbox (part of the forge menu) where the forge purchase listings will go.")]
    public RectTransform forgeMenuScrollbox;
    public ObjectiveManager objectiveManager;

    public event Action<string, bool> OnListingPurchaseStateChange; // <string> is the weaponId purchased, bool is true if purchased, false if not
    public event Action<string, bool> OnListingUnlockedStateChange;
    public event Action<string> OnListingEquipped; // same as above.

    private Dictionary<string, GameObject> _weaponsById = new();
    private List<WeaponPurchaseData> _purchaseData;
    private ILooter _playerLooter;
    private Animator _forgeMenuAnimator;
    private bool _isInitializing = false;
    public bool IsInitializing => _isInitializing;



    void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;
        foreach (GameObject w in availableWeapons)
        {
            string weaponId = w.GetComponent<IWeapon>().GetWeaponData().weaponId;
            if (!_weaponsById.ContainsKey(weaponId)) _weaponsById.Add(weaponId, w);
        }
        if (!player.TryGetComponent<ILooter>(out _playerLooter)) Debug.LogWarning("ForgeManager player needs an ILooter component to purchase weapons!");
        if (!forgeMenu.TryGetComponent(out _forgeMenuAnimator)) Debug.LogWarning("ForgeMenu missing animator.");
    }

    void Start()
    {
        SaveManager.instance.OnSaveDataChanged += ReloadWeaponPurchaseData;
        ReloadWeaponPurchaseData();
        if (_purchaseData.Count == 0) _purchaseData = GetDefaultWeaponPurchaseData();

        for (int i = 0; i < forgeMenuScrollbox.childCount; i++)
        {
            if (!forgeMenuScrollbox.GetChild(i).TryGetComponent(out ForgeItemListing listing))
                Debug.LogWarning("Forge content box child " + i + " is missing ForgeItemListing.");
            else
            {
                listing.OnEquipWeapon += EquipItem;
                listing.OnTryPurchaseWeapon += PurchaseItem;
            }
        }
    }

    public void OpenForgeMenu()
    {
        forgeMenu.SetActive(true);

        // Hide the objective arrow while forge menu is open
        if (objectiveManager != null && objectiveManager.arrow != null)
            objectiveManager.arrow.gameObject.SetActive(false);
        _forgeMenuAnimator.SetTrigger("OpenMenu");}


    public void CloseForgeMenu()
    {
        _forgeMenuAnimator.SetTrigger("CloseMenu");
        if (objectiveManager != null)
        objectiveManager.SetNextObjective();
    }

    public GameObject GetWeaponByID(string id)
    {
        if (id != null && _weaponsById.ContainsKey(id)) return _weaponsById[id];
        Debug.LogWarning("Requested weapon: " + id + ", not found!");
        return null;
    }

    public List<WeaponPurchaseData> GetWeaponPurchaseData() => _purchaseData;

    public void PurchaseItem(string id, PurchasePrice price)
    {
        if (_isInitializing)
        return; // ✅ skip accidental purchases during load

        if (!DoesListingSatisfy(id, (i) => i.isUnlocked))
        {
            Debug.LogWarning("Trying to equip weapon " + id + " but it is locked.");
            return;

        }
        if (_playerLooter.UseCurrency(price))
        {
            SetListingPurchased(id);
            OnListingPurchaseStateChange?.Invoke(id, true);
            EquipItem(id);
            AudioManager.Instance.PlayPurchaseSuccess();

        }
        else
    {
        Debug.LogWarning("Not enough currency to buy " + id);
        AudioManager.Instance.PlayPurchaseError();
    }

    }
    public void EquipItem(string id)
    {
        // Double check that listing is purchased & unlocked
        if (!DoesListingSatisfy(id, (i) => i.isPurchased && i.isUnlocked))
        {
            Debug.LogWarning("Trying to equip weapon " + id + " but it is either locked or unpurchased.");
            return;
        }
        Debug.Log("Equipping item " + id);
        OnListingEquipped?.Invoke(id);
    }
    // given a test to perform on a WeaponPurchaseData, return true if ID satisfies
    private bool DoesListingSatisfy(string id, Predicate<WeaponPurchaseData> test)
    {
        foreach (WeaponPurchaseData listing in _purchaseData)
        {
            if (listing.weaponId != id) continue;
            return test(listing);
        }
        return false; // auto-fail if weapon doesn't exist at all
    }

    // a little dangerous to use ! here (which asserts non-null) but since theoretically all weapons being purchased
    // should have a clear id (and hence call into this function with that defined id) it should be ok
    private void SetListingPurchased(string id) => _purchaseData.Find(l => l.weaponId == id)!.isPurchased = true;
    private void SetListingUnlocked(string id) => _purchaseData.Find(l => l.weaponId == id)!.isUnlocked = true;
    private void ReloadWeaponPurchaseData()
    {
        _isInitializing = true; // ✅ prevent sounds during data load
        _purchaseData = SaveManager.instance.GetWeaponsPurchased() ?? GetDefaultWeaponPurchaseData();

        // go through and send message out for each ForgeListing to update its data
        foreach (WeaponPurchaseData wpd in _purchaseData)
        {
            OnListingUnlockedStateChange?.Invoke(wpd.weaponId, wpd.isUnlocked);
            OnListingPurchaseStateChange?.Invoke(wpd.weaponId, wpd.isPurchased);
        }

        // also emit a weapon is equipped if necessary
        string equippedWeapon = SaveManager.instance.GetEquippedWeapon() ?? "";
        if (equippedWeapon.Length != 0) OnListingEquipped?.Invoke(equippedWeapon);
        _isInitializing = false; // ✅ re-enable sounds after data load
    }
    // this is not the best option here to just have a hardset default weapon data for a few reasons...
    // the worst of which is that the weapon ids for melee1 and ranged1 are hardcoded. if we ever change those
    // but forget to update it here it would make us very confused
    private List<WeaponPurchaseData> GetDefaultWeaponPurchaseData()
    {
        List<WeaponPurchaseData> l = new()
        {
            new WeaponPurchaseData { weaponId = "melee1", isPurchased = true, isUnlocked = true },
            new WeaponPurchaseData { weaponId = "ranged1", isPurchased = false, isUnlocked = true}
        };
        foreach (string key in _weaponsById.Keys)
        {
            if (key == "melee1" || key == "ranged1") continue;
            l.Add(new WeaponPurchaseData { weaponId = key, isPurchased = false, isUnlocked = false });
        }
        return l;
    }
}