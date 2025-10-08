using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PauseMenu : MonoBehaviour, IMenuPausable
{
    private Animator _animator;
    void Start()
    {
        _animator = GetComponent<Animator>();
        PauseManager.instance.OnPause += OpenMenu;
        PauseManager.instance.OnUnpause += CloseMenu;
    }
    public void CloseMenu()
    {
        _animator.SetTrigger("CloseMenu");
    }
    public void OpenMenu()
    {
        _animator.SetTrigger("OpenMenu");
    }

    public void Save()
    {
        if (ActiveGameManager.instance == null) Debug.LogError("Cannot save: ActiveGameManager instance is null.");
        else if (SaveManager.instance == null) Debug.LogError("Cannot save: SaveManager instance is null.");
        else
        {
            SaveManager.instance.SafeSave(ActiveGameManager.instance.saveSlot);
        }
    }

    public void Quit()
    {
        Save();
        Application.Quit(0);
    }
}
