using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed = 10f;
    public float MoveSpeed => _moveSpeed;

    [SerializeField]
    private float _interactRate = 3f;
    public float InteractRate => _interactRate;

    private Dictionary<Type, PlayerAbility> _abilityMap = new Dictionary<Type, PlayerAbility>();

    public T GetAbility<T>() where T : PlayerAbility
    {
        var type = typeof(T);

        if (_abilityMap.TryGetValue(type, out PlayerAbility ability))
        {
            return ability as T;
        }

        // 게이른 초기화/로딩 -> 처음에 곧바로 초기화/로딩을 하는게 아니라
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
