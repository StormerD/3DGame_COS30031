using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LootContainer : LootableBase
{
    public float _burstForce;
    public float _dampening;

    private List<DropObject> _dropObjects;
    private List<GameObject> _actualLootObjects;

    void Start()
    {
        _dropObjects = GenerateDrops();
        _actualLootObjects = new List<GameObject>();
    }

    public override void DropLoot()
    {
        InstantiateLootObjects();
        BurstLootObjects();
    }

    void InstantiateLootObjects()
    {
        foreach (DropObject drop in _dropObjects)
        {
            for (int i = 0; i < drop.amount; i++)
            {
                GameObject lootObj = Instantiate(drop.prefab, transform.position, Quaternion.identity, null);
                _actualLootObjects.Add(lootObj);
            }
        }
    }

    void BurstLootObjects()
    {
        foreach (GameObject obj in _actualLootObjects)
        {
            if (obj.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            {
                rb.linearDamping = _dampening;
                float randDegree = UnityEngine.Random.Range(0f, 360f);
                float rad = randDegree * Mathf.Deg2Rad;
                float randomForce = UnityEngine.Random.Range(_burstForce * 0.5f, _burstForce * 1.5f);
                float x = randomForce * Mathf.Cos(rad);
                float y = randomForce * Mathf.Sin(rad);
                rb.AddForce(new(x, y), ForceMode2D.Impulse);
            }
            else
            {
                Debug.LogError("Container LootObjects need an attached Rigidbody2D (with Linear Damping!)");
            }
        }
    }

}
