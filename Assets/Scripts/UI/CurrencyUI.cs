using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;
using TMPro;

public class CurrencyUI : MonoBehaviour
{
    private PlayerLooter looter;
    [SerializeField] private TextMeshProUGUI commonText;
    [SerializeField] private TextMeshProUGUI rareText;
    [SerializeField] private TextMeshProUGUI mythicText;

    private void OnEnable()
    {
        looter = FindFirstObjectByType<PlayerLooter>();

        if (looter != null)
            looter.CurrencyChanged += UpdateCurrencyUI;
        if (looter != null)
            looter.CurrencyChanged += UpdateCurrencyUI;
    }

    private void OnDisable()
    {
        if (looter != null)
            looter.CurrencyChanged -= UpdateCurrencyUI;
    }

    private void Start()
    {
        PullCurrencyValuesDirectly();
        if (SaveManager.instance != null) SaveManager.instance.OnSaveDataChanged += PullCurrencyValuesDirectly;
    }

    private void PullCurrencyValuesDirectly()
    {
        commonText.text = looter.GetCurrency(CurrencyType.COMMON).ToString();
        rareText.text   = looter.GetCurrency(CurrencyType.RARE).ToString();
        mythicText.text = looter.GetCurrency(CurrencyType.MYTHIC).ToString();
    }

    private void UpdateCurrencyUI(CurrencyType type, int value)
    {
        switch (type)
        {
            case CurrencyType.COMMON:
                commonText.text = value.ToString();
                break;
            case CurrencyType.RARE:
                rareText.text = value.ToString();
                break;
            case CurrencyType.MYTHIC:
                mythicText.text = value.ToString();
                break;
        }
    }
}

