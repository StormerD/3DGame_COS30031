using System;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager instance;
    [SerializeField] private BasicEventObject FreezeRequestStartStream;
    [SerializeField] private BasicEventObject FreezeRequestEndStream;
    public event Action OnPause;
    public event Action OnUnpause;
    private bool _paused = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void OnEnable()
    {
        if (FreezeRequestEndStream == null || FreezeRequestEndStream == null) Debug.LogWarning("Forge manager missing references to freeze streams!");
        OnPause += FreezeRequestStartStream.RaiseEvent;
        OnUnpause += FreezeRequestEndStream.RaiseEvent;
    }
    void OnDisable()
    {
        OnPause += FreezeRequestStartStream.RaiseEvent;
        OnUnpause += FreezeRequestEndStream.RaiseEvent;
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
