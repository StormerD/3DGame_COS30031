using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ForgeItemListing : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text priceText;
    public event Action<string> OnEquipWeapon;
    // purchased, unlocked, equipped
    public event Action<bool, bool, bool> OnWeaponStateChanged;
    // true == hovered, false == not hovered
    public event Action<bool> OnHoverChanged;
    public event Action<string> OnPurchaseWeapon;
    [SerializeField] private StringEventObject equippedWeaponStream;

    [Tooltip("This MUST be the same ID as the weapon ID set in the Weapon's WeaponData ScriptableObject.")]
    public WeaponPurchaseData weaponListing;
    public CurrencyValues purchasePrice;
    public GameObject unlockedVersion;
    public GameObject lockedVersion;

    [SerializeField] private PurchaseEventObject _playerCurrencyStream;
    [SerializeField] private PurchaseEventObject _playerCostsStream;
    private CurrencyValues _playerCurrency;
    private bool _isEquipped = false;
    private bool _isForgeOpen = false;

    void Awake()
    {
        priceText.text = purchasePrice.ToDisplayString();
        unlockedVersion.SetActive(weaponListing.isUnlocked);
        lockedVersion.SetActive(!weaponListing.isUnlocked);
    }

    private void OnEnable()
    {
        _playerCurrencyStream.RegisterListener(CurrencyUpdate);
        equippedWeaponStream.RegisterListener(SomeWeaponEquipped);
    }
    private void OnDisable()
    {
        _playerCurrencyStream.UnregisterListener(CurrencyUpdate);
        equippedWeaponStream.UnregisterListener(SomeWeaponEquipped);
    }
    private void CurrencyUpdate(CurrencyValues to) => _playerCurrency = to;

    void Start()
    {
        ForgeManager.instance.OnListingPurchaseStateChange += SomeWeaponPurchased;
        ForgeManager.instance.OnForgeOpened += SetForgeOpen;
        ForgeManager.instance.OnForgeClosed += SetForgeClosed;
    }

    private void SetForgeOpen() => _isForgeOpen = true;
    private void SetForgeClosed() => _isForgeOpen = false;

    // decided to go with a subscription + filter structure here, where all buttons
    // listen to the events from ForgeManager and only take action when necessary
    private void SomeWeaponEquipped(string whichId)
    {
        if (whichId != weaponListing.weaponId)
        {
            if (!_isEquipped) return;
            _isEquipped = false;
        }
        else _isEquipped = true;
        
        // only play equipping sound when forge is open
        if (_isForgeOpen) AudioManager.Instance.PlayEquipItemSound();
        EmitWeaponStateChanged();
    }

    private void SomeWeaponPurchased(string whichId, bool wasPurchased)
    {
        if (whichId != weaponListing.weaponId) return;
        weaponListing.isPurchased = wasPurchased;
        if (wasPurchased) priceText.text = "";

        EmitWeaponStateChanged();
    }

    private void EmitWeaponStateChanged() => OnWeaponStateChanged?.Invoke(weaponListing.isPurchased, weaponListing.isUnlocked, _isEquipped);

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (weaponListing.isPurchased) { OnEquipWeapon?.Invoke(weaponListing.weaponId); }
        else if (weaponListing.isUnlocked)
        {
            if (_playerCurrency.EnoughToPurchase(purchasePrice))
            {
                _playerCostsStream.RaiseEvent(purchasePrice);
                OnPurchaseWeapon?.Invoke(weaponListing.weaponId);
                AudioManager.Instance.PlayPurchaseSuccess();
            } else AudioManager.Instance.PlayPurchaseError();
        }
        else
        {
            Debug.Log("Clicked a locked weapon."); 
            AudioManager.Instance.PlayPurchaseError();
        }
    }

    public void OnPointerEnter(PointerEventData ped) => OnHoverChanged?.Invoke(true);
    public void OnPointerExit(PointerEventData ped) => OnHoverChanged?.Invoke(false);
}