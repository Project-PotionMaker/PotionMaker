using UnityEngine;
using VInspector.Libs;

public class CurrencyDTO
{
    public readonly int Value;

    public CurrencyDTO(Currency coin)
    {
        Value = coin.Value;
    }

    public CurrencyDTO(int value)
    {
        Value = value;
    }
}
