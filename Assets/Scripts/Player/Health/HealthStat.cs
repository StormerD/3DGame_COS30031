using UnityEngine;

[CreateAssetMenu(fileName = "HealthStat", menuName = "EntityData/Health Stat")]
public class HealthStat : ScriptableObject
{
    public int maxHealth;
    public int startingHealth;
}
