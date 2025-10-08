using System;
using UnityEngine;

/// <summary>
/// Originally was going to use an animator to switch between scenes, but realized that
/// a script works just as well, since it's just disabling / enabling objects. I still used an animator
/// for the transitions of the menu dropping on/ off the screen, but not for switching between screens.
/// </summary>
[RequireComponent(typeof(Animator))]
public class MainMenuManager : MonoBehaviour, IMenuPausable
{
    public GameObject startMenu;
    public GameObject explanationMenu;
    public GameObject savesMenu;
    // When the game starts, we will use the "main menu" as a pause menu, allowing players
    // to save whenever they want, so need a reference to saveButtons to enable them once gameplay starts
    // and saveMenuGobackButton to disable once game starts
    public GameObject saveButtons;
    public GameObject saveMenuGoBackButton;

    private Animator _animator;
    private bool _gameHasStarted = false;

    void Start()
    {
        _animator = GetComponent<Animator>();
        if (ActiveGameManager.instance != null) _gameHasStarted = ActiveGameManager.instance.gameHasStarted;
        PauseManager.instance.OnPause += OpenMenu;
        PauseManager.instance.OnUnpause += CloseMenu;
        if (ActiveGameManager.instance.gameHasStarted)
        {
            ConvertToPauseMenu();
        }
        else
        {
            _animator.SetTrigger("GameStart");
            PauseManager.instance.Pause();
        }
    }

    public void GoToExplanationMenu()
    {
        startMenu.SetActive(false);
        explanationMenu.SetActive(true);
    }

    public void GoToStartMenu()
    {
        explanationMenu.SetActive(false);
        savesMenu.SetActive(false);
        startMenu.SetActive(true);
    }

    public void GoToSavesMenu()
    {
        startMenu.SetActive(false);
        savesMenu.SetActive(true);
    }

    public void SaveToSlot(int which)
    {
        SaveManager.instance.Save(which);
    }

    private void ConvertToPauseMenu()
    {
        GoToSavesMenu();
        saveMenuGoBackButton.SetActive(false);
        saveButtons.SetActive(true);
    }

    public void StartGame(int slot)
    {
        Debug.Log("Loading game... slot " + slot);
        _gameHasStarted = true;
    
        // Load data
        SaveManager.instance.LoadFromSlot(slot);
        ActiveGameManager.instance.gameHasStarted = true;

        // Unpause the game -- bit of back and forth here, but since things subscribe to PauseManager for events (because the PauseMenu will be different between scenes)
        // we want to make sure that OnUnpause() event is emitted
        PauseManager.instance.Unpause();
    }

    public void CloseMenu()
    {
        // can't close the menu when the game hasn't started yet!
        if (_gameHasStarted) _animator.SetTrigger("CloseMenu");
    }
    public void OpenMenu()
    {
        if (!_gameHasStarted) return;
        // When the "main" menu is being opened during game, it should be as a pause menu
        ConvertToPauseMenu();
        _animator.SetTrigger("OpenMenu");
    }
    public void QuitGame() => Application.Quit();
}
