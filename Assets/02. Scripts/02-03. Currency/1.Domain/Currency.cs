using System;

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
    public void AddCurrency(int addendValue)
    {
        if(addendValue <= 0)
        {
            throw new ArgumentOutOfRangeException
                (
                nameof(addendValue),
                addendValue,
                $"{nameof(addendValue)} must be greater than zero");
        }

        _value += addendValue;
    }

    public bool TrySubtractCurrency(int subtrahendValue)
    {
        if (subtrahendValue <= 0)
        {
            throw new ArgumentOutOfRangeException
                (
                nameof(subtrahendValue),
                subtrahendValue,
                $"{nameof(subtrahendValue)} must be greater than zero");
        }

        if ( _value < subtrahendValue)
        {
            return false;
        }
        _value -= subtrahendValue;
        return true;
    }

    public CurrencyDTO ToDTO()
    {
        return new CurrencyDTO(_value);
    }

}
