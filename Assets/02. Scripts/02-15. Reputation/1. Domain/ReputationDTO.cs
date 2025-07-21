using UnityEngine;

public class ReputationDTO
{
    public readonly int Value;

    public ReputationDTO(Reputation reputation)
    {
        Value = reputation.Value;
    }

    public ReputationDTO(int value)
    {
        Value = value;
    }
}
