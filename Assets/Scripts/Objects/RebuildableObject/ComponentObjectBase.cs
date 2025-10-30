using UnityEngine;

public abstract class ComponentObjectBase : IItem
{
    public override void Interact(IInteractor interactor)
    {
        Debug.Log("I was interacted with: " + gameObject.name);
    }

    public override void Pickup(IInteractor interactor)
    {
        gameObject.SetActive(false);
        base.Pickup(interactor);
    }

    public override void Use(IInteractor interactor)
    {
        Destroy(gameObject, 1f);
    }
}