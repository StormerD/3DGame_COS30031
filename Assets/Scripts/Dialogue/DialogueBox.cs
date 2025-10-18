using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Animator))]
public class DialogueBox : MonoBehaviour, IPointerClickHandler
{
    private TMP_Text _textbox;
    private Animator _textboxAnimator;
    private bool _goNextScript = false;
    private bool _dialoguePrinting = false;
    private bool _fastWriteDialogue = false;

    void Awake()
    {
        _textbox = GetComponentInChildren<TMP_Text>();
        _textboxAnimator = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (DialogueManager.instance != null)
        {
            DialogueManager.instance.OnStartDialogue += WriteDialogueScene;
        }
    }

    public void OnPointerClick(PointerEventData ped)
    {
        if (_dialoguePrinting) _fastWriteDialogue = true; // if still printing then just write the rest of the text to the box
        else _goNextScript = true;
    }

    private void ResetTextBox() => _textbox.text = "";

    private void WriteDialogueScene(DialogueScene scene)
    {
        StartCoroutine(RunDialogueScene(scene));
    }
    
    IEnumerator RunDialogueScene(DialogueScene scene)
    {
        foreach(var script in scene.scripts)
        {
            StartCoroutine(PrintTextToBox(script));
            yield return new WaitUntil(() => _goNextScript == true);
            _goNextScript = false;
            _fastWriteDialogue = false;
        }
    }

    IEnumerator PrintTextToBox(DialogueBoxScript parameters)
    {
        ResetTextBox();
        WaitForSeconds charWriteWait = new(parameters.letterAppearanceTime);

        // print the script to the box based on its parameters; if a click happens we auto-write all dialogue
        Debug.Log("Printing text to the box: " + parameters.text);
        _dialoguePrinting = true;
        foreach (char c in parameters.text)
        {
            _textbox.text += c;
            yield return charWriteWait;
            if (_fastWriteDialogue && parameters.canSkipPrinting)
            {
                _textbox.text = parameters.text;
                break;
            }
        }
        _dialoguePrinting = false;

        // Now wait to auto-proceed or until a click
        float timer = 0;
        while (timer < parameters.goToNextDialogueAfter)
        {
            timer += Time.deltaTime;
            if (_goNextScript) yield break; // means the dialogue was clicked, so we depart to avoid skipping the next script
            yield return null;
        }
        _goNextScript = true;
    }
}
