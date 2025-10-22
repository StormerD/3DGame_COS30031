using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SceneLoaderTriggerHandler3D : SceneLoaderTriggerHandler
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger) WhenTriggerEntered(other.gameObject);
    } 

    public override void ChangeTrigger(bool unlocked)
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = unlocked;
    }
}