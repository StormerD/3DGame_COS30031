using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RebuildableVisualizer3D : MonoBehaviour
{
    [Tooltip("The number of components that this visualizer will play its rebuild animation at.")]
    [SerializeField] int playRebuildAtCollectedAmount;
    [Tooltip("If the RebuildableObject script is attached to the same game object, no need to set this value. This is only used so that multiple objects can change their sprite!")]
    [SerializeField] RebuildableObject3D rebuildable;
    [SerializeField] List<KeyedObject<ParticleSystem>> keyedParticleSystems;
    private int collectedComponents = 0;
    private int maxNumComponents;
    private Animator _animator;
    private AudioSource _audioSource;
    private bool _hasPlayedAnimation;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        TryGetComponent(out _audioSource);
    }

    void Start()
    {
        if (rebuildable == null && !TryGetComponent(out rebuildable)) Debug.LogError("Rebuildable is null, and there is not a rebuildable component script on this gameObject.");
        else
        {
            rebuildable.OnComponentsCollected += ComponentCollected;
            maxNumComponents = rebuildable.numComponents;
        }
    }

    private void ComponentCollected(int howMany)
    {
        collectedComponents += howMany;
        collectedComponents = Mathf.Clamp(collectedComponents, 0, maxNumComponents);

        if (collectedComponents >= playRebuildAtCollectedAmount && !_hasPlayedAnimation)
        {
            _animator.SetTrigger("RebuildComplete");
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null && _audioSource != null) _audioSource.PlayOneShot(clip);
    }
    public void PlayParticle(string key)
    {
        foreach(var keyedSystem in keyedParticleSystems)
        {
            if (key != keyedSystem.key) continue;
            
            keyedSystem.obj.Play();
            break;
        }
    }
}

[Serializable]
public class KeyedObject<T>
{
    public string key;
    public T obj;
}