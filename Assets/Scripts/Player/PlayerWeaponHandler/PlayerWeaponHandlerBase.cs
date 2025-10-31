using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(PlayerInput), typeof(PlayerDataTracker))]
public abstract class PlayerWeaponHandlerBase : MonoBehaviour, IFighter
{
    public Transform weaponInstantiationTransform;
    [SerializeField] private StringEventObject weaponEquippingStream;
    protected GameObject _equippedWeapon;
    protected WeaponBase _weaponScript;

    void OnEnable()
    {
        weaponEquippingStream.RegisterListener(EquipWeapon);
    }
    void OnDisable()
    {
        weaponEquippingStream.UnregisterListener(EquipWeapon);
    }

    protected virtual void Start()
    {
        PlayerInput inp = GetComponent<PlayerInput>();
        inp.attack.performed += Attack;
        inp.secondary.performed += Secondary;
        
        if (GameManager.instance != null) GameManager.instance.OnLoadComplete += SetEquippedWeapon;

        if (weaponInstantiationTransform == null) weaponInstantiationTransform = transform;

        InitializeWeapon();
    }

    #region Equipping
    protected void SetEquippedWeapon()
    {
        EquipWeapon(GameManager.instance.GetEquippedWeapon(true));
    }
    public void EquipWeapon(string to)
    {
        EquipWeapon(ForgeManager.instance.GetWeaponByID(to));
    }
    public virtual void EquipWeapon(GameObject to)
    {
        if (_equippedWeapon != null) { Destroy(_equippedWeapon); }
        if (to == null)
        {
            _equippedWeapon = null;
            _weaponScript = null;
            return;
        }
        if (this is PlayerWeaponHandler3D) to = (this as PlayerWeaponHandler3D).Get3DWeapon(to.GetComponent<WeaponBase>().weaponData.weaponId);
        _equippedWeapon = Instantiate(to, weaponInstantiationTransform);
        _weaponScript = _equippedWeapon.GetComponent<WeaponBase>();
    }
    public string GetEquippedWeapon() => _weaponScript?.GetWeaponData().weaponId;
    public virtual GameObject GetEquippedWeaponObject() => _equippedWeapon;

    #endregion

    #region Attacking
    private Vector2 lastClickDirection = Vector2.zero;
    public void Attack(CallbackContext ctx)
    {
        if (Mouse.current != null) lastClickDirection = Mouse.current.position.ReadValue();
        UseWeapon();
    }
    public void UseWeapon() { if (VerifyWeaponScriptSynced()) _weaponScript.Attack(lastClickDirection); }
    public void Secondary(CallbackContext ctx)
    {
        if (Mouse.current != null) lastClickDirection = Mouse.current.position.ReadValue();
        UseSecondary();
    }
    public void UseSecondary() { if (VerifyWeaponScriptSynced()) _weaponScript.Secondary(lastClickDirection); }
    #endregion

    protected abstract void InitializeWeapon();
    protected virtual bool VerifyWeaponScriptSynced()
    {
        if (_equippedWeapon != null && _weaponScript == null) return _equippedWeapon.TryGetComponent(out _weaponScript);
        return _equippedWeapon != null && _weaponScript != null;
    }
}