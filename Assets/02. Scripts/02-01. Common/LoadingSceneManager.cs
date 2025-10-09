using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneManager : MonoBehaviour
{
    [SerializeField]
    private Slider loadingBar;

    private void Start()
    {
        // 로딩 씬이 로드되면 게임 씬 로딩 코루틴을 시작합니다.
        StartCoroutine(Coroutine_LoadGameScene());
    }

    private IEnumerator Coroutine_LoadGameScene()
    {
        // NetworkRoomManager의 GameplayScene 이름을 가져옵니다.
        string gameplayScene = MirrorNetworkManager.Instance.GameplayScene;

        loadingBar.gameObject.SetActive(true);

        // 게임 씬을 비동기로 로드합니다.
        AsyncOperation op = SceneManager.LoadSceneAsync(gameplayScene, LoadSceneMode.Additive);

        // 로딩 진행도가 0.0에서 0.9 사이의 값을 가질 때까지 로딩바를 업데이트합니다.
        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);
            loadingBar.value = progress;
            yield return null;
        }

        loadingBar.value = 1f;

        // 로딩이 완료되었으므로, 로딩 씬은 언로드합니다.
        SceneManager.UnloadSceneAsync("LoadingScene");
    }
}