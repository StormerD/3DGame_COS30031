using System.Collections.Generic;
using UnityEngine;

public class DustPool : MonoBehaviour
{
    public static DustPool Instance { get; private set; }

    [SerializeField] private GameObject dustPrefab;
    [SerializeField] private int poolSize = 10;

    private readonly Queue<ParticleSystem> pool = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Pre-instantiate dust objects
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(dustPrefab);
            obj.SetActive(false);
            pool.Enqueue(obj.GetComponent<ParticleSystem>());
        }
    }

    public void PlayDust(Vector3 position)
    {
        if (pool.Count == 0)
        {
            Debug.LogWarning("DustPool empty! Consider increasing pool size.");
            return;
        }

        ParticleSystem dust = pool.Dequeue();
        dust.gameObject.SetActive(true);
        dust.transform.position = position;
        dust.Play();

        // Re-queue after particle finishes
        StartCoroutine(ReturnToPool(dust));
    }

    private System.Collections.IEnumerator ReturnToPool(ParticleSystem dust)
    {
        // Wait for the particle system to finish
        yield return new WaitWhile(() => dust.isPlaying);

        dust.gameObject.SetActive(false);
        pool.Enqueue(dust);
    }
}
