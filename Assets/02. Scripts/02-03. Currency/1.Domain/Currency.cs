using System;

public class Currency
{
    private int _value = 0;
    public int Value => _value;

    public Currency(int value = 0)
    {
        _value = value;
    }

    public void SetCurrency(int value)
    {
        _value = value;
    }
    public void AddCurrency(int value)
    {
        if(value <= 0)
        {
            throw new System.Exception("Can't add zero or less");
        }

        _value += value;
    }

    public bool TrySubtractCurrency(int subtractedValue)
    {
        if(subtractedValue <= 0)
        {
            throw new System.Exception("Can't subtract zero or less");
        }

        if( _value < subtractedValue)
        {
            return false;
        }
        _value -= subtractedValue;
        return true;
    }

    public CurrencyDTO ToDTO()
    {
        return new CurrencyDTO(_value);
    }

}
