using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class LevelLockedUI2D : LevelLockedUIBase
{
    protected override void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
        base.Awake();
    }
    void OnTriggerEnter2D(Collider2D collision) => PlayerEnteredTrigger(collision.gameObject);
    void OnTriggerExit2D(Collider2D collision) => PlayerExitedTrigger(collision.gameObject);
}
