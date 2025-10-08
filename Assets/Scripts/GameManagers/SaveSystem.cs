using UnityEngine; 
using System.IO;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class SaveSystem
{
    static string Dir => Application.persistentDataPath + "/saves";
    static string PathFor(int slot) => Dir + $"/slot_{slot}.json";
    static string SlotTimesPath => Dir + "/slot_save_times.json";

    public static void Save(SaveData data)
    {
        if (!Directory.Exists(Dir)) Directory.CreateDirectory(Dir); // Ensure directory exists
        var json = JsonUtility.ToJson(data, true);        // Pretty print for readability
        File.WriteAllText(PathFor(data.slotIndex), json);
    }


    public static void SaveSlotTimes(SlotTimesData data)
    {
        if (!Directory.Exists(Dir)) Directory.CreateDirectory(Dir); // Ensure directory exists
        var json = JsonUtility.ToJson(data, true);        // Pretty print for readability
        File.WriteAllText(SlotTimesPath, json);
    }

    public static bool TryLoad(int slot, out SaveData data)
    {
        var slotPath = PathFor(slot);
        if (!File.Exists(slotPath)) { data = null; return false; }   // Safe check prevents crashes
        data = JsonUtility.FromJson<SaveData>(File.ReadAllText(slotPath));
        return data != null;
    }

    public static bool TryLoadSlotTimes(out SlotTimesData slotTimes)
    {
        if (!File.Exists(SlotTimesPath)) { slotTimes = null; return false; }
        slotTimes = JsonUtility.FromJson<SlotTimesData>(File.ReadAllText(SlotTimesPath));
        return slotTimes != null;
    }
}

[Serializable]
public class SaveData
{
    public int slotIndex;       // identifies which save slot to use
    public int furthestUnlockedLevel;  
    public PlayerCurrency currency; // persistent player progress
    public string equippedWeapon; // the player's currently equipped weapon
    public List<WeaponPurchaseData> weaponsPurchased;
}

[Serializable]
public class PlayerCurrency
{
    public int common = 0;
    public int rare = 0;
    public int mythic = 0;
}

[Serializable]
public class SlotTimesData
{
    public long slotOneLastSaveTime;
    public long slotTwoLastSaveTime;
    public long slotThreeLastSaveTime;
}