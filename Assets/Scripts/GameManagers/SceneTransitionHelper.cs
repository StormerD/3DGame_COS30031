using System;
using UnityEngine;

public class SceneTransitionHelper : MonoBehaviour
{
    public static event Action OnFadeInComplete;
    private static bool exists; 
    private Animator _transitionAnimator;

    void Awake()
    {
        if (exists) Destroy(gameObject);

        exists = true;
        _transitionAnimator = GetComponentInChildren<Animator>();
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (GameManager.instance != null) GameManager.instance.OnDoneLoadingNewScene += EntryFadeOut;
        AsyncSceneLoader.OnBegunLoadingNewScene += ExitFadeIn;

        EntryFadeOut();
    }

    void OnDestroy() => AsyncSceneLoader.OnBegunLoadingNewScene -= ExitFadeIn;

    void EntryFadeOut() => _transitionAnimator.SetTrigger("PlayEntryFade");
    void ExitFadeIn() => _transitionAnimator.SetTrigger("PlayExitFade");

    void EmitFadeInComplete() { OnFadeInComplete?.Invoke(); }
}