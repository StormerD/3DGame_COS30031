using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AsyncSceneLoader : MonoBehaviour
{
    void Start()
    {
        SceneTransitionHelper.OnFadeInComplete += AllowFinishLoading;
    }

    public static event Action OnBegunLoadingNewScene;
    private static bool finishLoading = false;

    private void AllowFinishLoading() { finishLoading = true; }
    public static IEnumerator AsyncLoad(string loadScene)
    {
        OnBegunLoadingNewScene?.Invoke();

        yield return new WaitUntil(() => finishLoading);
        finishLoading = false;

        AsyncOperation op = SceneManager.LoadSceneAsync(loadScene);
        op.allowSceneActivation = false;

        while (op.progress < 0.89) yield return null; // just wait until it's finished now, if we've already passed minimum time

        op.allowSceneActivation = true;
    }
}
