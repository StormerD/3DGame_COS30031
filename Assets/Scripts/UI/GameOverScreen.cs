using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private BasicEventObject _playerDeathStream;

    void Awake() => _playerDeathStream.RegisterListener(OpenMenu);
    void OnDestroy() =>  _playerDeathStream.UnregisterListener(OpenMenu);

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
