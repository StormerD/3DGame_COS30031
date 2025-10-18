using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class LevelLockedUI3D : LevelLockedUIBase
{
    protected override void Awake()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
        base.Awake();
    }
    void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger) PlayerEnteredTrigger(other.gameObject);
    }
    void OnTriggerExit(Collider other)
    {
        if (!other.isTrigger) PlayerExitedTrigger(other.gameObject);
    }
}