using UnityEngine;

[RequireComponent(typeof(ForgeItemListing), typeof(Animator))]
public class ForgeItemListingAnimationHandler : MonoBehaviour
{
    private ForgeItemListing _forgeItemListing;
    private Animator _animator;

    private bool _isPurchased;
    private bool _isEquipped;

    void Awake()
    {
        _forgeItemListing = GetComponent<ForgeItemListing>();
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        _forgeItemListing.OnWeaponStateChanged += WeaponStateChanged;
        _forgeItemListing.OnHoverChanged += Hovered;
    }

    void WeaponStateChanged(bool purchaseState, bool unlockState, bool equipState)
    {
        if (equipState != _isEquipped)
        {
            _isEquipped = equipState;
            if (_isEquipped) EquipAnimation();
            else UnequipAnimation();
        }
        if (purchaseState != _isPurchased)
        {
            _isPurchased = purchaseState;
            if (_isPurchased) PurchaseAnimation();
            else UnpurchaseAnimation(); // this doesn't make a lot of sense at first glance but because saves can change purchase data we can technically "unpurchase" a weappon
        }
    }

    void EquipAnimation() { if (_animator != null) _animator.SetTrigger("Equipped"); }
    void UnequipAnimation() { if (_animator != null) _animator.SetTrigger("Unequipped"); }
    void PurchaseAnimation() { if (_animator != null) _animator.SetTrigger("Purchased"); }
    void UnpurchaseAnimation() { if (_animator != null) _animator.SetTrigger("Unpurchased"); }
    void Hovered(bool isHovered) { if (_animator != null) _animator.SetBool("Hovered", isHovered); } 
}
