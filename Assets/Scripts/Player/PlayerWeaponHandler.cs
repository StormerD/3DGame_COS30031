using System;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerInput))]
public class PlayerWeaponHandler : MonoBehaviour, IFighter
{
    private GameObject _equippedWeapon;
    private IWeapon _weaponScript;
    public event Action WeaponChanged;
    private IMover playerMover;

    void Start()
    {
        PlayerInput inp = GetComponent<PlayerInput>();
        inp.attack.performed += Attack;
        inp.secondary.performed += Secondary;
        playerMover = GetComponent<PlayerMovement>();

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
        if (ForgeManager.instance != null) ForgeManager.instance.OnListingEquipped += EquipWeapon;
        SaveManager.instance.OnSaveDataChanged += SetEquippedWeapon;
    }

    private void SetEquippedWeapon()
    {
        Debug.Log("SETTING EQUIPPED WEAPON.");
        EquipWeapon(SaveManager.instance.GetEquippedWeapon());
    }
    public void EquipWeapon(string to)
    {
        Debug.Log("EQUIPPING: " + to);
        EquipWeapon(ForgeManager.instance.GetWeaponByID(to));
    }
    public void EquipWeapon(GameObject to)
    {
        Destroy(_equippedWeapon);
        if (to == null)
        {
            _equippedWeapon = null;
            _weaponScript = null;
            return;
        }
        _equippedWeapon = Instantiate(to, transform);
        WeaponChanged?.Invoke();
        if (!_equippedWeapon.TryGetComponent(out _weaponScript)) Debug.LogWarning("Newly equipped weapon does not implement IWeapon interface");
        else _weaponScript.LinkNewMover(playerMover);
    }

    public string GetEquippedWeapon() => _weaponScript?.GetWeaponData().weaponId;
    public GameObject GetEquippedWeaponObject() => _equippedWeapon;

    public void Attack(CallbackContext ctx) => UseWeapon();
    public void UseWeapon() { if (VerifyWeaponScriptSynced()) _weaponScript?.Attack(); }

    public void Secondary(CallbackContext ctx) => UseSecondary();
    public void UseSecondary() { if (VerifyWeaponScriptSynced()) _weaponScript?.Secondary(); }

    private bool VerifyWeaponScriptSynced()
    {
        if (_equippedWeapon != null && _weaponScript == null)
        {
            bool res = _equippedWeapon.TryGetComponent(out _weaponScript);
            if (res) _weaponScript.LinkNewMover(playerMover);
            return res;
        }
        return _equippedWeapon != null && _weaponScript != null;
    }
}