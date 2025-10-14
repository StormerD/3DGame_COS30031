#pragma warning disable CS0162

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This manager is based on the Saving system provided in the modules, and uses a lot of the code from that. 
/// Instead of saving player position, it saves:
/// 1. Currency (points)
/// 2. Furthest unlocked level
/// 3. Forge purchases & unlocks
/// 4. Current player-equipped weapon
/// 5. slot index
/// Technically there can be infinite save slots, but the menu only allows 3. I also use the fourth "slot" to save metadata about the
/// saves themselves (like when they were last saved) but not any game-related data. That way to display
/// when the slots were most recently saved you don't have to reload the entire JSON. Not super important in this game
/// since the save data is small anyway, but I think it's a good idea in case we add more save data in the future
/// </summary>
public class SaveManager : MonoBehaviour
{
	public static SaveManager instance;
	public event Action OnSaveDataChanged;
	public event Action OnSlotTimesUpdated;
	public int slotIndex = 0;     // set in Inspector to test different slots
	public GameObject player;

	private ILooter _playerLooter;
	private IFighter _playerWeaponHandler;
	private SaveData _currentSaveData;
	private SlotTimesData _slotTimes;

	private const bool VERBOSE = false;

	#region Unity Functions
	void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(gameObject);
		}
		else
		{
			instance = this;
		}
	}

	void Start()
	{
		if (!player.TryGetComponent(out _playerLooter)) Debug.LogWarning("Save manager needs Player to have PlayerLooter!");
		if (!player.TryGetComponent(out _playerWeaponHandler)) Debug.LogWarning("Save manager needs Player to have PlayerWeaponHandler!");
		// If the game is ongoing and we are re-entering the main scene, load from the active scene
		if (ActiveGameManager.instance != null && ActiveGameManager.instance.gameHasStarted)
		{
			LoadFromSlot(ActiveGameManager.instance.saveSlot);
		}
	}
	#endregion

	#region Saving
	public void Save(int to) => DoSave(to, GetFurthestUnlockedLevel());
	public void SafeSave(int to) => SafeSaveWithLevel(to, GetFurthestUnlockedLevel());
	public void SafeSaveWithLevel(int to, int furthestLevel) // Confirms that the slot's data is loaded before saving.
	{
		SyncSlotDataWithoutEmittingEvents(to);
		DoSave(to, furthestLevel);
	}
	void DoSave() => DoSave(slotIndex, _currentSaveData.furthestUnlockedLevel);
	void DoSave(int sIndex, int furthestUnlockedLevel)
	{
		var saveData = new SaveData
		{
			slotIndex = sIndex,
			furthestUnlockedLevel = furthestUnlockedLevel,
			currency = _playerLooter.GetSaveableCurrency(),
			equippedWeapon = _playerWeaponHandler.GetEquippedWeapon(),
			weaponsPurchased = ForgeManager.instance != null ? ForgeManager.instance.GetWeaponPurchaseData() : _currentSaveData.weaponsPurchased
		};
		SaveSystem.Save(saveData);
		_currentSaveData = saveData;
		Debug.Log($"Saved slot {sIndex}");
		SaveSlotTimes(sIndex);
	}

	void SaveSlotTimes(int which)
	{
		long now = DateTime.Now.Ticks;
		var timeData = new SlotTimesData
		{
			slotOneLastSaveTime = which == 1 ? now : GetSlotLastSavedTime(1)?.Ticks ?? 0,
			slotTwoLastSaveTime = which == 2 ? now : GetSlotLastSavedTime(2)?.Ticks ?? 0,
			slotThreeLastSaveTime = which == 3 ? now : GetSlotLastSavedTime(3)?.Ticks ?? 0,
		};
		SaveSystem.SaveSlotTimes(timeData);
		_slotTimes = timeData;
		if (VERBOSE) Debug.Log("Updated slot saved times");
		OnSlotTimesUpdated?.Invoke();
	}
	#endregion

	#region Loading

	public void LoadFromSlot(int which)
	{
		slotIndex = which;
		if (VERBOSE) Debug.Log("Loading slot: " + which);
		DoLoad();
	}
	void DoLoad()
	{
		if (slotIndex == 0) return;

		if (SaveSystem.TryLoad(slotIndex, out _currentSaveData)) Debug.Log($"Loaded slot {slotIndex}");
		else
		{
			// Set to default data (new game)
			Debug.Log("Doing default load data for slot " + slotIndex);
			_currentSaveData = new SaveData
			{
				slotIndex = slotIndex,
				furthestUnlockedLevel = 1,
				currency = new PlayerCurrency { common = 0, rare = 0, mythic = 0 },
				equippedWeapon = "",
				weaponsPurchased = ForgeManager.instance != null ? ForgeManager.instance.GetWeaponPurchaseData() : new()
			};
		}
		ActiveGameManager.instance.saveSlot = slotIndex;
		StartCoroutine(DelayedEmit(OnSaveDataChanged, 0.2f));
	}
	private void LoadSlotTimes()
	{
		if (SaveSystem.TryLoadSlotTimes(out _slotTimes)) { if (VERBOSE) Debug.Log($"Loaded timeslots!"); }
		else
		{
			_slotTimes = new SlotTimesData
			{
				slotOneLastSaveTime = 0,
				slotTwoLastSaveTime = 0,
				slotThreeLastSaveTime = 0
			};
		}
	}
	private bool ConfirmDataLoaded()
	{
		if (slotIndex < 1 || slotIndex > 3) { if (VERBOSE) Debug.Log("Loading slot not set."); return false; }
		if (VERBOSE)
		{
			Debug.Log("Confirming data from slot " + slotIndex + " is loaded.");
			Debug.Log("Before: " + (_currentSaveData != null ? _currentSaveData.equippedWeapon : "null"));
		}
		if (_currentSaveData == null) { Debug.Log("Loading!"); DoLoad(); }

		if (VERBOSE) Debug.Log("After: " + (_currentSaveData != null ? _currentSaveData.equippedWeapon : "null"));

		return _currentSaveData != null; // possible that loading failed 
	}
	private bool ConfirmSlotTimesLoaded()
	{
		LoadSlotTimes();
		return _slotTimes != null;
	}

	public void SyncSlotDataWithoutEmittingEvents(int which)
	{
		if (SaveSystem.TryLoad(which, out _currentSaveData) && VERBOSE) { Debug.Log($"Loaded slot {which}"); }
		else if (VERBOSE) { Debug.Log("Failed to sync."); }
	}

	#endregion

	#region Getters
	// separate distinct pieces into individual getters so that components can get as needed (no reason for scene manager to have anything but furthest unlocked level, for instance)
	public int GetFurthestUnlockedLevel() => ConfirmDataLoaded() ? _currentSaveData.furthestUnlockedLevel : 1;
	public string GetEquippedWeapon() => ConfirmDataLoaded() ? _currentSaveData.equippedWeapon : "";
	public PlayerCurrency GetCurrency() => ConfirmDataLoaded() ? _currentSaveData.currency : new PlayerCurrency();
	public List<WeaponPurchaseData> GetWeaponsPurchased() => ConfirmDataLoaded() ? _currentSaveData.weaponsPurchased : new List<WeaponPurchaseData>();
	public DateTime? GetSlotLastSavedTime(int whichSlot)
	{
		if (!ConfirmSlotTimesLoaded()) return null;
		if (_slotTimes == null && VERBOSE)
		{
			Debug.Log("Somehow, _slotTimes is null after ConfirmSlotTimesLoaded.");
			return null;
		}
		return whichSlot switch
		{
			1 => new DateTime(_slotTimes.slotOneLastSaveTime),
			2 => new DateTime(_slotTimes.slotTwoLastSaveTime),
			3 => new DateTime(_slotTimes.slotThreeLastSaveTime),
			_ => null
		};
	}
	#endregion

	#region coroutines

	IEnumerator DelayedEmit(Action action, float delay)
	{
		yield return new WaitForSeconds(delay);
		action?.Invoke();
	}

	#endregion
}