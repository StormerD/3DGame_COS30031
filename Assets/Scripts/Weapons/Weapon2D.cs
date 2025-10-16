using UnityEngine;

// 2D weapons use the mouse position relative to player (weapon) position to determine attack direction
public abstract class Weapon2D : WeaponBase
{
    protected override Vector3 GetAttackDirection(Vector2 clickScreenPostion)
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(clickScreenPostion);

        Debug.DrawLine(transform.position, mousePosition, Color.rebeccaPurple, 5f);
        return (mousePosition - new Vector2(transform.position.x, transform.position.y)).normalized;
    }
}