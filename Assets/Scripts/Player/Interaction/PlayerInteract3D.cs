using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

// yes, we need both colliders. capsule is actual bounding box, sphere collider is a trigger detecting objects in area
[RequireComponent(typeof(CapsuleCollider), typeof(SphereCollider))]
public class PlayerInteract3D : PlayerInteractBase
{
    public Color interactFocusOutlineColor = new(1, 0.6f, 0.4f);
    private readonly Dictionary<GameObject, IInteractable> interactablesInRange = new();
    private readonly Dictionary<GameObject, InteractableOutline3D> interactableOutlinesInRange = new();
    private GameObject _closestInteractable = null;

    protected override void Start()
    {
        base.Start();
        SphereCollider c = GetComponent<SphereCollider>();
        if (!c.isTrigger) Debug.LogWarning("set sphere collider to trigger");
    }

    void FixedUpdate()
    {
        if (interactablesInRange.Count > 0) SyncClosestInteractable();
    }

    #region Interact

    protected override void TryInteract(CallbackContext ctx)
    {
        if (interactablesInRange.Count == 0) return;
        GameObject closest = FindClosestInteractable();
        interactablesInRange[closest].Interact(this);
        if (interactablesInRange[closest] is IPickupable)
        {
            bool shouldRemove = true;
            if (interactablesInRange[closest] is IItem) shouldRemove = TryCarry(interactablesInRange[closest] as IItem);
            else (interactablesInRange[closest] as IPickupable).Pickup(this);
            if (shouldRemove) RemoveInteractable(closest);
        }
    }

    private GameObject FindClosestInteractable()
    {
        float furthest = Mathf.Infinity;
        GameObject furthestObj = null;
        foreach (var obj in interactablesInRange.Keys)
        {
            float dist = (obj.transform.position - transform.position).sqrMagnitude;
            if (dist < furthest)
            {
                furthest = dist;
                furthestObj = obj;
            }
        }
        return furthestObj;
    }

    // finds the closest interactable in the range, sets its outline to a different color.
    private void SyncClosestInteractable()
    {
        GameObject closestNow = FindClosestInteractable();
        if (closestNow == _closestInteractable) return; // work already done prior

        if (closestNow != null && interactableOutlinesInRange.ContainsKey(closestNow))
        {
            interactableOutlinesInRange[closestNow].SetInteractFocus(interactFocusOutlineColor);
        }
        if (_closestInteractable != null && interactableOutlinesInRange.ContainsKey(_closestInteractable))
        {
            interactableOutlinesInRange[_closestInteractable].RemoveInteractFocus();
        }

        _closestInteractable = closestNow;
    }

    private void RemoveInteractable(GameObject which)
    {
        if (!interactablesInRange.ContainsKey(which)) Debug.LogWarning("Object left zone, but it was not registered as an interactable " + which.name);
        if (!interactableOutlinesInRange.ContainsKey(which)) Debug.Log("Object left zone, but it was not registered as an outline entry: " + which.name);

        interactablesInRange.Remove(which);
        interactableOutlinesInRange[which].ExitInteractZone();
        interactableOutlinesInRange.Remove(which);
    }

    private void AddInteractable(GameObject which)
    {
        if (!which.TryGetComponent(out IInteractable interactable))
        {
            Debug.LogWarning("Passed gameobject " + which.name + " which does not have IInteractable. Does the player's sphere collider have include LayerMask set to Interactables, and exclude set to everything but Interactables?");
            return;
        }
        if (!which.TryGetComponent(out InteractableOutline3D outline))
        {
            Debug.LogWarning("Passed gameobject " + which.name + " which does not have an Outline component attached to it.");
            return;
        }

        interactablesInRange.Add(which, interactable);
        interactableOutlinesInRange.Add(which, outline);
        outline.EnterInteractZone();
    }

    #endregion

    #region Triggers

    void OnTriggerEnter(Collider col)
    {
        AddInteractable(col.gameObject);
        SyncClosestInteractable();
    }
    void OnTriggerExit(Collider col)
    {
        RemoveInteractable(col.gameObject);
        SyncClosestInteractable();
    }

    #endregion
}