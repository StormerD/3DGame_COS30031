using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LootObject", menuName = "Scriptable Objects/LootObject")]
public class LootTable : ScriptableObject
{
    [Header("Item Amount")]
    public int totalDropsMin;
    public int totalDropsMax;
    [Header("Rarity chances (weights)")]
    [Range(0, 1)] public float commonChance;
    [Range(0, 1)] public float rareChance;
    [Range(0, 1)] public float mythicChance;
    // [Header("Special drops - items, powerups, etc")]
    // public List<SpecialDrop> specialDrops;
}

// [System.Serializable]
// public struct SpecialDrop
// {
//     public string LootName;
//     [Range(0, 1)] public float dropChance;
//     public GameObject dropObject; // the object that spawns if we decided to make these things pickupable
// }
