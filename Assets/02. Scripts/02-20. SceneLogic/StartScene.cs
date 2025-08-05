using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScene : MonoBehaviour
{
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            MirrorNetworkManager.Instance.StartHost();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            MirrorNetworkManager.Instance.StartClient();
        }
    }
}
