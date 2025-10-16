using UnityEngine;

// 3D weapons use the camera's direction to determine the attack direction
public abstract class Weapon3D : WeaponBase
{
    protected override Vector3 GetAttackDirection(Vector2 clickScreenPosition)
    {
        throw new System.NotImplementedException();
    }
}