using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// this manager dispatches the currently furthest unlocked level to all of the loaders.
/// realistically this could be bypassed by having each level loader subscribe directly to
/// the saving system, which i considered, but i think it's better to have the level loaders
/// as fully separate from the save system as possible. having a central manager instead of
/// multiple subscriptions to SaveManager is better imo.
/// </summary>
public class LevelLoaderManager : MonoBehaviour
{
    public static LevelLoaderManager instance;
    public event Action<int> OnLevelUnlockChanged;
    public event Action OnForgeHasBeenOpened;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (ForgeManager.instance != null) ForgeManager.instance.OnForgeOpened += ForgeOpened;
        SyncUnlockedLevels();
    }

    public void SyncUnlockedLevels()
    {
        if (GameManager.instance == null) { Debug.LogWarning("GameManager null; levels will not load normally."); return; }

        StartCoroutine(WaitUntilAllLevelLoadersSubscribed());
    }

    private void ForgeOpened() => OnForgeHasBeenOpened?.Invoke();

    IEnumerator WaitUntilAllLevelLoadersSubscribed(float timeout = 3f)
    {
        int levelMax = GameManager.LEVEL_AMOUNT;
        float startTime = Time.time;
        yield return new WaitUntil(() => (OnLevelUnlockChanged != null && OnLevelUnlockChanged?.GetInvocationList().Length == levelMax) || Time.time > startTime + timeout);

        int latestUnlock = GameManager.instance.GetFurthestUnlockedLevel();
        string equippedWeapon = GameManager.instance.GetEquippedWeapon();
        bool hasWeaponBeenEquipped = equippedWeapon != null && equippedWeapon != "";

        OnLevelUnlockChanged?.Invoke(latestUnlock);
        if (hasWeaponBeenEquipped) ForgeOpened();
    }
}
