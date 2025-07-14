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
    public void AddCurrency(int addendValue)
    {
        if(addendValue <= 0)
        {
            throw new System.Exception("Can't add zero or less");
        }

        _value += addendValue;
    }

    public bool TrySubtractCurrency(int subtrahendValue)
    {
        if(subtrahendValue <= 0)
        {
            throw new System.Exception("Can't subtract zero or less");
        }

        if( _value < subtrahendValue)
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
