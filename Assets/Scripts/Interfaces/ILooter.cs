using System;

// For entities that can hold and use currency
public interface ILooter : ICurrencyHoarder
{
    // Use a certain amount of currency. Does not check if you have enough money for it.
    private void UseCurrency(CurrencyType type, int amount = 1)
    {
        CollectCurrency(type, -Math.Abs(amount)); // abs ensures always negative here
    }
    void UseCurrency(CurrencyValues price);
    int GetCurrency(CurrencyType type);
    CurrencyValues GetSaveableCurrency();
}

public enum CurrencyType
{
    COMMON, RARE, MYTHIC
}