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

    // By checking if already paused, we can make it so that pressing the pause button twice will open then close the pause menu
    public void Pause()
    {
        Debug.Log("Paused.");
        if (_paused) Unpause();
        else
        {
            _paused = true;
            OnPause?.Invoke();
        }
    }

    public void Unpause()
    {
        Debug.Log("Unpause");
        _paused = false;
        OnUnpause?.Invoke();
    }
}
