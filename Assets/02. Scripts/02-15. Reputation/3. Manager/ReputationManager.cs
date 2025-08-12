using Mirror;
using System;

public class ReputationManager : NetworkBehaviourSingleton<ReputationManager>, IShopInfoSaveable
{
    public event Action OnDataChanged;

    private Reputation _reputation;
    public ReputationDTO Reputation => _reputation.ToDTO();

    private ReputationRepository _reputationRepository;

    private const float _increaseAmountOnSuccessOrder = 0.2f;
    private const float _decreaseAmountOnFailOrder = 0.1f;

    public override void OnStartClient()
    {
        base.OnStartClient();
        InitReputationManager();
        PhaseManager.Instance.PhaseDictionary[EPhaseType.ServingPhase].OnPhaseExited += AddDailyReputation;
    }

    private void InitReputationManager()
    {
        if (!NetworkClient.ready)
        {
            return;
        }

        _reputationRepository = new ReputationRepository();
        _reputation = new Reputation(2.5f);

        CmdRequestUpdateReputation();
        OnDataChanged?.Invoke();
    }

    [Command(requiresAuthority = false)]
    public void CmdRequestAddReputation(float addedValue)
    {
        AddReputation(addedValue);
    }

    [Command(requiresAuthority = false)]
    public void CmdRequestSubtractReputation(float subtractedValue)
    {
        SubtractReputation(subtractedValue);
    }

    [Server]
    public void AddDailyReputation()
    {
        AddReputation();
    }

    [Server]
    public void AddReputation(float addedValue = _increaseAmountOnSuccessOrder)
    {
        if (!isServer)
        {
            throw new InvalidOperationException($"{nameof(AddReputation)}() is server-only. Use {nameof(CmdRequestAddReputation)}() from client.");
        }

        _reputation.AddReputation(addedValue);
        OnDataChanged?.Invoke();

        UpdateReputation(_reputation.Value);
    }

    [Server]
    public void SubtractReputation(float subtractedValue = _decreaseAmountOnFailOrder)
    {
        if (!isServer)
        {
            throw new InvalidOperationException($"{nameof(SubtractReputation)}() is server-only. Use {nameof(CmdRequestSubtractReputation)}() from client.");
        }

        bool result = _reputation.TrySubtractReputation(subtractedValue);
        if (result)
        {
            OnDataChanged?.Invoke();
            UpdateReputation(_reputation.Value);
            UnityEngine.Debug.Log("Reputation subtracted");
        }
        else
        {
            UnityEngine.Debug.Log("Failed to subtract reputation");
        }
    }

    [ClientRpc]
    public void UpdateReputation(float value)
    {
        _reputation.SetReputation(value);
        OnDataChanged?.Invoke();
    }

    [Command(requiresAuthority = false)]
    public void CmdRequestUpdateReputation()
    {
        UpdateReputation(_reputation.Value);
    }

    public void OnServingPhaseEnd()
    {
        _reputation.UpdateValueYesterday();
    }

    public void ApplyLoadedData(ShopInfo shopInfo)
    {
        _reputation = shopInfo.Reputation;
    }

    public void ProvideSaveData(ShopInfo shopInfo)
    {
        shopInfo.Reputation = _reputation;
    }
}
