using UnityEngine;

[CreateAssetMenu(fileName = "MovementStats2D", menuName = "EntityData/2D Movement Stats")]
public class MovementStats2D : ScriptableObject
{
    public float speed = 5f;
    public float dashSpeed = 15f;
    public float dashLength = 0.3f;
    public float dashCooldownSeconds = 1.5f;
}
