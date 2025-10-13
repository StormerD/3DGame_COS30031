using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement3D))]
public class PlayerWeaponHandler3D : PlayerWeaponHandlerBase
{
    [Tooltip("Upon entry into a level scene, we pull the equipped weapon from the ActiveGameManager. That will give us a 2d weapon. This is a list of 3D equivalent weapons that will be matched by ID of the 2d weapon.")]
    public List<GameObject> weapon3DTranslations;
    private Dictionary<string, GameObject> _3dWeaponsById;
    private IMover3D _playerMovement;

    protected override void Start()
    {
        _playerMovement = GetComponent<PlayerMovement3D>();
        base.Start();
    }

    public override void EquipWeapon(GameObject weapon)
    {
        GameObject weapon3D = TranslateWeaponDimension(weapon);
        base.EquipWeapon(weapon3D);
    }

    private GameObject TranslateWeaponDimension(GameObject original)
    {
        string originalId = original.TryGetComponent(out IWeapon originalWeaponScript) ? originalWeaponScript.GetWeaponData().weaponId : "";
        
        if (originalId == "") Debug.LogWarning("No original weapon ID found on " + original.name);
        else if (!_3dWeaponsById.ContainsKey(originalId)) Debug.LogWarning("No suitable 3D translation found for ID: " + originalId);
        else return _3dWeaponsById[originalId];
        
        return null;
    }

    protected override void InitializeWeapon()
    {
        throw new System.NotImplementedException();
    }

    protected override bool VerifyWeaponScriptSynced()
    {
        throw new System.NotImplementedException();
    }
}