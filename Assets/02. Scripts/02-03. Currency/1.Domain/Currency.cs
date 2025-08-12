using System;

[Serializable]
public class Currency
{
    private int _value = 0;
    public int Value => _value;

    public Currency(int value = 0)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException
                (
                nameof(value),
                value,
                $"{nameof(value)} must be zero or greater");
        }
        _value = value;
    }

    public Currency(CurrencyDTO currencyDto)
    {
        _value = currencyDto.Value;
    }

    public void SetCurrency(int value)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException
                (
                nameof(value),
                value,
                $"{nameof(value)} must be zero or greater");
        }
        _value = value;
    }
    public void AddCurrency(int valueToAdd)
    {
        if(valueToAdd <= 0)
        {
            throw new ArgumentOutOfRangeException
                (
                nameof(valueToAdd),
                valueToAdd,
                $"{nameof(valueToAdd)} must be greater than zero");
        }

        _value += valueToAdd;
    }

    public bool TrySubtractCurrency(int valueToSubtract)
    {
        if (valueToSubtract <= 0)
        {
            throw new ArgumentOutOfRangeException
                (
                nameof(valueToSubtract),
                valueToSubtract,
                $"{nameof(valueToSubtract)} must be greater than zero");
        }

        if ( _value < valueToSubtract)
        {
            return false;
        }
        _value -= valueToSubtract;
        return true;
    }

    public CurrencyDTO ToDTO()
    {
        return new CurrencyDTO(_value);
    }

}
