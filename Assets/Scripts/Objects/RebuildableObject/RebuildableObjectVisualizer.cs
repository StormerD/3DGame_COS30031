using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(SpriteRenderer))]
public class RebuildableObjectVisualizer : MonoBehaviour
{
    [Tooltip("The object's sprites (in order from least built to most built)")]
    public List<Sprite> rebuildSprites;
    public ParticleSystem completeBuildEffects;
    [Tooltip("If the RebuildableObject script is attached to the same game object, no need to set this value. This is only used so that multiple objects can change their sprite!")]
    public RebuildableObject rebuildable;

    private int collectedComponents = 0;
    private int maxNumComponents;
    private SpriteRenderer _sr;

    void Start()
    {
        if (rebuildable == null && !TryGetComponent(out rebuildable)) Debug.LogError("Rebuildable is null, and there is not a rebuildable component script on this gameObject.");
        else
        {
            rebuildable.OnComponentsCollected += ComponentCollected;
            maxNumComponents = rebuildable.numComponents;
        }
        _sr = GetComponent<SpriteRenderer>();
    }

    private void ComponentCollected(int howMany)
    {
        collectedComponents += howMany;
        collectedComponents = Mathf.Clamp(collectedComponents, 0, maxNumComponents);

        int spriteIndex = Mathf.RoundToInt((float)collectedComponents / maxNumComponents * (rebuildSprites.Count - 1));

        _sr.sprite = rebuildSprites[spriteIndex];

        if (collectedComponents == maxNumComponents && completeBuildEffects != null)
        {

            Destroy(Instantiate(completeBuildEffects, transform.position, Quaternion.identity), 3f);
        }
    }
}