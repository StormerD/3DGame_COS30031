
// For entities that can hold currency.
public interface ICurrencyHoarder
{
    public void CollectCurrency(CurrencyType type, int amount = 1);
}