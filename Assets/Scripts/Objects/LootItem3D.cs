using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

// check sprite renderer and collider exists
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class LootItem3D : MonoBehaviour, IPickupable
{
    public event Action InteractedWith;

    [Header("Currency Materials")]
    public Material[] commonLootMaterials;
    public Material[] rareLootMaterials;
    public Material[] mythicLootMaterials;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 60f;
    public CurrencyType currencyType { get; private set; } // - COMMON - RARE - MYTHIC

    private IObjectPool<LootItem3D> _lootPool;
    private MeshRenderer mr;
    private bool _isRotating = true;

    public void SetPool(IObjectPool<LootItem3D> pool)
    {
        _lootPool = pool;
    }

    private void OnEnable()
    {
        _isRotating = true; // start rotating
    }

    public void ApplyRarity(CurrencyType rarity)
    {
        currencyType = rarity;
        if (mr == null) mr = GetComponent<MeshRenderer>();
        if (mr == null) return;

        Material selectedMaterial = null;
        switch (rarity)
        {
            case CurrencyType.COMMON:
                if (commonLootMaterials != null && commonLootMaterials.Length > 0)
                    selectedMaterial = commonLootMaterials[UnityEngine.Random.Range(0, commonLootMaterials.Length)];
                break;
            case CurrencyType.RARE:
                if (rareLootMaterials != null && rareLootMaterials.Length > 0)
                    selectedMaterial = rareLootMaterials[UnityEngine.Random.Range(0, rareLootMaterials.Length)];
                break;
            case CurrencyType.MYTHIC:
                if (mythicLootMaterials != null && mythicLootMaterials.Length > 0)
                    selectedMaterial = mythicLootMaterials[UnityEngine.Random.Range(0, mythicLootMaterials.Length)];
                break;
        }
        if (selectedMaterial != null)
        {
            mr.material = selectedMaterial;
        }

        // handle lifetime of item
        float destroyTime = UnityEngine.Random.Range(9f, 10f);
        StartCoroutine(LootLifetime(destroyTime));
    }

    private void Update()
    {
        if (_isRotating)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        }
    }

    private IEnumerator LootLifetime(float destroyTime)
    {
        float flashTime = destroyTime - 7f;
        yield return new WaitForSeconds(destroyTime - flashTime);
        StartCoroutine(FlashLoot());
        yield return new WaitForSeconds(flashTime);
        if (_lootPool != null) _lootPool.Release(this);
        else Destroy(gameObject);
    }

    private IEnumerator FlashLoot()
    {
        var renderers = GetComponentsInChildren<MeshRenderer>();
        if (renderers.Length == 0) yield break;

        float flashDuration = 2f;
        float flashInterval = 0.2f;
        float elapsed = 0f;
        bool visible = true;

        while (elapsed < flashDuration)
        {
            visible = !visible;
            foreach (var r in renderers)
                r.enabled = visible;
            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }
        foreach (var r in renderers)
            r.enabled = true;
    }

    public void Interact(IInteractor interactor)
    {
        InteractedWith?.Invoke();
        Pickup(interactor);
    }

    public void Pickup(IInteractor interactor)
    {
        interactor.CollectCurrency(currencyType);
        // pool item
        if (_lootPool != null) _lootPool.Release(this);
        else gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider collision)
    {
        // only pick up when PlayerBody layer capsule touches loot
        if (collision.gameObject.layer != LayerMask.NameToLayer("PlayerBody")) return;
        if (collision is not CapsuleCollider) return;
        // get interactor from parent
        if (collision.transform.root.TryGetComponent<IInteractor>(out var interactor))
        {
            Pickup(interactor);
        }
    }
}
