using UnityEngine;

public class ChestUI : MonoBehaviour
{
    public GameObject chestLidPivot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (TryGetComponent<LootableChest>(out var loot))
        {
            loot.ChestOpen += OnChestOpened;
        }
        else Debug.LogError("Could not find LootableChest script on Chest object");
    }

    void OnChestOpened()
    {
        chestLidPivot.transform.Rotate(new(0, 0,-60));
    }
}
