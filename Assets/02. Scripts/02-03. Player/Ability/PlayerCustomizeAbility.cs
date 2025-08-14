using System.Collections.Generic;
using UnityEngine;

public class PlayerCustomizeAbility : PlayerAbility
{
    [Header("캐릭터 외형 머티리얼")]
    [SerializeField]
    private Renderer _characterRenderer;
    [SerializeField]
    private List<Material> _colorMaterialList;

    protected override void Awake()
    {
        base.Awake();
        _owner.OnDataChanged += ChangeMaterial;
    }

    private void ChangeMaterial()
    {
        if (_characterRenderer == null)
        {
            Debug.LogWarning("캐릭터 렌더러가 없습니다.");
            return;
        }

        int playerIndex = _owner.PlayerOrderIndex;
        if (playerIndex < 0 || playerIndex >= _colorMaterialList.Count)
        {
            return;
        }

        _characterRenderer.material = _colorMaterialList[playerIndex];
    }
}
