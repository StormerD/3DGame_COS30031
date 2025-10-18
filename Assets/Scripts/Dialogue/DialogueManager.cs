using System;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    public event Action<DialogueScene> OnStartDialogue;

    void Awake()
    {
        if (instance != null) Destroy(gameObject);
        else instance = this;
    }

    public void StartNewDialogue(DialogueScene scene)
    {
        OnStartDialogue?.Invoke(scene);
    }
}
