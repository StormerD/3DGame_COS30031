using UnityEngine;

public class WinScreen : MonoBehaviour
{
    public static WinScreen instance;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (ActiveGameManager.instance == null) Debug.LogWarning("WinScreen needs the active manager to work!");
        if (ActiveGameManager.instance.gameComplete) gameObject.SetActive(true);
        else gameObject.SetActive(false);
    }

    public void RestartGame()
    {
        ActiveGameManager.instance.gameHasStarted = false;
        ActiveGameManager.instance.gameComplete = false;
        LevelManager.instance.LoadLevel("MainScene", 0.1f, -1);
    }

    public void QuitGame()
    {
        Application.Quit(0);
    }
}
