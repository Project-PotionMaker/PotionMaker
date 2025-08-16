using Mirror;

public class AudioNetworkManager : NetworkBehaviourSingleton<AudioNetworkManager>
{
    [Command(requiresAuthority = false)]
    public void CmdPlaySFX(EPlayerAudioType audioType)
    {
        RpcPlaySFX(audioType);
    }
    [Command(requiresAuthority = false)]
    public void CmdPlaySFX(EMachineAudioType audioType)
    {
        RpcPlaySFX(audioType);
    }
    [Command(requiresAuthority = false)]
    public void CmdPlaySFX(EStorageAudioType audioType)
    {
        RpcPlaySFX(audioType);
    }
    [Command(requiresAuthority = false)]
    public void CmdPlaySFX(ECustomerAudioType audioType)
    {
        RpcPlaySFX(audioType);
    }


    [ClientRpc]
    private void RpcPlaySFX (EPlayerAudioType audioType)
    {
        AudioManager.Instance.PlaySFX(audioType);
    }
    [ClientRpc]
    private void RpcPlaySFX(EMachineAudioType audioType)
    {
        AudioManager.Instance.PlaySFX(audioType);
    }
    [ClientRpc]
    private void RpcPlaySFX(EStorageAudioType audioType)
    {
        AudioManager.Instance.PlaySFX(audioType);
    }
    [ClientRpc]
    private void RpcPlaySFX(ECustomerAudioType audioType)
    {
        AudioManager.Instance.PlaySFX(audioType);
    }
}
