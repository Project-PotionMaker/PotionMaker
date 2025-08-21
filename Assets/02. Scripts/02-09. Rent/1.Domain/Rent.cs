using System;
using UnityEngine;

[Serializable]
public class Rent
{
    private const int RENT_PERIOD = 3;
    public int RentPeriod => RENT_PERIOD;

    private int _rentDayCounter;
    public int RentDayCounter => _rentDayCounter;

    private int _currentRentCost;
    public int CurrentRentCost => _currentRentCost;

    private int _rentIncrement;
    public int RentIncrement => _rentIncrement;

    public bool IsRentDay => _rentDayCounter == RENT_PERIOD;

    public Rent()
    {
        _rentDayCounter = 0;
        _currentRentCost = 0;
        _rentIncrement = 0;
    }

    public Rent(int rentDayCounter, int currentRentCost, int rentIncrement)
    {
        if (rentDayCounter < 1 || rentDayCounter > RENT_PERIOD)
        {
            throw new ArgumentOutOfRangeException
            (
                nameof(rentDayCounter),
                rentDayCounter,
                $"{nameof(rentDayCounter)} must be between 1 and {RENT_PERIOD}"
            );
        }
        if (currentRentCost < 0)
        {
            throw new ArgumentOutOfRangeException
            (
                nameof(currentRentCost),
                currentRentCost,
                $"{nameof(currentRentCost)} must be zero or greater"
            );
        }
        if (rentIncrement < 0)
        {
            throw new ArgumentOutOfRangeException
            (
                nameof(rentIncrement),
                rentIncrement,
                $"{nameof(rentIncrement)} must be zero or greater"
            );
        }
        _rentDayCounter = rentDayCounter;
        _currentRentCost = currentRentCost;
        _rentIncrement = rentIncrement;
    }

    public Rent(RentDTO rentDto)
    {
        _rentDayCounter = rentDto.RentDayCounter;
        _currentRentCost = rentDto.CurrentRentCost;
        _rentIncrement = rentDto.RentIncrement;
    }

    public void SetRent(int rentDayCounter, int currentRentCost, int rentIncrement)
    {
        if (rentDayCounter < 1 || rentDayCounter > RENT_PERIOD)
        {
            throw new ArgumentOutOfRangeException
            (
                nameof(rentDayCounter),
                rentDayCounter,
                $"{nameof(rentDayCounter)} must be between 1 and {RENT_PERIOD}"
            );
        }
        if (currentRentCost < 0)
        {
            throw new ArgumentOutOfRangeException
            (
                nameof(currentRentCost),
                currentRentCost,
                $"{nameof(currentRentCost)} must be zero or greater"
            );
        }
        if (rentIncrement < 0)
        {
            throw new ArgumentOutOfRangeException
            (
                nameof(rentIncrement),
                rentIncrement,
                $"{nameof(rentIncrement)} must be zero or greater"
            );
        }
        _rentDayCounter = rentDayCounter;
        _currentRentCost = currentRentCost;
        _rentIncrement = rentIncrement;
    }
    public void OnRentPaid()
    {
        _currentRentCost += _rentIncrement;
    }

    public void IncreaseRentDayCounter()
    {
        if (_rentDayCounter < RENT_PERIOD)
        {
            _rentDayCounter = (_rentDayCounter % RENT_PERIOD) + 1;
        }
        else
        {
            throw new InvalidOperationException("Cannot increase RentDayCounter beyond RENT_PERIOD.");
        }
    }

    public RentDTO ToDTO()
    {
        return new RentDTO(this);
    }
}
