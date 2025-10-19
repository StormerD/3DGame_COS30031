using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// this manager dispatches the currently furthest unlocked level to all of the loaders.
/// realistically this could be bypassed by having each level loader subscribe directly to
/// the saving system, which i considered, but i think it's better to have the level loaders
/// as fully separate from the save system as possible. having a central manager instead of
/// multiple subscriptions to OnSaveDataChanged is better imo.
/// </summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public event Action<int> OnLevelUnlockChanged;
    public event Action OnForgeHasBeenOpened;
    public GameObject player;
    private ILooter _pLooter;
    private IFighter _pWeaponHandler;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    void Start()
    {
        SaveManager.instance.OnSaveDataChanged += LoadLevels;
        if (player == null) Debug.LogWarning("LevelManager player is null.");
        if (!player.TryGetComponent(out _pLooter)) Debug.LogWarning("Player is missing PlayerLooter.");
        if (!player.TryGetComponent(out _pWeaponHandler)) Debug.LogWarning("Player is missing WeaponHandler.");
    }

    void LoadLevels()
    {
        int latestUnlock = SaveManager.instance.GetFurthestUnlockedLevel();
        string equippedWeapon = SaveManager.instance.GetEquippedWeapon();
        bool hasWeaponBeenEquipped = equippedWeapon != null && equippedWeapon != "";

        OnLevelUnlockChanged?.Invoke(latestUnlock);
        if (hasWeaponBeenEquipped) ForgeOpened();
    }

    public void LoadLevel(string which, float minLoadTime, int levelNum)
    {
        // if going to a scene other than the main menu, save (this prevents everything from breaking.)
        if (levelNum != -1) SaveManager.instance.Save(ActiveGameManager.instance.saveSlot);
        if (SceneUtility.GetBuildIndexByScenePath(which) != -1) StartCoroutine(AsyncLoader(which, minLoadTime, levelNum));
        else Debug.LogWarning("Tried to load unknown scene: " + which);
    }

    IEnumerator AsyncLoader(string loader, float loadTime, int levelNum)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(loader);
        op.allowSceneActivation = false;

        yield return new WaitForSeconds(loadTime);
        while (op.progress < 0.89)
        {

            yield return null; // just wait until it's finished now, if we've already passed minimum time
        }

        // check values that need to be saved before transitioning to new scene
        if (ActiveGameManager.instance != null)
        {
            ActiveGameManager.instance.common = _pLooter.GetCurrency(CurrencyType.COMMON);
            ActiveGameManager.instance.rare = _pLooter.GetCurrency(CurrencyType.RARE);
            ActiveGameManager.instance.mythic = _pLooter.GetCurrency(CurrencyType.MYTHIC);

            GameObject equipped = _pWeaponHandler.GetEquippedWeaponObject();
            if (equipped == null) equipped = Instantiate(ForgeManager.instance.GetWeaponByID("melee1")); // hardcoded default
            ActiveGameManager.instance.equippedWeapon = equipped;
            equipped.transform.parent = ActiveGameManager.instance.transform;

            ActiveGameManager.instance.activeLevel = levelNum;
        }

        op.allowSceneActivation = true;
    }

    public void ForgeOpened() => OnForgeHasBeenOpened?.Invoke();
}
