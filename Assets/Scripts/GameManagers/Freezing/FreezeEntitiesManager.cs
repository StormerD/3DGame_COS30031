

using System;
using UnityEngine;

// this is a class that all entities should subscribe to, and anything that wants to "freeze" the entities in the game
// should link to by passing an Action StartAction and Action EndAction
public class FreezeEntitiesManager : MonoBehaviour
{
    [SerializeField] private IntEventObject FreezeStream; // emits 0 (false) for "freeze stopped" and anything else for freeze started
    [SerializeField] private BasicEventObject FreezeStartRequestStream;
    [SerializeField] private BasicEventObject FreezeEndRequestStream;
    private bool _frozen; // for sanity-checking

    void OnEnable()
    {
        FreezeStartRequestStream.RegisterListener(StartFreeze);
        FreezeEndRequestStream.RegisterListener(EndFreeze);
    }

    void OnDisable()
    {
        FreezeStartRequestStream.UnregisterListener(StartFreeze);
        FreezeEndRequestStream.UnregisterListener(EndFreeze);
    }
    public void StartFreeze()
    {
        Debug.Log("Starting freeze.");
        if (!_frozen) FreezeStream.RaiseEvent(1);
        else Debug.Log("Tried to freeze when already frozen!");
        _frozen = true;
    }
    
    public void EndFreeze()
    {
        Debug.Log("Starting freeze.");
        if (_frozen) FreezeStream.RaiseEvent(0);
        else Debug.Log("Tried to unfreeze when not frozen!");
        _frozen = false;
    }
}