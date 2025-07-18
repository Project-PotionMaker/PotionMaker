using Photon.Pun;
using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    protected Player _owner { get; private set; }
    protected PhotonView _photonView { get; private set; }
    
    protected virtual void Awake()
    {
        _owner = GetComponentInParent<Player>();
        _photonView = GetComponentInParent<PhotonView>();
    }
}
