using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class LootContainer : LootableBase
{
    [Header("Physics")]
    public float _burstForce;
    public float _dampening;

    [Header("Base loot object")]
    [SerializeField] private LootItem _lootBasePrefab;

    private List<LootItem> _spawnedLoot;
    private IObjectPool<LootItem> _lootPool;

    void Awake()
    {
        _spawnedLoot = new List<LootItem>();
        _lootPool = new ObjectPool<LootItem>(CreateLoot, OnGet, OnRelease);
    }

    void Start()
    {
        
    }

    public override void DropLoot()
    {
        var drops = GenerateDrops();
        InstantiateLootObjects(drops);
        BurstLootObjects();
    }

    private LootItem CreateLoot()
    {
        var item = Instantiate(_lootBasePrefab, transform.position, Quaternion.identity, null);
        item.gameObject.SetActive(false);
        item.SetPool(_lootPool);
        return item;
    }

    void OnGet(LootItem item)
    {
        item.gameObject.SetActive(true);
    }

    void OnRelease(LootItem item)
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
                loot.transform.position = transform.position;
                loot.ApplyRarity(drop.rarity);
                _spawnedLoot.Add(loot);
            }
        }
    }

    void BurstLootObjects()
    {
        foreach (var obj in _spawnedLoot)
        {
            if (obj.TryGetComponent<Rigidbody2D>(out var rb))
            {
                rb.linearDamping = _dampening;
                float randDegree = Random.Range(0f, 360f);
                float rad = randDegree * Mathf.Deg2Rad;
                float randomForce = Random.Range(_burstForce * 0.5f, _burstForce * 1.5f);
                float x = randomForce * Mathf.Cos(rad);
                float y = randomForce * Mathf.Sin(rad);
                rb.AddForce(new(x, y), ForceMode2D.Impulse);
            }
            else
            {
                Debug.LogError("Container LootObjects need an attached Rigidbody2D (with Linear Damping!)");
            }
        }
        _spawnedLoot.Clear();
    }

}
