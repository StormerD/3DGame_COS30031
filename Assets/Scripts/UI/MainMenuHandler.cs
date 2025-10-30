using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] private GameObject startMenu;
    [SerializeField] private GameObject explanationMenu;
    [SerializeField] private GameObject savesMenu;
    [SerializeField] private GameObject optionsMenu;
    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
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

    public void GoToOptionsMenu()
    {
        startMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void StartGame(int slot)
    {
        Debug.Log("Loading game... slot " + slot);

        // Load data
        GameManager.instance.OnLoadComplete += LoadToActiveScene;
        GameManager.instance.LoadFromSlot(slot);
    }

    private void LoadToActiveScene()
    {
        Debug.Log("Loading scene: " + GameManager.instance.GetActiveScene());
        GameManager.instance.OnLoadComplete -= LoadToActiveScene;
        StartCoroutine(AsyncSceneLoader.AsyncLoad(GameManager.instance.GetActiveScene()));
    }

    public void QuitGame() => Application.Quit();
}
