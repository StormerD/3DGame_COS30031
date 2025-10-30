

using UnityEngine;

public class ItemPickupEffects : MonoBehaviour
{
    [SerializeField] private GameobjectEventObject ObjectPickedUpStream;
    [SerializeField] private GameObject particlePickupPoolPrefab;
    private ParticleSystemPool particlePool;

    void Awake()
    {
        particlePool = Instantiate(particlePickupPoolPrefab).GetComponent<ParticleSystemPool>();
    }

    void OnEnable() => ObjectPickedUpStream.RegisterListener(ItemPickedUp);
    void OnDisable() => ObjectPickedUpStream.UnregisterListener(ItemPickedUp);

    private void ItemPickedUp(GameObject item)
    {
        particlePool.PlayParticle(item.transform.position);
    }
}