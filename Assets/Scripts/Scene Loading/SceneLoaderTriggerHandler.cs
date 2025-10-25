using System;
using UnityEngine;

public abstract class SceneLoaderTriggerHandler : MonoBehaviour
{
    public event Action OnEnterLoadZone;

    void Awake()
    {
        LevelSceneChanger controller = GetComponentInParent<LevelSceneChanger>();
        if (controller == null) Debug.LogWarning($"{gameObject.name} unable to find LevelSceneChanger in parent.");
        else
        {
            controller.OnUnlockedChanged += ChangeTrigger;
            OnEnterLoadZone += controller.Load;
        }
    }

    protected void WhenTriggerEntered(GameObject enter)
    {
        if (enter.CompareTag("Player")) OnEnterLoadZone?.Invoke();
    }

    public abstract void ChangeTrigger(bool unlocked);
}
