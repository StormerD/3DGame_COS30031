using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SaveSlotDisplay : MonoBehaviour, IPointerClickHandler
{
    public int slot;
    public TMP_Text savetimeDisplay;
    private MainMenuManager menuManager;

    void Start()
    {
        menuManager = GetComponentInParent<MainMenuManager>();
        SaveManager.instance.OnSlotTimesUpdated += UpdateTimeText;
        UpdateTimeText();
    }

    void UpdateTimeText()
    {
        DateTime? time = SaveManager.instance.GetSlotLastSavedTime(slot);
        savetimeDisplay.text = time == null || time?.Ticks == 0 ? "Never saved!" : time?.ToString("F") ?? "Never saved!";
    }

    public void OnPointerClick(PointerEventData ped)
    {
        menuManager.StartGame(slot);
    }
}
