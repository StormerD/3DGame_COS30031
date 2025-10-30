using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemPool : MonoBehaviour
{
    public static ParticleSystemPool Instance { get; private set; }

    [SerializeField] private GameObject prefabWithParticleSystem;
    [SerializeField] private int poolSize = 10;

    private readonly Queue<ParticleSystem> pool = new();

    void Awake()
    {
        // Pre-instantiate dust objects
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefabWithParticleSystem);
            obj.SetActive(false);
            pool.Enqueue(obj.GetComponent<ParticleSystem>());
        }
    }

    public void PlayParticle(Vector3 position)
    {
        if (pool.Count == 0)
        {
            Debug.LogWarning("Particle pool empty! Consider increasing pool size.");
            return;
        }

        ParticleSystem particle = pool.Dequeue();
        particle.gameObject.SetActive(true);
        particle.transform.position = position;
        particle.Play();

        // Re-queue after particle finishes
        StartCoroutine(ReturnToPool(particle));
    }

    private System.Collections.IEnumerator ReturnToPool(ParticleSystem particle)
    {
        // Wait for the particle system to finish
        yield return new WaitWhile(() => particle.isPlaying);

        particle.gameObject.SetActive(false);
        pool.Enqueue(particle);
    }
}
