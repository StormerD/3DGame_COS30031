using System.Collections.Generic;
using UnityEngine;

public class LevelDialogueEventWatcherBase : DialogueEventWatcherBase
{
    public DialogueScene playOnLevelEnter;
    public float levelEnterDialogueDelay;
    private bool _hasPlayedLevelEnter = false;
    public List<DialogueEventTrigger> otherEvents;

    protected override void Start()
    {
        base.Start();
        if (!_hasPlayedLevelEnter) StartCoroutine(DelayedRequestDialogue(playOnLevelEnter, levelEnterDialogueDelay));
    }

    protected override void SyncDialogue()
    {
        _hasPlayedLevelEnter = HasSceneAlreadyHappened(playOnLevelEnter.dialogueKey);
        foreach (var e in otherEvents)
        {
            e.SetHasTriggered(HasSceneAlreadyHappened(e.dialogueScene.dialogueKey));
            e.DoRegistration();
            e.ConnectRequest(RequestDialogue);
        }
    }

    private void RequestDialogue(DialogueScene what)
    {
        StartCoroutine(DelayedRequestDialogue(what));
    }

    protected override void SomeDialogueCompleted(string which)
    {
        if (which == playOnLevelEnter.dialogueKey)
        {
            _hasPlayedLevelEnter = true;
        }
        else
        {
            foreach (var e in otherEvents)
            {
                if (e.dialogueScene.dialogueKey != which) continue;
                Debug.Log("Setting " + e.dialogueScene.dialogueKey + " to triggered!");
                e.SetHasTriggered(true);
                break;
            }
        }
    }
    
    void OnDestroy()
    {
        foreach(var e in otherEvents) // we re-use some event objects between scenes, better safe than sorry and unsubscribe here
        {
            e.Unregister();
        }
    }
}