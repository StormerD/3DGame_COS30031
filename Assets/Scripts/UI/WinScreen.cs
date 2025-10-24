using UnityEngine;
using UnityEngine.SceneManagement;

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
        if (GameManager.instance != null && GameManager.instance.GetGameComplete()) gameObject.SetActive(true);
        else gameObject.SetActive(false);
    }

    public void ContinuePlaying()
    {
        gameObject.SetActive(false);
    }

    public void QuitToMainMenu()
    {
        if (GameManager.instance == null) Debug.LogWarning("Unable to save!");
        else { GameManager.instance.SafeSave(); SceneManager.LoadScene(0); }
    }
}
