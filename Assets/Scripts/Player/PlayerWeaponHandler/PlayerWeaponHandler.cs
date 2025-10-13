using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(PlayerInput))]
public abstract class PlayerWeaponHandlerBase : MonoBehaviour, IFighter
{
    public Transform weaponInstantiationTransform;
    protected GameObject _equippedWeapon;
    protected IWeapon _weaponScript;
    protected virtual void Start()
    {
        PlayerInput inp = GetComponent<PlayerInput>();
        inp.attack.performed += Attack;
        inp.secondary.performed += Secondary;
        
        if (ForgeManager.instance != null) ForgeManager.instance.OnListingEquipped += EquipWeapon;
        SaveManager.instance.OnSaveDataChanged += SetEquippedWeapon;

        if (weaponInstantiationTransform == null) weaponInstantiationTransform = transform;

        InitializeWeapon();
    }

    #region Equipping
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
    public virtual void EquipWeapon(GameObject to)
    {
        Destroy(_equippedWeapon);
        if (to == null)
        {
            _equippedWeapon = null;
            _weaponScript = null;
            return;
        }
        _equippedWeapon = Instantiate(to, weaponInstantiationTransform);
    }
    public string GetEquippedWeapon() => _weaponScript?.GetWeaponData().weaponId;
    public GameObject GetEquippedWeaponObject() => _equippedWeapon;
    #endregion

    #region Attacking
    public void Attack(CallbackContext ctx) => UseWeapon();
    public void UseWeapon() { if (VerifyWeaponScriptSynced()) _weaponScript?.Attack(); }
    public void Secondary(CallbackContext ctx) => UseSecondary();
    public void UseSecondary() { if (VerifyWeaponScriptSynced()) _weaponScript?.Secondary(); }
    #endregion

    protected abstract void InitializeWeapon();
    protected abstract bool VerifyWeaponScriptSynced();
}