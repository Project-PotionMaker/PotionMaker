using Photon.Pun;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class BaseFactory : MonoBehaviourPun
{
    public async Task<GameObject> RequestCreateAsync(string addressableKey)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            return await CreateAsync(addressableKey);
        }
        else
        {
            // 마스터 클라이언트에게 생성 요청
            photonView.RPC(nameof(CreateRPC), RpcTarget.MasterClient, addressableKey);
            return null;
        }
    }

    [PunRPC]
    private async void CreateRPC(string addressableKey)
    {
        await CreateAsync(addressableKey);
    }

    private async Task<GameObject> CreateAsync(string addressableKey)

    {
        var handle = Addressables.LoadAssetAsync<GameObject>(addressableKey);
        GameObject prefab = await handle.Task;
        Debug.Log(prefab.name);

        if (prefab != null)
        {
            GameObject instance = PhotonNetwork.Instantiate(prefab.name, Vector3.zero, Quaternion.identity);
            return instance;
        }


        Debug.LogError($"프리팹이 없습니다. 키: {addressableKey}");
        return null;
    }
}
