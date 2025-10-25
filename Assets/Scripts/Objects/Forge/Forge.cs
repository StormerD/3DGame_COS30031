using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Forge : MonoBehaviour, IInteractable
{
    public void Interact(IInteractor interactor)
    {
        ForgeManager.instance.OpenForgeMenu();
    }
}
                           