using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LootObject", menuName = "Scriptable Objects/LootObject")]
public class LootTable : ScriptableObject
{
    [Header("Common Object Range")]
    public int commonObjectMin;
    public int commonObjectMax;
    [Header("Rare Object Range")]
    public int rareObjectMin;
    public int rareObjectMax;
    [Header("Mythic Object Range")]
    public int mythicObjectMin;
    public int mythicObjectMax;
    [Header("Special drops - items, powerups, etc")]
    public List<SpecialDrop> specialDrops;
}

[System.Serializable]
public struct SpecialDrop
{
    public string LootName;
    [Range(0, 1)] public float dropChance;
    public GameObject dropObject; // the object that spawns if we decided to make these things pickupable
}
