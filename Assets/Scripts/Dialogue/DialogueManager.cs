using System;
using TMPro;
using UnityEngine;

/// <summary>
/// This is an implementation for a currently single-sided self dialogue.
/// It can be triggered by calling RunDialogue()
/// </summary>
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    public event Action<DialogueScene> OnStartDialogue;
    [Tooltip("The dialogue box is where dialogue will get displayed. It should have ONE TMP_Text element somewhere in its children.")]
    public DialogueBox dialogueBox;

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
