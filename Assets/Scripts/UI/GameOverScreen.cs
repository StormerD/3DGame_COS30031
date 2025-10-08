using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    public static GameOverScreen instance;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        gameObject.SetActive(false);
    }

    public void OpenMenu()
    {
        gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        ActiveGameManager.instance.gameHasStarted = false;
        LevelManager.instance.LoadLevel("MainScene", 0.1f, -1);
    }

    public void QuitGame()
    {
        Application.Quit(0);
    }
}
