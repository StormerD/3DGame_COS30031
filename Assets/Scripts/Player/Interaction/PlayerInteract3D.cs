

using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

// yes, we need both colliders. capsule is actual bounding box, sphere collider is a trigger detecting objects in area
[RequireComponent(typeof(CapsuleCollider), typeof(SphereCollider))]
public class PlayerInteract3D : PlayerInteractBase
{
    public Color interactFocusOutlineColor = new(1, 0.6f, 0.4f);
    private readonly Dictionary<GameObject, IInteractable> interactablesInRange = new();
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
            if (interactablesInRange[closest] is IItem) TryCarry(interactablesInRange[closest] as IItem);
            else (interactablesInRange[closest] as IPickupable).Pickup(this);
            interactablesInRange.Remove(closest);
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

        if (closestNow != null && interactablesInRange.ContainsKey(closestNow))
        {
            interactablesInRange[closestNow].SetInteractFocus(interactFocusOutlineColor);
        }
        if (_closestInteractable != null && interactablesInRange.ContainsKey(_closestInteractable))
        {
            interactablesInRange[_closestInteractable].RemoveInteractFocus();
        }

        _closestInteractable = closestNow;
    }

    #endregion

    #region Triggers

    void OnTriggerEnter(Collider col)
    {
        if (!col.TryGetComponent(out IInteractable interactable))
        {
            Debug.Log("Detected an object that is not IInteractable: " + col.name + ". Does the player's sphere collider have include LayerMask set to Interactables, and exclude set to everything but Interactables?");
            return;
        }
        interactable.EnterInteractZone();
        interactablesInRange.Add(col.gameObject, interactable);
        SyncClosestInteractable();
    }
    void OnTriggerExit(Collider col)
    {
        if (!interactablesInRange.ContainsKey(col.gameObject))
        {
            Debug.Log("Object left zone, but it was not registered as a dictionary entry: " + col.name);
            return;
        }
        interactablesInRange[col.gameObject].ExitInteractZone();
        interactablesInRange[col.gameObject].RemoveInteractFocus();
        interactablesInRange.Remove(col.gameObject);
    }

    #endregion
}