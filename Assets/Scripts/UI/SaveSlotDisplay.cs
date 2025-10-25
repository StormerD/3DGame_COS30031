using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SaveSlotDisplay : MonoBehaviour, IPointerClickHandler
{
    public int slot;
    public TMP_Text savetimeDisplay;
    private MainMenuHandler menuManager;

    void Start()
    {
        menuManager = GetComponentInParent<MainMenuHandler>();
        GameManager.instance.OnSlotTimesUpdated += UpdateTimeText;
        UpdateTimeText();
    }

    void UpdateTimeText()
    {
        DateTime? time = GameManager.instance.GetSlotLastSavedTime(slot);
        savetimeDisplay.text = time == null || time?.Ticks == 0 ? "Never saved!" : time?.ToString("F") ?? "Never saved!";
    }

    public void OnPointerClick(PointerEventData ped)
    {
        menuManager.StartGame(slot);
    }
}
