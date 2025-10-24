

using System;
using UnityEngine;

// this is a class that all entities should subscribe to, and anything that wants to "freeze" the entities in the game
// should link to by passing an Action StartAction and Action EndAction
public class FreezeEntitiesManager : MonoBehaviour
{
    public static FreezeEntitiesManager instance;
    public event Action OnFreeze;
    public event Action OnUnfreeze;
    private bool _frozen; // for sanity-checking
    void Awake()
    {
        if (instance != null) Destroy(this);
        else instance = this;
    }

    public void StartFreeze()
    {
        if (!_frozen) OnFreeze?.Invoke();
        else Debug.Log("Tried to freeze when already frozen!");
        _frozen = true;
    }
    
    public void EndFreeze()
    {
        if (_frozen) OnUnfreeze?.Invoke();
        else Debug.Log("Tried to unfreeze when not frozen!");
        _frozen = false;
    }
}