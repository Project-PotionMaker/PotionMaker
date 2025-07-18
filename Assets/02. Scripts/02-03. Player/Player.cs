using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private PlayerStat _stat;
    public PlayerStat Stat => _stat;

    private Dictionary<Type, PlayerAbility> _abilityMap = new Dictionary<Type, PlayerAbility>();

    private void Update()
    {
        if (CheckObjectInFront())
        {
            // TODO : GridManager를 통해 오브젝트 있으면 빛나게 활성화 작업 추가
        }
    }

    public bool CheckObjectInFront()
    {
        Vector3 targetPosition = GetFrontPosition();

        return GridManager.Instance.CheckObjectOnGrid(targetPosition);
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
        ability = GetComponent<T>();

        if (ability != null)
        {
            _abilityMap[ability.GetType()] = ability;

            return ability as T;
        }

        throw new Exception($"어빌리티 {type.Name}을 {gameObject.name}에서 찾을 수 없습니다.");
    }
}
