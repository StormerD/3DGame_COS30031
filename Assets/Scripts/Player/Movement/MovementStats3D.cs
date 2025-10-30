using UnityEngine;

[CreateAssetMenu(fileName = "MovementStats3D", menuName = "EntityData/3D Movement Stats")]
public class MovementStats3D : ScriptableObject
{
    public float maxSpeed = 100f;
    public float speed = 10f;
    public float acceleration = 8f;
    public float deceleration = 5f;
    public float jumpForce = 10f;
    public float dashForce = 15f;
    public float dashCooldownSeconds = 1.5f;
    public float directionLerpSpeed = 2f;
}
