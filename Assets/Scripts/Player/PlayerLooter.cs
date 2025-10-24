using System;
using UnityEngine;

[RequireComponent(typeof(PlayerDataTracker))]
public class PlayerLooter : MonoBehaviour, ILooter
{
    private int _currencyCommon;
    private int _currencyRare;
    private int _currencyMythic;
    public event Action<CurrencyType, int> CurrencyChanged;

    void Start()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.OnLoadComplete += SetCurrency;
        }
    }

    public void CollectCurrency(CurrencyType type, int amount = 1)
    {
        switch (type)
        {
            case CurrencyType.COMMON:
                _currencyCommon += amount;
                CurrencyChanged?.Invoke(type, _currencyCommon);
                break;
            case CurrencyType.RARE:
                _currencyRare += amount;
                CurrencyChanged?.Invoke(type, _currencyRare);
                break;
            case CurrencyType.MYTHIC:
                _currencyMythic += amount;
                CurrencyChanged?.Invoke(type, _currencyMythic);
                break;
        }

    }

    public int GetCurrency(CurrencyType type)
    {
        return type switch
        {
            CurrencyType.COMMON => _currencyCommon,
            CurrencyType.RARE => _currencyRare,
            CurrencyType.MYTHIC => _currencyMythic,
            _ => 0,
        };
    }

    public PlayerCurrency GetSaveableCurrency() => new() { common = _currencyCommon, rare = _currencyRare, mythic = _currencyMythic };

    public void SetCurrency()
    {
        PlayerCurrency pc = GameManager.instance.GetCurrency();
        _currencyCommon = pc.common; _currencyRare = pc.rare; _currencyMythic = pc.mythic;
    }
}
