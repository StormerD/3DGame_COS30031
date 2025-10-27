using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

// check sprite renderer and collider exists
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class LootItem : MonoBehaviour, IPickupable
{
    public event Action InteractedWith;
    [Header("Currency Sprites")]
    public Sprite[] commonLootSprites;
    public Sprite[] rareLootSprites;
    public Sprite[] mythicLootSprites;
    public CurrencyType currencyType { get; private set; } // - COMMON - RARE - MYTHIC

    private IObjectPool<LootItem> _lootPool;
    private SpriteRenderer sr;

    public void SetPool(IObjectPool<LootItem> pool)
    {
        _lootPool = pool;
    }

    public void ApplyRarity(CurrencyType rarity)
    {
        currencyType = rarity;
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;

        Sprite selectedSprite = null;
        switch (rarity)
        {
            case CurrencyType.COMMON:
                if (commonLootSprites != null && commonLootSprites.Length > 0)
                    selectedSprite = commonLootSprites[UnityEngine.Random.Range(0, commonLootSprites.Length)];
                break;
            case CurrencyType.RARE:
                if (rareLootSprites != null && rareLootSprites.Length > 0)
                    selectedSprite = rareLootSprites[UnityEngine.Random.Range(0, rareLootSprites.Length)];
                break;
            case CurrencyType.MYTHIC:
                if (mythicLootSprites != null && mythicLootSprites.Length > 0)
                    selectedSprite = mythicLootSprites[UnityEngine.Random.Range(0, mythicLootSprites.Length)];
                break;
        }
        if (selectedSprite != null)
        {
            sr.sprite = selectedSprite;
        }

        // handle lifetime of item
        float destroyTime = UnityEngine.Random.Range(9f, 10f);
        StartCoroutine(LootLifetime(destroyTime));
    }

    private IEnumerator LootLifetime(float destroyTime)
    {
        float flashTime = destroyTime - 7f;
        yield return new WaitForSeconds(destroyTime - flashTime);
        StartCoroutine(FlashLoot());
        yield return new WaitForSeconds(flashTime);
        Destroy(gameObject);
    }

    private IEnumerator FlashLoot()
    {
        var spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        if (spriteRenderers.Length == 0) yield break;

        float flashDuration = 2f;
        float flashInterval = 0.2f;
        float elapsed = 0f;
        bool visible = true;

        while (elapsed < flashDuration)
        {
            visible = !visible;
            foreach (var sr in spriteRenderers)
                sr.enabled = visible;
            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }
        foreach (var sr in spriteRenderers)
            sr.enabled = true;
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

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<IInteractor>(out var interactor))
        {
            Pickup(interactor);
        }
    }
}
