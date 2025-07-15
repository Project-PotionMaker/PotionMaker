using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Machine : MonoBehaviour
{
    private MachineData _data;
    public MachineData Data { get => _data; private set => _data = value; }

    private float _currentProgress;

    private bool _isStarted;

    private List<int> InputTIDList;

    public bool TryInput(int tid, EInputType inputType)
    {
        if (InputTIDList.Count >= _data.MaxInputCount || _isStarted || InputTIDList.Contains(tid))
        {
            return false;
        }
        return true;
    }

    // MachineManager에서 플레이어의 입력을 받고 작업 수행
    public bool TryProgress()
    {
        if(InputTIDList.Count != _data.MaxInputCount)
        {
            return false;
        }


        return true;
    }

    //private IEnumerator Progress_Coroutine()
    //{

    //}

    // 마지막 출력물은 OutputManager.Instance.GetOutput(int machineTID)로 여기의 TID를 주면 Output DataSheet에서 출력물 가져오기
    // 아직 시트에 이 내용은 없습니다.
}
