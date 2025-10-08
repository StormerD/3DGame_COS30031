
using System;
using System.Collections.Generic;

[Serializable]
public struct PurchasePrice
{
    public int common;
    public int rare;
    public int mythic;

    public PurchasePrice(int c = 0, int r = 0, int m = 0)
    {
        common = c;
        rare = r;
        mythic = m;
    }

    public string ToDisplayString()
    {
        if (common == 0 && rare == 0 && mythic == 0) return "Free";

        var parts = new List<string>(3);
        if (common > 0) parts.Add($"{common} Common");
        if (rare > 0) parts.Add($"{rare} Rare");
        if (mythic > 0) parts.Add($"{mythic} Mythic");

        return parts.Count == 0 ? "" : string.Join(", ", parts);
    }
}

[Serializable]
public class WeaponPurchaseData
{
    public string weaponId;
    public bool isPurchased;
    public bool isUnlocked;
}