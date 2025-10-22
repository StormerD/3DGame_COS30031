#pragma warning disable CS0162

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// This manager is based on the Saving system provided in the modules, and uses a lot of the code from that. 
/// Instead of saving player position, it saves:
/// 1. Currency (points)
/// 2. Furthest unlocked level
/// 3. Weapons that have been purchased or unlocked
/// 4. Current player-equipped weapon
/// 5. slot index
/// 6. (IN PROGRESS) Player position
/// 7. (IN PROGRESS) Dialogue played
/// Technically there can be infinite save slots, but the menu only allows 3. Another file saves metadata about the
/// saves themselves (currently only when last saved) but not any game-related data. That way to display
/// when the slots were most recently saved you don't have to reload the entire slot. Not super important in this game
/// since the save data is small anyway, but I think it's a good idea in case we add more save data in the future
/// </summary>
public class GameManager : MonoBehaviour
{
	public static GameManager instance;
	public event Action OnSaveStart;
	public event Action OnSaveComplete;
	public event Action OnLoadStart;
	public event Action OnLoadComplete;
	public event Action OnSlotTimesUpdated;
	public event Action OnPlayerStartPositionChanged;
	public int slotIndex = 0;     // set in Inspector to test different slots
	public const int LEVEL_AMOUNT = 4;

	private HashSet<string> _dialoguesPlayedSinceLastSave = new();
	private SaveData _currentSaveData;
	private SlotTimesData _slotTimes;
	private event Action OnActivePlayerDataChanged;
	private PlayerDataTracker _activePlayerData;
	private PlayerDataTracker _previousPlayerData;
	private int _currentLevel = -1;
	private Vector3 _spawnPosition = Vector3.zero;

	private const bool VERBOSE = false;

	#region Unity Functions
	void Awake()
	{
		if (instance != null && instance != this) Destroy(gameObject);
		else
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
			SceneManager.activeSceneChanged += OnActiveSceneChanged;
		}
	}

	private void OnActiveSceneChanged(Scene current, Scene next)
	{
		LevelInformation levelInformation = FindFirstObjectByType<LevelInformation>();
		if (levelInformation == null) { Debug.LogWarning("Could not find level information for new scene."); return; }

		if (_currentLevel == -1) { _currentLevel = levelInformation.levelNumber; }
		else
		{
			_activePlayerData.transform.position = levelInformation.GetSpawnPositionComingFrom(_currentLevel);
			_currentLevel = levelInformation.levelNumber;
		}
	}

	public void RegisterPlayer(PlayerDataTracker newPlayer)
	{
		if (VERBOSE) Debug.Log("New player registering");
		if (_activePlayerData != null)
		{
			// new player has higher registration; disable the active one and set it to previous
			if (_activePlayerData.registrationPriority < newPlayer.registrationPriority)
			{
				Debug.Log("Registering player has higher prio then current; disabling current and setting new to active");
				if (_previousPlayerData != null) Destroy(_previousPlayerData.gameObject);
				_previousPlayerData = _activePlayerData; _previousPlayerData.gameObject.SetActive(false);
				_activePlayerData = newPlayer;
			}
			else
			{
				Debug.Log("Registering player has lower priority than active; disabling registering and setting to previous");
				newPlayer.gameObject.SetActive(false);
				if (_previousPlayerData != null) Destroy(_previousPlayerData.gameObject);
				_previousPlayerData = newPlayer;
			}
		}
		else _activePlayerData = newPlayer;

		if (Camera.main.gameObject.TryGetComponent<CameraFollow>(out var follow)) { follow.SetPlayer(_activePlayerData.transform); }
	}

	public void UnregisterPlayer(PlayerDataTracker player)
    {
		if (_activePlayerData == null) Debug.LogWarning("Unregistering player while activeplayer is null?");
		else if (_activePlayerData.gameObject == player.gameObject) // re-activate previous if it exists
		{
			_activePlayerData = _previousPlayerData;
			if (_activePlayerData != null) _activePlayerData.gameObject.SetActive(true);
		}
		else if (_previousPlayerData.gameObject == player.gameObject) _previousPlayerData = null;
    }
	
	public void CurrentLevelComplete()
    {
		if (_currentLevel < 1 || _currentLevel > LEVEL_AMOUNT) Debug.LogWarning("GameManager does not know the current level number; it currently has it set as: " + _currentLevel);
		// todo assorted things
    }
	
	#endregion

	#region Saving
	public void Save(int to) => DoSave(to, GetFurthestUnlockedLevel());
	public void SafeSave() => DoSave(slotIndex, GetFurthestUnlockedLevel());
	void DoSave(int sIndex, int furthestUnlockedLevel)
	{
		OnSaveStart?.Invoke();

		_dialoguesPlayedSinceLastSave.AddRange(_currentSaveData.dialoguesPlayed);
		var saveData = new SaveData
		{
			slotIndex = sIndex,
			furthestUnlockedLevel = furthestUnlockedLevel,
			currency = _activePlayerData.GetSaveableCurrency(),
			equippedWeapon = _activePlayerData.GetEquippedWeapon(),
			weaponsPurchased = ForgeManager.instance != null ? ForgeManager.instance.GetWeaponPurchaseData() : _currentSaveData.weaponsPurchased,
			dialoguesPlayed = _dialoguesPlayedSinceLastSave.ToList()
		};
		SaveSystem.Save(saveData);
		_currentSaveData = saveData;
		_dialoguesPlayedSinceLastSave = new();

		Debug.Log($"[Save late] {GetInstanceID()} _currentSaveData: {_currentSaveData} (objRef: {(_currentSaveData != null ? _currentSaveData.GetHashCode() : 0)})");

		if (VERBOSE) Debug.Log($"Saved slot {sIndex}");
		SaveSlotTimes(sIndex);

		StartCoroutine(DelayedParamlessActionInvoke(OnSaveComplete, 0.1f));
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

	public void UpdateDialogueList(string with) { _dialoguesPlayedSinceLastSave.Add(with); }
	
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

		OnLoadStart?.Invoke();

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
				weaponsPurchased = null,
				dialoguesPlayed = new()
			};
			Debug.Log("Current save data: " + _currentSaveData);
		}
		StartCoroutine(DelayInvoke(1f));
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

	#endregion

	#region Getters
	// separate distinct pieces into individual getters so that components can get as needed (no reason for scene manager to have anything but furthest unlocked level, for instance)
	public int GetFurthestUnlockedLevel() => ConfirmDataLoaded() ? _currentSaveData.furthestUnlockedLevel : 1;
	public string GetEquippedWeapon() => ConfirmDataLoaded() ? _currentSaveData.equippedWeapon : "";
	public PlayerCurrency GetCurrency() => ConfirmDataLoaded() ? _currentSaveData.currency : new PlayerCurrency();
	public List<WeaponPurchaseData> GetWeaponsPurchased() => ConfirmDataLoaded() ? _currentSaveData.weaponsPurchased : null;
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
	public bool GetDialogueHasBeenPlayed(string which)
	{
		if (ConfirmDataLoaded()) return _dialoguesPlayedSinceLastSave.Contains(which) || _currentSaveData.dialoguesPlayed.Contains(which);
		else return _dialoguesPlayedSinceLastSave.Contains(which);
	}
	public bool GetGameComplete() => _currentSaveData?.furthestUnlockedLevel > LEVEL_AMOUNT;
	#endregion

	IEnumerator DelayedParamlessActionInvoke(Action action, float delay = 0.1f)
	{
		yield return new WaitForSeconds(delay);
		action?.Invoke();
	}
	
	IEnumerator DelayInvoke(float delay = 0.1f)
    {
		yield return new WaitForSeconds(delay);
		OnLoadComplete?.Invoke();
    }
}