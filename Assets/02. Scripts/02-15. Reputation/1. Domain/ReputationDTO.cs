using UnityEngine;

public class ReputationDTO
{
    public readonly float Value;
    public readonly EReputationGrade ReputationGrade;

    public ReputationDTO(Reputation reputation)
    {
        Value = reputation.Value;
        ReputationGrade = reputation.ReputationGrade;
    }
}
