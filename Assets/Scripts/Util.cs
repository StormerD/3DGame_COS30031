using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Util
{
    public static IEnumerator AsyncLoader(string loadScene, float delay = 0, float loadTime = 0)
    {
        yield return new WaitForSeconds(delay);

        AsyncOperation op = SceneManager.LoadSceneAsync(loadScene);
        op.allowSceneActivation = false;

        yield return new WaitForSeconds(loadTime);
        while (op.progress < 0.89) yield return null; // just wait until it's finished now, if we've already passed minimum time

        op.allowSceneActivation = true;
    }
}
