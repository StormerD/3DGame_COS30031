using System;

// For entities that can hold and use currency
public interface ILooter : ICurrencyHoarder
{
    // Use a certain amount of currency. Does not check if you have enough money for it.
    private void UseCurrency(CurrencyType type, int amount = 1)
    {
        CollectCurrency(type, -Math.Abs(amount)); // abs ensures always negative here
    }
    // Use an amount of currency. Returns true if you have enough money for it, and false if not.
    public bool UseCurrency(PurchasePrice price)
    {
        if (GetCurrency(CurrencyType.COMMON) < price.common || GetCurrency(CurrencyType.RARE) < price.rare || GetCurrency(CurrencyType.MYTHIC) < price.mythic) return false;
        UseCurrency(CurrencyType.COMMON, price.common);
        UseCurrency(CurrencyType.RARE, price.rare);
        UseCurrency(CurrencyType.MYTHIC, price.mythic);
        return true;
    }
    int GetCurrency(CurrencyType type);
    PlayerCurrency GetSaveableCurrency();
}

public enum CurrencyType
{
    COMMON, RARE, MYTHIC
}