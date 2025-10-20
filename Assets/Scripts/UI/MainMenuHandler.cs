using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
public class MainMenuHandler : MonoBehaviour
{
    public GameObject startMenu;
    public GameObject explanationMenu;
    public GameObject savesMenu;
    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.SetTrigger("GameStart");
    }

    public void GoToExplanationMenu()
    {
        startMenu.SetActive(false);
        explanationMenu.SetActive(true);
    }

    public void GoToStartMenu()
    {
        explanationMenu.SetActive(false);
        savesMenu.SetActive(false);
        startMenu.SetActive(true);
    }

    public void GoToSavesMenu()
    {
        startMenu.SetActive(false);
        savesMenu.SetActive(true);
    }

    public void SaveToSlot(int which)
    {
        SaveManager.instance.Save(which);
    }

    public void StartGame(int slot)
    {
        Debug.Log("Loading game... slot " + slot);

        // Load data
        SaveManager.instance.LoadFromSlot(slot);
        SceneManager.LoadScene("HomeArea");
    }

    public void QuitGame() => Application.Quit();
}
