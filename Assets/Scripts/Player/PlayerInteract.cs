using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(PlayerLooter), typeof(PlayerInput))]
public class PlayerInteract : MonoBehaviour, IInteractor
{

    #region Public
    public float interactSearchRadius = 1.0f;
    public int maxLoadCarry = 2;

    #endregion

    #region Private

    private List<IItem> _carriedItems;
    private PlayerInput _inp;
    private PlayerLooter _playerLooter;

    #endregion

    void Awake()
    {
        if (!TryGetComponent<PlayerInput>(out _inp)) Debug.LogError("PlayerInteract requires a PlayerInput component.");
        if (!TryGetComponent<PlayerLooter>(out _playerLooter)) Debug.LogError("PlayerInteract requires a PlayerLooter component.");
        // Todo: this should become an actual inventory class if we decide to do items
        _carriedItems = new List<IItem>(maxLoadCarry);
    }

    void Start()
    {
        _inp.interact.performed += TryInteract;
    }

    void TryInteract(CallbackContext ctx)
    {
        LayerMask interactableLayer = 1 << LayerMask.NameToLayer("Interactable");
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactSearchRadius, interactableLayer);

        foreach (Collider2D collider in colliders)
        {
            if (!collider.gameObject.TryGetComponent<IInteractable>(out IInteractable iObj))
            {
                Debug.LogWarning("Object " + collider.gameObject.transform.name + " is on the Interactable layer, but is not an Interactable object.");
                continue;
            }

            iObj.Interact(this);

            if (iObj is IItem)
            {
                if (_carriedItems.Count >= maxLoadCarry) continue; // cannot hold objects when full bag

                (iObj as IItem).Pickup(this);
                _carriedItems.Add(iObj as IItem);
            }

            return; // Only handle one object per interact
        }
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

    public void CollectCurrency(CurrencyType type, int amount) => _playerLooter.CollectCurrency(type, amount);

    #endregion
}
