using UnityEngine;

// For entities that can hold weapons and fight with them
public interface IFighter
{
    void UseWeapon();
    void UseSecondary();
    void EquipWeapon(string weaponId);
    void EquipWeapon(GameObject weapon);
    string GetEquippedWeapon();
    GameObject GetEquippedWeaponObject();
}