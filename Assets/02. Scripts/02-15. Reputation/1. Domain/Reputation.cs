using System;
using UnityEngine;

public class Reputation
{
    private int _value = 0;
    public int Value => _value;

    public Reputation(int value = 0)
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

    public void SetReputation(int value)
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

    public void AddReputation(int valueToAdd)
    {
        if (valueToAdd <= 0)
        {
            throw new ArgumentOutOfRangeException
                (
                nameof(valueToAdd),
                valueToAdd,
                $"{nameof(valueToAdd)} must be greater than zero");
        }

        _value += valueToAdd;
    }

    public bool TrySubtractReputation(int valueToSubtract)
    {
        if (valueToSubtract <= 0)
        {
            throw new ArgumentOutOfRangeException
                (
                nameof(valueToSubtract),
                valueToSubtract,
                $"{nameof(valueToSubtract)} must be greater than zero");
        }

        if (_value < valueToSubtract)
        {
            return false;
        }
        _value -= valueToSubtract;
        return true;
    }

    public ReputationDTO ToDTO()
    {
        return new ReputationDTO(_value);
    }
}

