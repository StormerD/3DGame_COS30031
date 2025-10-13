using UnityEngine;

[RequireComponent(typeof(PlayerMovement2D))]
public class PlayerWeaponHandler2D : PlayerWeaponHandlerBase
{
    private IMover2D playerMover;
    protected override void Start()
    {
        playerMover = GetComponent<PlayerMovement2D>();
        base.Start();
    }

    protected override void InitializeWeapon()
    {
        if (_equippedWeapon == null)
        {
            if (ActiveGameManager.instance != null && ActiveGameManager.instance.equippedWeapon != null)
            {
                _equippedWeapon = ActiveGameManager.instance.equippedWeapon;
                if (!_equippedWeapon.TryGetComponent(out _weaponScript)) Debug.LogWarning("Equipped weapon missing IWeapon interface");
                else
                {
                    _equippedWeapon.transform.parent = transform;
                    _equippedWeapon.transform.localPosition = Vector2.zero;
                    _weaponScript.LinkNewMover(playerMover);
                }
            }
            if (ActiveGameManager.instance == null && transform.childCount > 0)
            {
                foreach (Transform t in transform) if (t.TryGetComponent<IWeapon>(out var _))
                    {
                        _equippedWeapon = t.gameObject;
                        if (!_equippedWeapon.TryGetComponent(out _weaponScript)) Debug.LogWarning("Equipped weapon does not implement IWeapon interface");
                        else _weaponScript.LinkNewMover(playerMover);
                        break;
                    }
            }
        }
    }

    protected override bool VerifyWeaponScriptSynced()
    {
        if (_equippedWeapon != null && _weaponScript == null)
        {
            bool res = _equippedWeapon.TryGetComponent(out _weaponScript);
            if (res) _weaponScript.LinkNewMover(playerMover);
            return res;
        }
        return _equippedWeapon != null && _weaponScript != null;
    }

    public override void EquipWeapon(GameObject to)
    {
        base.EquipWeapon(to);
        if (!_equippedWeapon.TryGetComponent(out _weaponScript)) Debug.LogWarning("Newly equipped weapon does not implement IWeapon interface");
        else _weaponScript.LinkNewMover(playerMover);
    }
}