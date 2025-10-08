using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public int maxHealth = 5;
    public float moveSpeed = 2.0f;
    public int damage = 1;
    public float attackRange = 2;
    public float attacksPerSec = 1f;
    public float rotationSpeed = 100;
}
