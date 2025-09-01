using System;

[Serializable]
public class ReputationDTO
{
    public readonly float Value;
    public readonly float ValueYesterday;
    public readonly float Difference;
    public readonly EReputationGrade ReputationGrade;
    

    public ReputationDTO(Reputation reputation)
    {
        Value = reputation.Value;
        ValueYesterday = reputation.ValueYesterday;
        Difference = reputation.Difference;
        ReputationGrade = reputation.ReputationGrade;
    }
}
