using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueScene", menuName = "Scriptable Objects/DialogueScene")]
public class DialogueScene : ScriptableObject
{
    [Tooltip("In order, the dialogue text that will be displayed to the player.")]
    public List<DialogueBoxScript> scripts;
}
