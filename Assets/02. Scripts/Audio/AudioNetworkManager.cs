using Mirror;

public enum EAudioGroupType
{
    Player,
    Machine,
    Storage,
    Customer
}

public class AudioNetworkManager : NetworkBehaviourSingleton<AudioNetworkManager>
{
    [Command]
    public void CmdPlaySFX(EAudioGroupType audioGroupType, int audioSubType)
    {
        RpcPlaySFX(audioGroupType, audioSubType);
    }

    [ClientRpc]
    private void RpcPlaySFX (EAudioGroupType audioGroupType, int audioSubType)
    {
        switch (audioGroupType)
        {
            case EAudioGroupType.Player:
            {
                AudioManager.Instance.PlaySFX((EPlayerAudioType)audioSubType);
                break;
            }
            case EAudioGroupType.Machine:
            {
                AudioManager.Instance.PlaySFX((EMachineAudioType)audioSubType);
                break;
            }
            case EAudioGroupType.Storage:
            {
                AudioManager.Instance.PlaySFX((EStorageAudioType)audioSubType);
                break;
            }
            case EAudioGroupType.Customer:
            {
                AudioManager.Instance.PlaySFX((ECustomerAudioType)audioSubType);
                break;
            }
        }
    }
}
