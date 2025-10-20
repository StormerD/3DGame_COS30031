using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
public class PauseMenu : MonoBehaviour
{
    private Animator _animator;
    void Start()
    {
        _animator = GetComponent<Animator>();
        PauseManager.instance.OnPause += OpenAnimation;
        PauseManager.instance.OnUnpause += CloseAnimation;
    }

    public void CloseMenu() => PauseManager.instance.Unpause();
    private void CloseAnimation() => _animator.SetTrigger("CloseMenu");
    private void OpenAnimation() => _animator.SetTrigger("OpenMenu");

    public void Save()
    {
        if (ActiveGameManager.instance == null) Debug.LogError("Cannot save: ActiveGameManager instance is null.");
        else if (SaveManager.instance == null) Debug.LogError("Cannot save: SaveManager instance is null.");
        else
        {
            SaveManager.instance.SafeSave();
        }
    }

    public void QuitToMainMenu()
    {
        if (SaveManager.instance == null) Debug.LogError("Cannot save: SaveManager instance is null.");
        else
        {
            SaveManager.instance.SafeSave();
            SceneManager.LoadScene(0);
        }
    }
}
