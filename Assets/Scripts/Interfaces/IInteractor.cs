using System.Collections.Generic;
using UnityEngine;

// Implement this for entities that can pickup + hold IItems. This is really just the player, but in case we went crazy
// and added some AI enemy that could pick up items and take them away from you, made it an interface
public interface IInteractor : ICurrencyHoarder
{
    // Items should have an ID; this checks if the player has any of them (used for components)
    bool IsHoldingAnyItemsMatchingIds(in HashSet<int> ids);
    // Call IItem.Use() on all items in ids
    int UseItemsByIds(in HashSet<int> ids);
}