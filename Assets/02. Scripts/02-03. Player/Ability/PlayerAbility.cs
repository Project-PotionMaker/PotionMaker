using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    protected Player _owner { get; private set; }
    
    protected virtual void Awake()
    {
        _owner = GetComponentInParent<Player>();
    }
}
