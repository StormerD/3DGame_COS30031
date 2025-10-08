using System.Collections.Generic;
using UnityEngine;

public abstract class LootableBase : MonoBehaviour
{
    public LootTable lootTable;
    public GameObject commonObject;
    public GameObject rareObject;
    public GameObject mythicObject;

    // uses the loot table and chooses a random amount of wires, ore, and cores to drop, as well as which special
    // items will be dropped
    public List<DropObject> GenerateDrops()
    {
        var drops = new List<DropObject>
        {
            new(Random.Range(lootTable.commonObjectMin, lootTable.commonObjectMax), commonObject),
            new(Random.Range(lootTable.rareObjectMin, lootTable.rareObjectMax), rareObject),
            new(Random.Range(lootTable.mythicObjectMin, lootTable.mythicObjectMax), mythicObject)
        };

        foreach (var item in lootTable.specialDrops)
        {
            if (Random.Range(0f, 1f) > item.dropChance) continue;

            drops.Add(new(1, item.dropObject));
        }

        return drops;
    }

    public abstract void DropLoot();
}