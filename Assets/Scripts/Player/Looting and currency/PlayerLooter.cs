using System;
using UnityEngine;

[RequireComponent(typeof(PlayerDataTracker))]
public class PlayerLooter : MonoBehaviour, ILooter
{
    [Tooltip("The stream that THIS OBJECT will share its currency on.")]
    [SerializeField] private PurchaseEventObject _shareCurrencyStream;
    [Tooltip("The stream that THIS OBJECT will listen to for requests to share its currency (to ShareCurrencyStream)")]
    [SerializeField] private BasicEventObject _requestShareCurrencyStream;
    [Tooltip("The stream that THIS OBJECT will listen to for costs.")]
    [SerializeField] private PurchaseEventObject _costsStream;
    private CurrencyValues _currencyValues;

    void OnEnable()
    {
        _requestShareCurrencyStream.RegisterListener(EmitCurrency);
        _costsStream.RegisterListener(UseCurrency);
    }
    void OnDisable()
    {
        _requestShareCurrencyStream.UnregisterListener(EmitCurrency);
        _costsStream.UnregisterListener(UseCurrency);
    }
    void EmitCurrency() => _shareCurrencyStream.RaiseEvent(_currencyValues);

    void Awake() => _currencyValues = new();

    void Start()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.OnLoadComplete += SetCurrency;
            SetCurrency();
        }
    }

    public void CollectCurrency(CurrencyType type, int amount = 1)
    {
        switch (type)
        {
            case CurrencyType.COMMON:
                _currencyValues.common += amount;
                break;
            case CurrencyType.RARE:
                _currencyValues.rare += amount;
                break;
            case CurrencyType.MYTHIC:
                _currencyValues.mythic += amount;
                break;
        }
        _shareCurrencyStream.RaiseEvent(_currencyValues);
    }

    public int GetCurrency(CurrencyType type)
    {
        return type switch
        {
            CurrencyType.COMMON => _currencyValues.common,
            CurrencyType.RARE => _currencyValues.rare,
            CurrencyType.MYTHIC => _currencyValues.mythic,
            _ => 0,
        };
    }
    public CurrencyValues GetSaveableCurrency() => _currencyValues ?? new();
    public void SetCurrency()
    {
        _currencyValues = GameManager.instance.GetCurrency();
        _shareCurrencyStream.RaiseEvent(_currencyValues);
    }
    public void UseCurrency(CurrencyValues cost)
    {
        Debug.Log("Using currency.");
        if (_currencyValues == null) { Debug.LogWarning("Trying to use currency, but _currencyValues is null."); return; }

        _currencyValues.common -= Mathf.Abs(cost.common);
        _currencyValues.rare -= Mathf.Abs(cost.rare);
        _currencyValues.mythic -= Mathf.Abs(cost.mythic);

        _shareCurrencyStream.RaiseEvent(_currencyValues);
    }
}
