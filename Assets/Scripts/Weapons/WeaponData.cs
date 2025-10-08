using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ScriptableObject
{
    // weaponId should be a unique ID; this is used for saving data and also equipping weapons. 
    // Using the weapon's name should be an easy ID.
    public string weaponId; 
    public float basicAttacksPerSecond = 1;
    public int basicAttackDamage = 1;
    public float secondaryAttackCooldownSeconds = 1;
    public int secondaryAttackDamage = 1;
}
