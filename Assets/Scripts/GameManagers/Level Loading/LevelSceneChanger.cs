using System;
using TMPro;
using UnityEngine;

public class LevelSceneChanger : MonoBehaviour
{
    public string sceneToLoad;
    public int levelNum = -1; // If -1, will always be unlocked
    public float minimumLoadingTime = 1; // in case we want to do transitions. if we don't then just set this to 0
    public event Action LoadingScene; // to subscribe to for transitions or other things we may need to do
    public event Action<bool> OnUnlockedChanged;
    public SceneLoaderTriggerHandler sceneEntry;
    public LevelLockedUI visuals;
    [SerializeField] TMP_Text lockedText;

    private bool _levelUnlocked = true;
    private bool _canLoad = true;
    private bool _forgeOpened = false;
    private int _furthestUnlock = 0;

    void Awake()
    {
        if (sceneEntry == null) Debug.LogError(transform.name + " missing SceneLoaderTriggerHandler ref.");
        if (visuals == null) Debug.LogError(transform.name + " Missing visuals ref");
        OnUnlockedChanged += sceneEntry.ChangeTrigger;
        OnUnlockedChanged += visuals.SetUnlocked;
        sceneEntry.OnEnterLoadZone += Load;
    }

    void Start()
    {
        if (LevelManager.instance != null)
        {
            LevelManager.instance.OnForgeHasBeenOpened += ForgeHasBeenOpened;
            LevelManager.instance.OnLevelUnlockChanged += LevelUnlockDataChanged;
        }
    }

    public void Load()
    {
        if (_levelUnlocked && _canLoad)
        {
            _canLoad = false; // prevent multiple loads
            LoadingScene?.Invoke();
            LevelManager.instance.LoadLevel(sceneToLoad, minimumLoadingTime, levelNum);
        }
    }

    private void LevelUnlockDataChanged(int furthestUnlocked)
    {
        // lockedText == null since this script is also used for level loaders to go back home and we don't care if the forge has been opened there
        if (furthestUnlocked >= levelNum && (_forgeOpened || lockedText == null)) _levelUnlocked = true;
        else if (!_forgeOpened && lockedText != null && furthestUnlocked >= levelNum) // last condition (furthestUnlocked >= levelNum) to display the level's locked reason over forge not being opened
        {
            lockedText.text = "Open the forge and equip a weapon first!"; // we won't actually check that they equipped a weapon (since we auto-equip anyway) but they should at least open the forge before leaving home
            _levelUnlocked = false;
        }
        else _levelUnlocked = false;

        _furthestUnlock = furthestUnlocked;
        OnUnlockedChanged?.Invoke(_levelUnlocked);
    }

    private void ForgeHasBeenOpened()
    {
        _forgeOpened = true;
        LevelUnlockDataChanged(_furthestUnlock);
    }
}
