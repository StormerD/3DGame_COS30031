// Implement this for items you want to actually be held in the "inventory" of the player.
using UnityEngine;

public interface IItem : IPickupable
{
    public void Use(IInteractor interactor);
    public int GetId();
    public void SetId(int id);
    public Texture2D GetObject2DRepresentation();
}
