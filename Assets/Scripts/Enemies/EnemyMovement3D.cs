using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement3D : MonoBehaviour
{
    public Transform player;

    private NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

            var ptrans = GameObject.FindWithTag("Player");
            if (ptrans) player = ptrans.transform;
            else Debug.LogWarning("EnemyMovement3D: Player not assigned");

        // player = GameObject.FindWithTag("player");
    }

    void Update()
    {
        navMeshAgent.SetDestination(player.position);
    }

}
