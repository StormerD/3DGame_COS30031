using UnityEngine;

[RequireComponent(typeof(PlayerMovement2D))]
public class PlayerWeaponHandler2D : PlayerWeaponHandlerBase
{
    protected override void InitializeWeapon()
    {
        if (_equippedWeapon != null) return;

        // really only used for debugging - happens when you start the game within a level (in editor) 
        if (GameManager.instance == null && transform.childCount > 0)
        {
            foreach (Transform t in transform) if (t.TryGetComponent<WeaponBase>(out var _))
                {
                    _equippedWeapon = t.gameObject;
                    if (!_equippedWeapon.TryGetComponent(out _weaponScript)) Debug.LogWarning("Equipped weapon does not implement IWeapon interface");
                    break;
                }
        }
    }

    public override void EquipWeapon(GameObject to)
    {
        base.EquipWeapon(to);
        if (to != null && !_equippedWeapon.TryGetComponent(out _weaponScript)) Debug.LogWarning("Newly equipped weapon does not implement IWeapon interface");
    }
}