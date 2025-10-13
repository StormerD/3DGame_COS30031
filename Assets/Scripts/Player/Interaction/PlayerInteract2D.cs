using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(Collider2D))]
public class PlayerInteract2D : PlayerInteractBase
{
    protected override void TryInteract(CallbackContext ctx)
    {
        LayerMask interactableLayer = 1 << LayerMask.NameToLayer("Interactable");
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactSearchRadius, interactableLayer);

        foreach (Collider2D collider in colliders)
        {
            if (!collider.gameObject.TryGetComponent(out IInteractable iObj))
            {
                Debug.LogWarning("Object " + collider.gameObject.transform.name + " is on the Interactable layer, but is not an Interactable object.");
                continue;
            }

            iObj.Interact(this);

            if (iObj is IPickupable)
            {
                if (iObj is IItem) TryCarry(iObj as IItem);
                else (iObj as IPickupable).Pickup(this);
            }

            return; // Only handle one object per interact
        }
    }
}
