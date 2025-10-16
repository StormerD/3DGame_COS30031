using UnityEngine;

[RequireComponent(typeof(PlayerMovement2D))]
public class PlayerWeaponHandler2D : PlayerWeaponHandlerBase
{
    protected override void InitializeWeapon()
    {
        if (_equippedWeapon != null) return;

        if (ActiveGameManager.instance != null && ActiveGameManager.instance.equippedWeapon != null)
        {
            _equippedWeapon = ActiveGameManager.instance.equippedWeapon;
            if (!_equippedWeapon.TryGetComponent(out _weaponScript)) Debug.LogWarning("Equipped weapon missing IWeapon interface");
            else
            {
                _equippedWeapon.transform.parent = transform;
                _equippedWeapon.transform.localPosition = Vector2.zero;
            }
        }
        if (ActiveGameManager.instance == null && transform.childCount > 0)
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
        if (!_equippedWeapon.TryGetComponent(out _weaponScript)) Debug.LogWarning("Newly equipped weapon does not implement IWeapon interface");
    }
}