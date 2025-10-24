public class HomeDialogueEventWatcher : DialogueEventWatcherBase
{
    public DialogueScene introductionScene;
    public float introductionSceneStartDelay = 3f;
    private bool _introductionPlayed = false;
    public DialogueScene forgeClosedFirstTimeScene;
    private bool _forgeClosedPlayed = false;
    public DialogueScene winScene;
    public float winDialogueDelay;
    private bool _winPlayed = false;

    protected override void Start()
    {
        base.Start();
        ForgeManager.instance.OnForgeClosed += PlayForge;
        if (!_introductionPlayed) StartCoroutine(DelayedRequestDialogue(introductionScene, introductionSceneStartDelay));
        if (GameManager.instance != null && GameManager.instance.GetGameComplete() && !_winPlayed)
        {
            StartCoroutine(DelayedRequestDialogue(winScene, winDialogueDelay));
        }
    }

    // with the current save slot, check which of the scenes this dialogue watcher is responsible for
    // have been played before
    protected override void SyncDialogue()
    {
        _introductionPlayed = HasSceneAlreadyHappened(introductionScene.dialogueKey);
        _forgeClosedPlayed = HasSceneAlreadyHappened(forgeClosedFirstTimeScene.dialogueKey);
        _winPlayed = HasSceneAlreadyHappened(winScene.dialogueKey);
    }

    private void PlayForge()
    {
        if (!_forgeClosedPlayed) StartCoroutine(DelayedRequestDialogue(forgeClosedFirstTimeScene));
    }

    protected override void SomeDialogueCompleted(string which)
    {
        if (which == introductionScene.dialogueKey)
        {
            _introductionPlayed = true;   
        } else if (which == forgeClosedFirstTimeScene.dialogueKey)
        {
            _forgeClosedPlayed = true;
        } else if (which == winScene.dialogueKey)
        {
            _winPlayed = true;
        }
    }
}