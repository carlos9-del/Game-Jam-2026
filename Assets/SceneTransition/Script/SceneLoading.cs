using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// =============================================
// Scene loading class / シーン読み込みクラス
// =============================================
// EN: Lives in the "Loading" scene. Reads the target scene from SceneChangeManager,
//     loads it asynchronously in the background, waits until both the load itself and
//     the loading animation are done, then activates the new scene.
// JP: 「Loading」シーンに配置する。SceneChangeManagerから遷移先のシーン名を取得し、
//     バックグラウンドで非同期に読み込みを行う。読み込み完了とローディングアニメーションの
//     再生完了の両方が満たされたら、新しいシーンをアクティブにする。
public class SceneLoading : MonoBehaviour
{
    [Header("Loading Animation (waits for this to finish before transitioning)")]
    [SerializeField] private LoadingAnimation loadingAnimation;

    private void Start()
    {
        Time.timeScale = 1.0f; // EN: make sure gameplay isn't left paused from a previous scene / JP: 前のシーンで一時停止していた場合に解除する
        StartCoroutine(LoadSceneRoutine());
    }

    // EN: Loads the target scene in the background and activates it once ready.
    // JP: 目的のシーンをバックグラウンドで読み込み、準備ができ次第アクティブにする。
    private IEnumerator LoadSceneRoutine()
    {
        string nextScene = SceneChangeManager.Instance.GetNextScene(); // EN: get target scene name / JP: 遷移するシーンの名前を取得

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(nextScene);
        asyncOperation.allowSceneActivation = false; // EN: prevent auto-activation so we control the timing / JP: 読み込み完了後、勝手に遷移しないようにする

        while (!asyncOperation.isDone)
        {
            // EN: Unity reports progress up to 0.9 while allowSceneActivation is false; 0.9 means "ready".
            // JP: allowSceneActivationがfalseの間、進行度は最大0.9までしか上がらない。0.9で「準備完了」を意味する。
            bool isLoadReady = asyncOperation.progress >= 0.9f;
            bool isAnimationDone = loadingAnimation == null || loadingAnimation.IsFinished;

            if (isLoadReady && isAnimationDone)
            {
                Debug.Log("Finished loading scene: " + nextScene + " / シーン:" + nextScene + "のロードが完了しました。");
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}