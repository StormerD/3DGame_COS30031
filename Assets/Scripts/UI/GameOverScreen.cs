using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private BasicEventObject _playerDeathStream;
    [SerializeField] private BasicEventObject _playerUndeathStream;

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

    public void RestartLevel()
    {
        AsyncSceneLoader.OnBegunLoadingNewScene += RaiseUndeathEvent;
        StartCoroutine(AsyncSceneLoader.AsyncLoad(SceneManager.GetActiveScene().buildIndex));
        _playerUndeathStream.RaiseEvent();
    }
    private void RaiseUndeathEvent() => _playerUndeathStream.RaiseEvent();

    public void SaveAndQuit()
    {
        if (GameManager.instance == null) Debug.LogError("Cannot save: SaveManager instance is null.");
        else
        {
            GameManager.instance.SaveAndClearGame();
            StartCoroutine(AsyncSceneLoader.AsyncLoad(0));
        }
    }
}
