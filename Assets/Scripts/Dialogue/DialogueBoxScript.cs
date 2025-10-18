using UnityEngine;

[CreateAssetMenu(fileName = "DialogueBoxScript", menuName = "Scriptable Objects/DialogueBoxScript")]
public class DialogueBoxScript : ScriptableObject
{
    [Header("Configuration for what is actually displayed")]
    [Tooltip("the dialogue text, don't make it too long!")]
    public string text;
    public Texture2D speakerImage;
    public string speakerName = "???";
    [Tooltip("If true, the speaker name and image will be displayed to the left of the textbox. If false, they will be on the right")]
    public bool speakerShowsOnLeft = true;

    [Header("Configuration around text appearance / speed")]
    [Tooltip("The amount of time it takes for one letter to appear.")]
    public float letterAppearanceTime;
    [Tooltip("If this is true, then clicking the dialogue box will allow the rest of the text to appear")]
    public bool canSkipPrinting = true;
    [Tooltip("After this text finishes displaying, the scene will wait this long before going to the next dialogue")]
    public float goToNextDialogueAfter = 5f;

    [Header("Other")]
    public AudioClip syncedAudio;

}
