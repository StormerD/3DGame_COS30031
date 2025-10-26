using System.Collections;
using UnityEngine;
/// <summary>
/// this script is a base for objects that watch for key gameplay events and trigger dialogue based on them
/// the specific conditions around triggering must be handled by the implementing class
/// as well as which dialogue gets triggered, but they can use the utility provided
/// by this class to help 
/// </summary>
public abstract class DialogueEventWatcherBase : MonoBehaviour
{
    private bool _dialogueSynced = false;
    protected virtual void Start()
    {
        SyncDialogue();
        if (GameManager.instance != null)
        {
            GameManager.instance.OnLoadComplete += SyncDialogue;
        }
        else Debug.LogWarning("GameManager null; Dialogue does not know save state.");
        if (DialogueScreen.instance != null)
        {
            DialogueScreen.instance.OnWhichDialogueEnded += SomeDialogueCompleted;
        }
        else Debug.LogError("Missing dialogue screen!");
    }
    protected IEnumerator DelayedRequestDialogue(DialogueScene scene, float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        if (!_dialogueSynced) SyncWrapper();
        DialogueScreen.instance.RunScene(scene);
    }
    protected bool HasSceneAlreadyHappened(string which)
    {
        return GameManager.instance != null && GameManager.instance.GetDialogueHasBeenPlayed(which);
    }
    private void SyncWrapper()
    {
        SyncDialogue();
        _dialogueSynced = true;
    }
    protected abstract void SyncDialogue();
    protected abstract void SomeDialogueCompleted(string which);
}