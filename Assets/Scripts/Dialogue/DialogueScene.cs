using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueScene", menuName = "Scriptable Objects/DialogueScene")]
public class DialogueScene : ScriptableObject
{
    [Tooltip("A UNIQUE key to identify this scene with. This is how dialogue gets saved!")]
    public string dialogueKey;
    [Tooltip("In order, the dialogue text that will be displayed to the player.")]
    public List<DialogueBoxScript> scripts;
}
