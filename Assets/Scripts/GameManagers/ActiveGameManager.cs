
using UnityEngine;

// This class will always stay loaded between scenes. It holds information about the current game, like 
// which slot it was saved to, which weapon was equipped, and how much currency the player has.
// really it's not a class but more of a glorified container for data.
public class ActiveGameManager : MonoBehaviour
{
    public static ActiveGameManager instance;
    private const int LAST_LEVEL = 3;

    public int saveSlot;
    public GameObject equippedWeapon;
    public int common;
    public int rare;
    public int mythic;
    public int lastCompleteLevel;
    public int activeLevel;
    public bool gameHasStarted;
    public bool gameComplete;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    // same thing could be achieved by directly setting values, but this just makes the job a little clearer 
    // and easier
    public void LevelComplete(int which)
    {
        lastCompleteLevel = which;
        if (lastCompleteLevel == LAST_LEVEL) gameComplete = true; // will trigger Win screen on next entry to Home
        SaveManager.instance.SafeSaveWithLevel(saveSlot, which + 1);
    }

    public void CurrentLevelComplete() => LevelComplete(activeLevel);
}