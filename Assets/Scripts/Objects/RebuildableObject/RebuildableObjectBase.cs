

using System;
using System.Collections.Generic;
using UnityEngine;

public class RebuildableObjectBase : MonoBehaviour, IInteractable
{
    [TextArea] public string info = "This script creates 'components' in the editor as children of this object, instead of having us create them manually and linking them to this object. To create them, open the context menu (three dots next to script name) and click 'RecreateComponents' at the bottom of the menu. The created objects can be moved to their position freely. Make sure to have put the correct prefab in the prefab component!";
    public int numComponents = 3;
    public GameObject componentPrefab;
    public event Action<int> OnComponentsCollected;
    public event Action OnCompletedRebuild;

    private int _numCollected = 0;
    private HashSet<int> _componentIds;

    // editor functions

    [ContextMenu("Recreate Components")]
    void RegenerateChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < numComponents; i++)
        {
            GameObject child = Instantiate(componentPrefab, transform);
            child.name = $"Child {i}";
            child.transform.localPosition = Vector3.zero;
        }
    }

    void OnDrawGizmosSelected()
    {
        for (int i = transform.childCount - 1; i >= 0; i--) Debug.DrawLine(transform.position, transform.GetChild(i).position, Color.red);
    }

    // Game functions
    void Awake()
    {
        if (componentPrefab != null && !componentPrefab.TryGetComponent<IInteractable>(out _))
        {
            Debug.LogError("Rebuildable object component prefab requires a Pickupable script.");
        }
        _componentIds = new HashSet<int>();
        for (int i = 0; i < transform.childCount; i++)
        {
            int randId = UnityEngine.Random.Range(0, int.MaxValue);
            transform.GetChild(i).GetComponent<IItem>().SetId(randId);
            _componentIds.Add(randId);
        }
    }

    void Start()
    {
        if (ActiveGameManager.instance != null)
        {
            OnCompletedRebuild += ActiveGameManager.instance.CurrentLevelComplete;
        }
        else Debug.LogWarning("ActiveGameManager is null; level complete events will not be emitted.");
    }

    public void Interact(IInteractor interactor)
    {
        if (interactor.IsHoldingAnyItemsMatchingIds(_componentIds))
        {
            int componentsGathered = interactor.UseItemsByIds(_componentIds);
            Debug.Log("Collected components: " + componentsGathered);
            _numCollected += componentsGathered;
            OnComponentsCollected?.Invoke(componentsGathered);
            if (_numCollected == numComponents) { Debug.Log("Completed rebuild!"); OnCompletedRebuild?.Invoke(); }

            if (AudioManager.Instance != null) AudioManager.Instance.PlayComponentPlaced();
        }
    }
}