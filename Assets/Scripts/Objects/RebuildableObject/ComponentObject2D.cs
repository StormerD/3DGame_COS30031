using UnityEngine;

public class ComponentObject2D : ComponentObjectBase
{
    public override void Pickup(IInteractor interactor)
    {
        // when we integrate with UI / effects, might want to do some other things. for now, just turns off the game object
        // when gameobjects are disabled the script also disables, but if you have a reference to it, you can still call methods
        // on it and fields retain same data
        gameObject.SetActive(false);
    }
}
