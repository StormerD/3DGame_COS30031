using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "PurchaseEventObject", menuName = "EventObjects/Purchase Event")]
public class PurchaseEventObject : ScriptableObject
{
    private event UnityAction<CurrencyValues> OnEventRaised;

    public void RaiseEvent(CurrencyValues currency) => OnEventRaised?.Invoke(currency);

    public void RegisterListener(UnityAction<CurrencyValues> listener) => OnEventRaised += listener;
    public void UnregisterListener(UnityAction<CurrencyValues> listener) => OnEventRaised -= listener;
}
