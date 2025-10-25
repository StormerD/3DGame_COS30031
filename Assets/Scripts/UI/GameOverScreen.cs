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

    public void RestartGame() // Todo: improve this UI. Restart should probably restart the level, not the game?
    {
        
    }

    public void QuitGame()
    {
        Application.Quit(0);
    }
}
