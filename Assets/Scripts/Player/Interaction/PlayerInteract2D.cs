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

            if (iObj is IItem)
            {
                if (_carriedItems.Count >= maxLoadCarry) continue; // cannot hold objects when full bag

                (iObj as IItem).Pickup(this);
                _carriedItems.Add(iObj as IItem);
            }

            return; // Only handle one object per interact
        }
    }
}
