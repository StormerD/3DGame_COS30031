// Implement this script if you want an object to be interactable. You will also need to set the gameobject
// with this script attached to be on the "Interactable" layer (maybe we change that)
public interface IInteractable
{
    public void Interact(IInteractor interactor);
}
