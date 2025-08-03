using Mirror;
using UnityEngine;

public class PlayerAbility : NetworkBehaviour
{
    protected Player _owner { get; private set; }
    
    protected virtual void Awake()
    {
        _owner = GetComponentInParent<Player>();
    }
}
