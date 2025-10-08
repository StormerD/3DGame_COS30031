using UnityEngine;

public interface IProjectile
{
    void SetSpeed(float s);
    void SetDamage(int d);
    void SetDirection(Vector2 d);
}