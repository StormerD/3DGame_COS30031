using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LootableChest : LootableBase, IInteractable
{
    public event Action ChestOpen;
    public float burstForce;

    [DisplayInfo]
    private List<DropObject> _dropObjects;
    private List<GameObject> _actualLootObjects;

    private bool _opened = false;

  public override void DropLoot()
  {
    InstantiateLootObjects();
    BurstLootObjects(); // give the objects we just either pulled from pool or created a circular velocity
  }

    void Start()
    {
        _dropObjects = GenerateDrops();
        _actualLootObjects = new List<GameObject>();
        
    }

    public void Interact(IInteractor interactor)
    {
        if (_opened) return;
        Debug.Log("chest open");

        // Here we probably should use a pool of objects instead of instantiating them all when the chest gets opened.
        // GrabPoolLootObjects();

        DropLoot();

        ChestOpen?.Invoke();

        _opened = true;
    }

    void GrabPoolLootObjects()
    {
        // Todo
    }

    void InstantiateLootObjects()
    {
        foreach (DropObject drop in _dropObjects)
        {
            for (int i = 0; i < drop.amount; i++)
            {
                _actualLootObjects.Add(Instantiate(drop.prefab, transform, false));
            }
        }
    }

    void BurstLootObjects()
    {
        foreach (GameObject obj in _actualLootObjects)
        {
            if (obj.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            {
                obj.transform.localPosition = Vector3.zero;
                float randDegree = UnityEngine.Random.Range(0f, 360f);
                float x = burstForce * (float)Math.Cos(randDegree);
                float y = burstForce * (float)Math.Sin(randDegree);
                rb.AddForce(new(x, y), ForceMode2D.Impulse);
            }
            else
            {
                Debug.LogError("Chest LootObjects need an attached Rigidbody2D (with Linear Damping!)");
            }
        }
    }
}
