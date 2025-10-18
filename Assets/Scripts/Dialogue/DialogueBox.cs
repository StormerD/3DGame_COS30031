using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class DialogueBox : MonoBehaviour, IPointerClickHandler
{
    #region vars
    [Header("Blank image is for speakers")]
    [SerializeField] private TMP_Text _dialogueTextBox;
    public Texture2D defaultBlankImage;
    [Header("Left speaker")]
    [SerializeField] private RawImage _leftSpeakerImage;
    [SerializeField] private GameObject _leftSpeakerNameBox;
    [SerializeField] private TMP_Text _leftSpeakerNameText;
    [Header("Right speaker")]
    [SerializeField] private RawImage _rightSpeakerImage;
    [SerializeField] private GameObject _rightSpeakerNameBox;
    [SerializeField] private TMP_Text _rightSpeakerNameText;

    private Animator _textboxAnimator;
    private float _boxEntryTime;
    private float _boxExitTime;
    private float _transitionEnterTime;
    private float _transitionExitTime;

    private bool _goNextScript = false;
    private bool _dialoguePrinting = false;
    private bool _fastWriteDialogue = false;
    private bool _playerCanClick = false;

    #endregion

    void Awake()
    {
        _textboxAnimator = GetComponent<Animator>();
        AnimationClip[] clips = _textboxAnimator.runtimeAnimatorController.animationClips;
        _boxEntryTime = clips.First(clip => clip.name == "Entry").length;
        _boxExitTime = clips.First(clip => clip.name == "Exit").length;
        _transitionEnterTime = clips.First(clip => clip.name == "TransitionEntry").length;
        _transitionExitTime = clips.First(clip => clip.name == "TransitionExit").length;
        if (defaultBlankImage == null) Debug.LogWarning("Dialogue box missing blank texture image! Speaker sprites will display oddly.");
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
        if (!_playerCanClick) return;

        if (_dialoguePrinting) _fastWriteDialogue = true; // if still printing then just write the rest of the text to the box
        else _goNextScript = true;
    }

    public void ResetTextBox() => _dialogueTextBox.text = "";

    private void WriteDialogueScene(DialogueScene scene)
    {
        StartCoroutine(RunDialogueScene(scene));
    }

    private void ToggleLeftSpeaker(bool on, DialogueBoxScript script)
    {
        _leftSpeakerImage.texture = on ? script.speakerImage : defaultBlankImage;
        _leftSpeakerNameText.text = on ? script.speakerName : "";
        _leftSpeakerNameBox.SetActive(on);
    }
    
    private void ToggleRightSpeaker(bool on, DialogueBoxScript script)
    {
        _rightSpeakerImage.texture = on ? script.speakerImage : defaultBlankImage;
        _rightSpeakerNameText.text = on ? script.speakerName : "";
        _rightSpeakerNameBox.SetActive(on);
    }
    
    IEnumerator RunDialogueScene(DialogueScene scene)
    {
        for (int i = 0; i < scene.scripts.Count; i++)
        {
            ResetTextBox();
            DialogueBoxScript cur = scene.scripts[i];

            // set the left / right side speaker and image 
            bool isLeft = cur.speakerShowsOnLeft;
            ToggleLeftSpeaker(isLeft, cur);
            ToggleRightSpeaker(!isLeft, cur);

            if (i == 0)
            {
                _textboxAnimator.SetTrigger("EnterDialogue");
                yield return new WaitForSeconds(_boxEntryTime);
            }
            else
            {
                _textboxAnimator.SetTrigger("TransitionEnter");
                yield return new WaitForSeconds(_transitionEnterTime);
            }

            _playerCanClick = true;

            StartCoroutine(PrintTextToBox(scene.scripts[i]));
            yield return new WaitUntil(() => _goNextScript == true);
            _playerCanClick = false;

            if (i != scene.scripts.Count - 1)
            {
                _textboxAnimator.SetTrigger("TransitionExit");
                yield return new WaitForSeconds(_transitionExitTime);
            }
            else
            {
                _textboxAnimator.SetTrigger("ExitDialogue");
                yield return new WaitForSeconds(_boxExitTime);
            }

            _goNextScript = false;
            _fastWriteDialogue = false;
        }
    }

    IEnumerator PrintTextToBox(DialogueBoxScript parameters)
    {
        WaitForSeconds charWriteWait = new(parameters.letterAppearanceTime);

        // print the script to the box based on its parameters; if a click happens we auto-write all dialogue
        _dialoguePrinting = true;
        foreach (char c in parameters.text)
        {
            _dialogueTextBox.text += c;
            yield return charWriteWait;
            if (_fastWriteDialogue && parameters.canSkipPrinting)
            {
                _dialogueTextBox.text = parameters.text;
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
