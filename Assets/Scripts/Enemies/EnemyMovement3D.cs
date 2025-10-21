using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement3D : MonoBehaviour
{
    public Transform player;

    private NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        navMeshAgent.SetDestination(player.position);
    }

}
