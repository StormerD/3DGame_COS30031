using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ForgeItemListing : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text priceText;
    public event Action<string, CurrencyValues> OnTryPurchaseWeapon;
    public event Action<string> OnEquipWeapon;
    // purchased, unlocked, equipped
    public event Action<bool, bool, bool> OnWeaponStateChanged;
    // true == hovered, false == not hovered
    public event Action<bool> OnHoverChanged;
    public event Action ClickedLockedWeapon;

    [Tooltip("This MUST be the same ID as the weapon ID set in the Weapon's WeaponData ScriptableObject.")]
    public WeaponPurchaseData weaponListing;
    public CurrencyValues purchasePrice;
    public GameObject unlockedVersion;
    public GameObject lockedVersion;

    private bool _isEquipped = false;
    private bool _isForgeOpen = false;

    void Awake()
    {
        priceText.text = purchasePrice.ToDisplayString();
        unlockedVersion.SetActive(weaponListing.isUnlocked);
        lockedVersion.SetActive(!weaponListing.isUnlocked);
    }

    void Start()
    {
        ForgeManager.instance.OnListingPurchaseStateChange += SomeWeaponPurchased;
        ForgeManager.instance.OnListingEquipped += SomeWeaponEquipped;
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
        
        // only play equipping sound when forge is open.
        if (_isForgeOpen) AudioManager.Instance.PlayEquipItemSound();
        EmitWeaponStateChanged();
    }

    private void SomeWeaponPurchased(string whichId, bool wasPurchased)
    {
        if (whichId != weaponListing.weaponId) return;
        weaponListing.isPurchased = wasPurchased;

        EmitWeaponStateChanged();
    }

    private void EmitWeaponStateChanged() => OnWeaponStateChanged?.Invoke(weaponListing.isPurchased, weaponListing.isUnlocked, _isEquipped);

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (weaponListing.isPurchased) { OnEquipWeapon?.Invoke(weaponListing.weaponId); }
        else if (weaponListing.isUnlocked) OnTryPurchaseWeapon?.Invoke(weaponListing.weaponId, purchasePrice);
        else
        {
            Debug.Log("Clicked a locked weapon."); ClickedLockedWeapon?.Invoke();
            // Play error sound
            AudioManager.Instance.PlayPurchaseError();      // ✅ fail sound
        }
    }

    public void OnPointerEnter(PointerEventData ped) => OnHoverChanged?.Invoke(true);
    public void OnPointerExit(PointerEventData ped) => OnHoverChanged?.Invoke(false);
}