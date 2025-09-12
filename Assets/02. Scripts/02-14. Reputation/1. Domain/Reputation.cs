using System;
using UnityEngine;

public enum EReputationGrade
{
    VeryBad,
    Bad,
    Normal,
    Good,
    Excellent
}

[Serializable]
public class Reputation
{
    private float _value = 0;
    public float Value
    {
        get => _value;
        private set
        {
            _value = Mathf.Clamp(value, _minValue, _maxValue);
            UpdateReputationGrade();
            UpdateDifference();
        }
    }

    private float _valueYesterday = 0;
    public float ValueYesterday
    {
        get => _valueYesterday;
        private set
        {
            _valueYesterday = Mathf.Clamp(value, _minValue, _maxValue);
        }
    }

    private float _difference = 0;
    public float Difference
    {
        get => _difference;
        private set
        {
            _difference = Mathf.Clamp(value, -_maxValue, _maxValue);
        }
    }

    private EReputationGrade _reputationGrade = EReputationGrade.Normal;
    public EReputationGrade ReputationGrade => _reputationGrade;

    private const float _minValue = 0f;
    private const float _maxValue = 5f;

    public Reputation(float value = 0f)
    {
        if (value < 0f)
        {
            throw new ArgumentOutOfRangeException
                (
                nameof(value),
                value,
                $"{nameof(value)} must be zero or greater");
        }
        Value = value;
        ValueYesterday = value;
    }

    public Reputation(ReputationDTO reputationDto)
    {
        _value = reputationDto.Value;
        _valueYesterday = reputationDto.ValueYesterday;
        _difference = reputationDto.Difference;
        _reputationGrade = reputationDto.ReputationGrade;
    }

    public Reputation(float value, float valueYesterday, float difference, EReputationGrade grade)
    {
        _value = value;
        _valueYesterday = valueYesterday;
        _difference = difference;
        _reputationGrade = grade;
    }

    public void SetReputation(float value)
    {
        if (value < 0f)
        {
            throw new ArgumentOutOfRangeException
                (
                nameof(value),
                value,
                $"{nameof(value)} must be zero or greater");
        }
        Value = value;
    }

    public void AddReputation(float valueToAdd)
    {
        if (valueToAdd <= 0f)
        {
            throw new ArgumentOutOfRangeException
                (
                nameof(valueToAdd),
                valueToAdd,
                $"{nameof(valueToAdd)} must be greater than zero");
        }

        Value += valueToAdd;
    }

    public bool TrySubtractReputation(float valueToSubtract)
    {
        if (valueToSubtract <= 0f)
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
        Value -= valueToSubtract;
        return true;
    }

    public void UpdateValueYesterday()
    {
        ValueYesterday = _value;
    }

    public void UpdateDifference()
    {
        Difference = _value - _valueYesterday;
    }

    public void UpdateReputationGrade()
    {
        int enumIndex = Mathf.Clamp(Mathf.FloorToInt(_value), 0, 4);
        if (Enum.IsDefined(typeof(EReputationGrade), enumIndex))
        {
            _reputationGrade = (EReputationGrade)enumIndex;
        }
    }

    public ReputationDTO ToDTO()
    {
        return new ReputationDTO(this);
    }
}

