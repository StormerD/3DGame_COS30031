using System.Collections.Generic;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine;

[RequireComponent(typeof(PlayerLooter), typeof(PlayerInput))]
public abstract class PlayerInteractBase : MonoBehaviour, IInteractor
{
    #region Public
    public float interactSearchRadius = 1.0f;
    public int maxLoadCarry = 2;

    #endregion

    protected List<IItem> _carriedItems;
    private PlayerInput _inp;
    private PlayerLooter _playerLooter;

    void Awake()
    {
        if (!TryGetComponent(out _inp)) Debug.LogError("PlayerInteract requires a PlayerInput component.");
        if (!TryGetComponent(out _playerLooter)) Debug.LogError("PlayerInteract requires a PlayerLooter component.");
        // Todo: this should become an actual inventory class if we decide to do items
        _carriedItems = new List<IItem>(maxLoadCarry);
    }

    protected virtual void Start()
    {
        _inp.interact.performed += TryInteract;
    }

    #region Usable Items

    public bool DropItem()
    {
        return true;
    }

    public bool IsHoldingAnyItemsMatchingIds(in HashSet<int> ids)
    {
        foreach (IItem obj in _carriedItems)
        {
            if (ids.Contains(obj.GetId())) return true;
        }
        return false;
    }

    public int UseItemsByIds(in HashSet<int> ids)
    {
        int amt = 0;
        for (int i = _carriedItems.Count - 1; i >= 0; i--)
        {
            if (ids.Contains(_carriedItems[i].GetId()))
            {
                _carriedItems[i].Use(this);
                _carriedItems.RemoveAt(i);
                amt += 1;
            }
        }
        return amt;
    }

    // Todo: this is odd, but i see why it's this way. CurrencyLoot is only passed an Interactor when collected, so it 
    // calls back into this script (attached to the player) to let player logic deal with player logic. is there a way 
    // to maybe do this via just the looter, and skip the middle step? might have to do some significant refactoring
    public void CollectCurrency(CurrencyType type, int amount) => _playerLooter.CollectCurrency(type, amount);

    protected bool TryCarry(IItem item)
    {
        if (!CanCarryMoreItems()) return false; // cannot hold objects when full bag

        item.Pickup(this);
        AddNewCarry(item);
        return true;
    }

    protected bool CanCarryMoreItems() => _carriedItems.Count < maxLoadCarry;
    protected void AddNewCarry(IItem item) => _carriedItems.Add(item);

    #endregion

    protected abstract void TryInteract(CallbackContext ctx);
}