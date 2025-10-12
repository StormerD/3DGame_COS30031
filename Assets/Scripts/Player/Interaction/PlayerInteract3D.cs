

using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

// yes, we need both colliders. capsule is actual bounding box, sphere collider is a trigger detecting objects in area
[RequireComponent(typeof(CapsuleCollider), typeof(SphereCollider))]
public class PlayerInteract3D : PlayerInteractBase
{
    private readonly Dictionary<GameObject, IInteractable> interactablesInRange = new();

    protected override void Start()
    {
        base.Start();
        SphereCollider c = GetComponent<SphereCollider>();
        if (!c.isTrigger) Debug.LogWarning("set sphere collider to trigger");
    }

    #region Interact

    protected override void TryInteract(CallbackContext ctx)
    {
        if (interactablesInRange.Count == 0) return;
        GameObject closest = FindClosestInteractable();
        interactablesInRange[closest].Interact(this);
        // if (interactablesInRange[closest] is IPickupable)
        // {
        //     (interactablesInRange[closest] as IPickupable).Pickup(this);
        //     interactablesInRange.Remove(closest);
        // }
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
    }
    void OnTriggerExit(Collider col)
    {
        if (!interactablesInRange.ContainsKey(col.gameObject))
        {
            Debug.Log("Object left zone, but it was not registered as a dictionary entry: " + col.name);
            return;
        }
        interactablesInRange[col.gameObject].ExitInteractZone();
        interactablesInRange.Remove(col.gameObject);
    }

    #endregion
}