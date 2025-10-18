using UnityEngine;

[CreateAssetMenu(fileName = "DialogueBoxScript", menuName = "Scriptable Objects/DialogueBoxScript")]
public class DialogueBoxScript : ScriptableObject
{
    [Tooltip("the dialogue text, don't make it too long!")]
    public string text;
    [Tooltip("The amount of time it takes for one letter to appear.")]
    public float letterAppearanceTime;
    [Tooltip("If this is true, then clicking the dialogue box will allow the rest of the text to appear")]
    public bool canSkipPrinting = true;
    [Tooltip("After this text finishes displaying, the scene will wait this long before going to the next dialogue")]
    public float goToNextDialogueAfter = 5f;
}
