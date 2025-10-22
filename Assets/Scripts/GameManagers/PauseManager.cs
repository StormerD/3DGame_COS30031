using System;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager instance;
    public event Action OnPause;
    public event Action OnUnpause;
    private bool _paused = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (FreezeEntitiesManager.instance != null)
        {
            Debug.Log("pause manager linking");
            OnPause += FreezeEntitiesManager.instance.StartFreeze;
            OnUnpause += FreezeEntitiesManager.instance.EndFreeze;
        }
        else Debug.Log("Cannot add dialogue to freeze, as freezeentitiesmanager is null!");
    }

    // By checking if already paused, we can make it so that pressing the pause button twice will open then close the pause menu
    public void Pause()
    {
        if (_paused) Unpause();
        else
        {
            _paused = true;
            OnPause?.Invoke();
        }
    }

    public void Unpause()
    {
        _paused = false;
        OnUnpause?.Invoke();
    }
}
