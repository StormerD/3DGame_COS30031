using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class LootContainer3D : LootableBase
{
    [Header("Physics")]
    public float _burstForce = 5f;
    public float _dampening = 2f;

    [Header("Base Loot 3D object")]
    [SerializeField] private LootItem3D _lootBasePrefab;
    [SerializeField] private float _spawnHeight = 0.5f;

    private List<LootItem3D> _spawnedLoot;
    private IObjectPool<LootItem3D> _lootPool;

    void Awake()
    {
        _spawnedLoot = new List<LootItem3D>();
        _lootPool = new ObjectPool<LootItem3D>(CreateLoot, OnGet, OnRelease);
    }

    public override void DropLoot()
    {
        var drops = GenerateDrops();
        InstantiateLootObjects(drops);
        BurstLootObjects();
    }

    private LootItem3D CreateLoot()
    {
        var item = Instantiate(_lootBasePrefab, transform.position, Quaternion.identity, null);
        item.gameObject.SetActive(false);
        item.SetPool(_lootPool);
        return item;
    }

    void OnGet(LootItem3D item)
    {
        item.gameObject.SetActive(true);
    }

    void OnRelease(LootItem3D item)
    {
        item.gameObject.SetActive(false);
        item.transform.SetParent(null);
    }

    void InstantiateLootObjects(List<LootDrop> drops)
    {
        foreach (var drop in drops)
        {
            for (int i = 0; i < drop.amount; i++)
            {
                var loot = _lootPool.Get();
                loot.transform.position = transform.position + Vector3.up * _spawnHeight;
                loot.ApplyRarity(drop.rarity);
                _spawnedLoot.Add(loot);
            }
        }
    }

    void BurstLootObjects()
    {
        foreach (var obj in _spawnedLoot)
        {
            if (obj.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.linearDamping = _dampening;
                Vector3 dir = Random.onUnitSphere;
                dir.y = Mathf.Abs(dir.y); // bias upward
                float force = Random.Range(_burstForce * 0.5f, _burstForce * 1.5f);
                rb.AddForce(dir * force, ForceMode.Impulse);
            }
            else
            {
                Debug.LogError("Container LootObjects need an attached Rigidbody component!");
            }
        }
        _spawnedLoot.Clear();
    }

}
