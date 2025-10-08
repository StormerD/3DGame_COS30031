// Implement this for items you want to actually be held in the "inventory" of the player.
public interface IItem : IPickupable
{
    public void Use(IInteractor interactor);
    public int GetId();
    public void SetId(int id);
}
