using UnityEngine;

public class PlayerPickupAbility : PlayerAbility
{
    // 나중에 IPickable로 변경 가능
    private GameObject _heldItem = null;

    private void Start()
    {
        InputManager.Instance.OnPickupEvent += OnPickupInput;
    }

    private void OnPickupInput()
    {
        if (_heldItem == null)
        {
            TryPickup();
        }
        else
        {
            TryPutDown();
        }
    }

    private void TryPickup()
    {
        Debug.Log("Pickup");
        GameObject item = FindFrontPickupItem();
        if (item == null)
        {
            return;
        }

        _heldItem = item;
        _heldItem.transform.SetParent(this.transform);
    }

    private void TryPutDown()
    {
        Debug.Log("Put Down");
        _heldItem.transform.SetParent(null);
        _heldItem = null;
    }

    private GameObject FindFrontPickupItem()
    {
        // TODO : 플레이어가 있는 그리드에서 플레이어가 바라보는 방향의 한 칸 앞의 그리드에 있는 오브젝트 확인
        // 확인한 오브젝트가 들 수 있는 오브젝트인지 확인
        return null;
    }
}
