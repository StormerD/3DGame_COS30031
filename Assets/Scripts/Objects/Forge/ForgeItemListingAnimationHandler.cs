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

    void EquipAnimation() => _animator.SetTrigger("Equipped");
    void UnequipAnimation() => _animator.SetTrigger("Unequipped");
    void PurchaseAnimation() => _animator.SetTrigger("Purchased");
    void UnpurchaseAnimation() => _animator.SetTrigger("Unpurchased");
    void Hovered(bool isHovered) => _animator.SetBool("Hovered", isHovered);
}
