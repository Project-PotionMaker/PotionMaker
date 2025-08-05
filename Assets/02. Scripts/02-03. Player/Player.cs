using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Rendering;

public class Player : NetworkBehaviour
{
    [SerializeField]
    private PlayerStat _stat;
    public PlayerStat Stat => _stat;

    [Header("Ability 오브젝트")]
    [SerializeField]
    private GameObject _abilityObject;

    [Header("들기 위치")]
    [SerializeField]
    private Transform _heldPosition;
    public Transform HeldPosition => _heldPosition;

    private Dictionary<Type, PlayerAbility> _abilityMap = new Dictionary<Type, PlayerAbility>();

    // 영상에 넣을 임시 테스트
    private CanvasAlphaChanger _lastHighlightedStructure;

    private void Awake()
    {
    }

    private void Update()
    {
        if(isLocalPlayer == false)
        {
            return;
        }

        GameObject frontObject = GetObjectInFront();
        if (frontObject != null)
        {
            CanvasAlphaChanger currentStructure = frontObject.GetComponent<CanvasAlphaChanger>();
            if (_lastHighlightedStructure != null && _lastHighlightedStructure != currentStructure)
            {
                _lastHighlightedStructure.HideCanvas();
            }

            if (currentStructure != null)
            {
                currentStructure.ShowCanvas();
            }
            _lastHighlightedStructure = currentStructure;
        }
        else
        {
            if (_lastHighlightedStructure != null)
            {
                _lastHighlightedStructure.HideCanvas();
                _lastHighlightedStructure = null;
            }
        }
    }

    public GameObject GetObjectInFront()
    {
        Vector3 targetPosition = GetFrontPosition();

        if (GridManager.Instance != null)
        {
            return GridManager.Instance.GetObjectOnGrid(targetPosition);
        }
        return null;
    }

    public Vector3 GetFrontPosition()
    {
        return transform.position + transform.forward * _stat.FindOffset;
    }

    public T GetAbility<T>() where T : PlayerAbility
    {
        var type = typeof(T);

        if (_abilityMap.TryGetValue(type, out PlayerAbility ability))
        {
            return ability as T;
        }

        // 게으른 초기화/로딩 -> 처음에 곧바로 초기화/로딩을 하는게 아니라
        //                      필요할 때만 하는 뒤로 미루는 기법
        ability = _abilityObject.GetComponent<T>();

        if (ability != null)
        {
            _abilityMap[ability.GetType()] = ability;

            return ability as T;
        }

        throw new Exception($"어빌리티 {type.Name}을 {gameObject.name}에서 찾을 수 없습니다.");
    }
}
