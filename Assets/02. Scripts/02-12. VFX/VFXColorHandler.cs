using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class VFXColorHandler : NetworkBehaviour
{
    [Header("플레이어 별 핑 색깔")]
    [SerializeField]
    private List<Color> _pingColor;
    
    private ParticleSystem[] _particleSystems;

    private void Awake()
    {
        // 자신 + 모든 자식의 ParticleSystem 가져오기
        _particleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    [ClientRpc]
    public void RpcChangeVFXColor(int playerIndex)
    {
        if (playerIndex < 0 || playerIndex >= _pingColor.Count)
        {
            return;
        }

        foreach(ParticleSystem particleSystem in _particleSystems)
        {
            var main = particleSystem.main;
            main.startColor = _pingColor[playerIndex];
        }
    }
}
