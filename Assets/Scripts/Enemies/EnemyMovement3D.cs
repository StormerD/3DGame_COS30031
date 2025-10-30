using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement3D : MonoBehaviour
{
    [SerializeField] private IntEventObject _freezeStream;
    public EnemyData unitData;
    public event Action OnAttack;
    public Transform player;

    private NavMeshAgent _agent;
    private EnemyAttack3D _attack;
    private float _repathTimer;
    [SerializeField] private float repathInterval = 0.1f;
    private bool canMove = true;

    void Awake()
    {
        InitializeMovement();
    }

    void Start()
    {
        InitializeMovement();
    }

    void InitializeMovement()
    {
        _agent = GetComponent<NavMeshAgent>();
        _attack = GetComponent<EnemyAttack3D>();

        OnAttack += () => _attack.TryAttackPlayer();

        // if (_agent != null) _agent.stoppingDistance = unitData.attackRange * 0.9f;

        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;
        else Debug.LogWarning("EnemyMovement3D: Player not assigned");
    }

    void OnEnable()
    {
        _freezeStream.RegisterListener(FreezeEvent);
    }

    void OnDisable()
    {
        _freezeStream.UnregisterListener(FreezeEvent);
    }

    private void FreezeEvent(int state)
    {
        if (state == 0) UnfreezeActions();
        else FreezeActions();
    }

    public void FreezeActions()
    {
        canMove = false;
        _agent.isStopped = true;
    }

    public void UnfreezeActions()
    {
        canMove = true;
        _agent.isStopped = false;
    }

    void FixedUpdate()
    {
        if (_agent == null || player == null || !canMove) return;

        // refresh enemy destination in small intervals
        _repathTimer -= Time.deltaTime;
        if (_repathTimer <= 0)
        {
            _agent.SetDestination(player.position);
            _repathTimer = repathInterval;
        }

        // check distance to player
        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;
        if (toPlayer.sqrMagnitude <=unitData.attackRange * unitData.attackRange)
        {
            OnAttack?.Invoke();
        }
    }

}
