using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class ObjectiveData
{
    public string sceneName;
    public string objectiveName;
}

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager instance;

    [Header("References")]
    public ObjectiveArrow arrow;

    [Header("Objectives")]
    public ObjectiveData[] objectives;

    private int currentIndex = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SetNextObjective();
    }

    public void SetNextObjective()
    {
        if (currentIndex >= objectives.Length)
        {
            if (arrow != null)
                arrow.gameObject.SetActive(false);
            Debug.Log("All objectives complete");
            return;
        }

        ObjectiveData next = objectives[currentIndex];
        currentIndex++;

        // Check if objective is in current scene
        if (SceneManager.GetActiveScene().name == next.sceneName)
        {
            GameObject obj = GameObject.Find(next.objectiveName);
            if (obj != null)
                arrow.target = obj.transform;
        }
        else
        {
            // Load the scene additive if different
            SceneManager.LoadSceneAsync(next.sceneName, LoadSceneMode.Additive)
                .completed += (op) =>
                {
                    GameObject objInScene = GameObject.Find(next.objectiveName);
                    if (objInScene != null)
                        arrow.target = objInScene.transform;
                };
        }

        arrow.gameObject.SetActive(true);
        Debug.Log("New objective set: " + next.objectiveName);
    }
}
