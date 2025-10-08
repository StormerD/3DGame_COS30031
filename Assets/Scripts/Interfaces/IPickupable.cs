using UnityEngine;

// Implement this on objects that you want to be able to pickup, but they don't count towards 'inventory' size
// for instance, currency that instantly gets cashed into the player bank, or powerups that are instantly used
public interface IPickupable : IInteractable
{
    public void Pickup(IInteractor interactor);
}

// This is used to represent stacks of currency; mostly used in LootableChest right now.
public struct DropObject
{
    public DropObject(int amt, GameObject go)
    {
        amount = amt;
        prefab = go;
    }

    public int amount;
    public GameObject prefab;
}