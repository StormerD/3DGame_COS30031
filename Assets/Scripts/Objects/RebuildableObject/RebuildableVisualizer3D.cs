using System.Collections.Generic;
using UnityEngine;

public class RebuildableVisualizer3D : MonoBehaviour
{
    [Tooltip("Object prefabs, from least built to most built. Include the base state as well.")]
    public List<GameObject> rebuildPrefabs;
    public ParticleSystem completeBuildEffects;
    [Tooltip("If the RebuildableObject script is attached to the same game object, no need to set this value. This is only used so that multiple objects can change their sprite!")]
    public RebuildableObject3D rebuildable;

    private int collectedComponents = 0;
    private int maxNumComponents;

    void Start()
    {
        if (rebuildable == null && !TryGetComponent(out rebuildable)) Debug.LogError("Rebuildable is null, and there is not a rebuildable component script on this gameObject.");
        else
        {
            rebuildable.OnComponentsCollected += ComponentCollected;
            maxNumComponents = rebuildable.numComponents;
        }

        NewRebuildLevel();
    }

    private void ComponentCollected(int howMany)
    {
        collectedComponents += howMany;
        collectedComponents = Mathf.Clamp(collectedComponents, 0, maxNumComponents);

        int prefabIndex = Mathf.RoundToInt((float)collectedComponents / maxNumComponents * (rebuildPrefabs.Count - 1));

        if (collectedComponents == maxNumComponents && completeBuildEffects != null)
        {
            Destroy(Instantiate(completeBuildEffects, transform.position, Quaternion.identity), 3f);
        }
    }

    private void NewRebuildLevel()
    {
        
    }
}