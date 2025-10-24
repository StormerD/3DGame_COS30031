using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement3D))]
public class PlayerWeaponHandler3D : PlayerWeaponHandlerBase
{
    [Tooltip("Upon entry into a level scene, we pull the equipped weapon from the save manager. That will give us a weapon string. This is a list of 3D equivalent weapons that will be matched by ID of the 2d weapon.")]
    public List<GameObject> weapon3DTranslations;
    private Dictionary<string, GameObject> _3dWeaponsById = new();

    protected override void Start()
    {
        foreach(var w in weapon3DTranslations)
        {
            if (w.TryGetComponent<WeaponBase>(out var weapon))
            {
                WeaponData d = weapon.GetWeaponData();
                if (d != null) _3dWeaponsById.Add(d.weaponId, w);
                else Debug.LogWarning("Weapon " + w.name + " missing weapon data.");
            }
            else Debug.LogWarning("Weapon " + w.name + " missing a weapon script (or does not inherit WeaponBase)");
        }
        base.Start();
    }
    
    private GameObject Get3DWeapon(string id)
    {
        if (_3dWeaponsById.ContainsKey(id)) return _3dWeaponsById[id];

        Debug.LogWarning("No 3D weapon found for ID: " + id); 
        return null;
    }

    protected override void InitializeWeapon()
    {
        if (_equippedWeapon != null) return;

        if (GameManager.instance != null)
        {
            string equippedWeapon = GameManager.instance.GetEquippedWeapon();
            GameObject weapon3D = Get3DWeapon(equippedWeapon);
            if (weapon3D != null)
            {
                if (!weapon3D.TryGetComponent(out _weaponScript)) Debug.LogWarning("Weapon " + weapon3D.name + " does not have an IWeapon script.");
                EquipWeapon(weapon3D);
            } else Debug.LogWarning("Failed equipping 3D weapon.");
        }
        if (GameManager.instance == null && transform.childCount > 0)
        {
            WeaponBase test = GetComponentInChildren<WeaponBase>();
            if (test != null)
            {
                _equippedWeapon = test.gameObject;
                _weaponScript = test;
            } else
            {
                Debug.Log("Could not find any weapon on player");
            }
        }
    }
}