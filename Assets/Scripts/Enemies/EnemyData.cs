using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "EntityData/EnemyData")]
public class EnemyData : ScriptableObject
{
    public int maxHealth = 5;
    public float moveSpeed = 2.0f;
    public int damage = 1;
    public float attackRange = 2;
    public float timeBetweenAttacks = 1f;
    public float rotationSpeed = 100;
    public float lungeDistance = 2f;
    public float lungeDuration = 0.1f;
}
