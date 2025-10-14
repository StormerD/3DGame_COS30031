using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement3D))]
public class PlayerWeaponHandler3D : PlayerWeaponHandlerBase
{
    [Tooltip("Upon entry into a level scene, we pull the equipped weapon from the ActiveGameManager. That will give us a 2d weapon. This is a list of 3D equivalent weapons that will be matched by ID of the 2d weapon.")]
    public List<GameObject> weapon3DTranslations;
    private Dictionary<string, GameObject> _3dWeaponsById = new();
    private GameObject _weapon2D;

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

    public override void EquipWeapon(GameObject weapon)
    {
        GameObject weapon3D = TranslateWeaponDimension(weapon);
        if (weapon != weapon3D) _weapon2D = weapon; // store 2d version for the active game manager
        base.EquipWeapon(weapon3D);
    }

    private GameObject TranslateWeaponDimension(GameObject original)
    {
        string originalId = original.TryGetComponent(out WeaponBase originalWeaponScript) ? originalWeaponScript.GetWeaponData().weaponId : "";
        
        if (originalId == "") Debug.LogWarning("No original weapon ID found on " + original.name);
        else if (!_3dWeaponsById.ContainsKey(originalId)) Debug.LogWarning("No suitable 3D translation found for ID: " + originalId);
        else return _3dWeaponsById[originalId];
        
        return null;
    }

    protected override void InitializeWeapon()
    {
        if (_equippedWeapon != null) return;
        
        if (ActiveGameManager.instance != null && ActiveGameManager.instance.equippedWeapon != null)
        {
            GameObject temp = ActiveGameManager.instance.equippedWeapon;
            if (!temp.TryGetComponent(out _weaponScript)) Debug.LogWarning("Equipped weapon missing IWeapon interface");
            else
            {
                GameObject translation = TranslateWeaponDimension(temp);
                if (!translation.TryGetComponent<WeaponBase>(out _)) Debug.LogWarning("Translation " + translation.name + " does not have an IWeapon script.");
                EquipWeapon(temp);
            }
        }
        if (ActiveGameManager.instance == null && transform.childCount > 0)
        {
            foreach (Transform t in transform)
            {
                if (t.TryGetComponent<WeaponBase>(out var _))
                {
                    _equippedWeapon = t.gameObject;
                    if (!_equippedWeapon.TryGetComponent(out _weaponScript)) Debug.LogWarning("Equipped weapon does not implement IWeapon interface");
                    break;
                }
            }
        }
    }

    protected override bool VerifyWeaponScriptSynced()
    {
        throw new System.NotImplementedException();
    }

    public override GameObject GetEquippedWeaponObject()
    {
        if (_weapon2D != null) return _weapon2D;
        return base.GetEquippedWeaponObject();
    }
}