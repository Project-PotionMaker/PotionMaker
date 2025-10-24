using Mirror;
using UnityEngine;

public class PopupSyncronizer : NetworkBehaviourSingleton<PopupSyncronizer>
{

    [Command(requiresAuthority = false)]
    private void CmdRequestCloseAllPopups()
    {
        RpcCloseAllPopups();
    }

    [ClientRpc]
    private void RpcCloseAllPopups()
    {
        GameSceneUIManager.Instance.CloseAllPopups();
    }

    public void CloseAllPopupsSynced()
    {
        if (NetworkServer.active)
        {
            RpcCloseAllPopups();
        }
        else
        {
            CmdRequestCloseAllPopups();
        }
    }
}
