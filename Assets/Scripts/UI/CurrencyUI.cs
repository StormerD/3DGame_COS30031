using UnityEngine;
using TMPro;

public class CurrencyUI : MonoBehaviour
{
    // _requestCurrencyStream is listened to by player currency - when it recieves a request, it emits on _playerCurrencyStream
    [SerializeField] private PurchaseEventObject _playerCurrencyStream;
    [SerializeField] private BasicEventObject _requestCurrencyStream;
    [SerializeField] private TextMeshProUGUI commonText;
    [SerializeField] private TextMeshProUGUI rareText;
    [SerializeField] private TextMeshProUGUI mythicText;

    private void OnEnable() => _playerCurrencyStream.RegisterListener(DisplayNewCurrency);
    private void OnDisable() =>  _playerCurrencyStream.UnregisterListener(DisplayNewCurrency);

    private void Start()
    {
        _requestCurrencyStream.RaiseEvent();
    }

    private void DisplayNewCurrency(CurrencyValues currencies)
    {
        Debug.Log("Recieved new currency: " + currencies);
        if (currencies == null) { Debug.LogWarning("currencies null"); return; } 
        commonText.text = currencies.common.ToString();
        rareText.text = currencies.rare.ToString();
        mythicText.text = currencies.mythic.ToString();
    }
}

