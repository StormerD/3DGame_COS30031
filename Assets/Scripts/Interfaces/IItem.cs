// Implement this for items you want to actually be held in the "inventory" of the player.
using System;
using UnityEngine;

public abstract class IItem : MonoBehaviour, IPickupable
{
    public event Action InteractedWith;
    [SerializeField] private Texture2D _2dRepresentation;
    [SerializeField] private GameobjectEventObject itemPickupStream;
    private int _id;
    public int GetId()
    {
        return _id;
    }

    public void SetId(int to)
    {
        _id = to;
    }
    public Texture2D GetObject2DRepresentation() => _2dRepresentation;

    public abstract void Use(IInteractor interactor);
    public virtual void Pickup(IInteractor interactor)
    {
        itemPickupStream.RaiseEvent(gameObject);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayItemPickedUp();
    }
    public abstract void Interact(IInteractor interactor);
}
