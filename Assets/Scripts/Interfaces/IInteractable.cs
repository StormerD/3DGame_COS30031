// Implement this script if you want an object to be interactable. You will also need to set the gameobject
// with this script attached to be on the "Interactable" layer (maybe we change that)
using UnityEngine;

public interface IInteractable
{
    void EnterInteractZone() { Debug.Log("Entered Interact Zone."); }
    void ExitInteractZone() { Debug.Log("Exited Interact Zone."); }
    void SetInteractFocus(Color color) { Debug.Log("Now focused."); }
    void RemoveInteractFocus() { Debug.Log("No longer focused."); }
    void Interact(IInteractor interactor);
}
