using System;
using UnityEngine;

public abstract class ComponentObjectBase : MonoBehaviour, IItem
{
    public event Action InteractedWith;
    private int _id;
    public int GetId()
    {
        return _id;
    }

    public void SetId(int to)
    {
        _id = to;
    }

    public void Interact(IInteractor interactor)
    {
        Debug.Log("I was interacted with: " + gameObject.name);
        InteractedWith?.Invoke(); // probably just UI things
    }

    public virtual void Pickup(IInteractor interactor)
    {
        gameObject.SetActive(false);
    }
    
    public virtual void Use(IInteractor interactor)
    {
        Destroy(gameObject);
    }
}