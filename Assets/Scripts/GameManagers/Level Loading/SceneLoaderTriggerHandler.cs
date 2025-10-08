using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SceneLoaderTriggerHandler : MonoBehaviour
{
    public event Action OnEnterLoadZone;
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player")) OnEnterLoadZone?.Invoke();
    }

    public void ChangeTrigger(bool unlocked)
    {
        Collider2D col = GetComponent<Collider2D>();
        // If the level is not unlocked, this should be a solid collider, to prevent them from walking further.
        // If it is unlocked, make it a trigger! That way they can walk into it and let the scene load while they're still moving.
        col.isTrigger = unlocked;
    }
}
