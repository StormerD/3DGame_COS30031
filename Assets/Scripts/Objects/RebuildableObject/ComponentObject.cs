using System;
using UnityEngine;

public class ComponentObject : MonoBehaviour, IItem
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
        InteractedWith?.Invoke(); // probably just UI things
    }

    public void Pickup(IInteractor interactor)
    {
        // when we integrate with UI / effects, might want to do some other things. for now, just turns off the game object
        // when gameobjects are disabled the script also disables, but if you have a reference to it, you can still call methods
        // on it and fields retain same data
        gameObject.SetActive(false);
    }

    public void Use(IInteractor interactor)
    {
        Destroy(gameObject);
    }
}
