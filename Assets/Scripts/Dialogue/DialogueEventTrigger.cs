

using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueEventTrigger", menuName = "Scriptable Objects/DialogueEventTrigger")]
public class DialogueEventTrigger : ScriptableObject
{
    public EventObject eventStream;
    public DialogueScene dialogueScene;
    public float dialogueRequestDelay = 0;
    private event Action OnEventHappened;
    private event Action<DialogueScene> Request;
    private bool _hasTriggered;
    public bool GetHasTriggered() => _hasTriggered;
    public void SetHasTriggered(bool to) { _hasTriggered = to; }
    public void DoRegistration()
    {
        OnEventHappened += Triggered;
        eventStream.RegisterListener(OnEventHappened);
    }
    public void Unregister() => eventStream.UnregisterListener(OnEventHappened);
    private void Triggered() { if (!_hasTriggered) Request?.Invoke(dialogueScene); } 
    public void ConnectRequest(Action<DialogueScene> invoker) { Request += invoker; }
}