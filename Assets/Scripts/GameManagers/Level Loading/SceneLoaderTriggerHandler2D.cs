using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SceneLoaderTriggerHandler2D : SceneLoaderTriggerHandler
{
    void OnTriggerEnter2D(Collider2D col) => WhenTriggerEntered(col.gameObject);

    public override void ChangeTrigger(bool unlocked)
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = unlocked;
    }
}