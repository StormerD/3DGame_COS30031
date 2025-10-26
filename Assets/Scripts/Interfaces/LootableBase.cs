using System.Collections.Generic;
using UnityEngine;

public struct LootDrop
{
    public int amount;
    public CurrencyType rarity;
    public LootDrop(int amount, CurrencyType rarity) { this.amount = amount; this.rarity = rarity; }
}

public abstract class LootableBase : MonoBehaviour
{
    public LootTable lootTable;

    // uses the loot table and chooses a random amount of wires, ore, and cores to drop, as well as which special
    // items will be dropped
    public List<LootDrop> GenerateDrops()
    {
        var drops = new List<LootDrop>();

        int total = Random.Range(lootTable.totalDropsMin, lootTable.totalDropsMax + 1);

        float c = Mathf.Max(0f, lootTable.commonChance);
        float r = Mathf.Max(0f, lootTable.rareChance);
        float m = Mathf.Max(0f, lootTable.mythicChance);
        float sum = Mathf.Max(0.0001f, c + r + m);
        c /= sum;
        r /= sum;
        m /= sum;

        for (int i = 0; i < total; i ++)
        {
            float roll = Random.value;
            CurrencyType rarity = roll < c ? CurrencyType.COMMON : roll < c + r ? CurrencyType.RARE : CurrencyType.MYTHIC;
            drops.Add(new LootDrop(1, rarity));
        }

        return drops;
    }

    public abstract void DropLoot();
}