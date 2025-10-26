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
        if (GameManager.instance == null) Debug.LogError("Cannot save: GameManager instance is null.");
        else
        {
            GameManager.instance.Save();
        }
    }

    public void QuitToMainMenu()
    {
        if (GameManager.instance == null) Debug.LogError("Cannot save: SaveManager instance is null.");
        else
        {
            GameManager.instance.SaveAndClearGame();
            SceneManager.LoadScene(0);
        }
    }
}
