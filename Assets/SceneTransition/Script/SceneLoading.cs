using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// =============================================
// Scene loading class / シーン読み込みクラス
// =============================================
// EN: Lives in the "Loading" scene. Reads the target scene (and optional StageData)
//     from SceneChangeManager, loads it asynchronously in the background, waits until
//     both the load itself and a minimum display time are done, then activates the new scene.
// JP: 「Loading」シーンに配置する。SceneChangeManagerから遷移先のシーン名（および任意のStageData）を取得し、
//     バックグラウンドで非同期に読み込みを行う。読み込み完了と最低表示時間の両方が満たされたら、
//     新しいシーンをアクティブにする。
public class SceneLoading : MonoBehaviour
{
    [Header("Background Image (optional, set from Stage Data)")]
    [SerializeField] private Image backgroundImage;

    [Header("Label Text (optional, set from Stage Data)")]
    [SerializeField] private Text labelText; // EN: swap for TMP_Text if using TextMeshPro / JP: TextMeshPro使用時はTMP_Textに変更する

    [Header("Minimum Time To Show This Screen (seconds)")]
    [SerializeField] private float minLoadTime = 0.5f;

    private void Start()
    {
        Time.timeScale = 1.0f; // EN: make sure gameplay isn't left paused from a previous scene / JP: 前のシーンで一時停止していた場合に解除する
        ApplyStageVisuals();
        StartCoroutine(LoadSceneRoutine());
    }

    // EN: If StageData was passed for this transition, use it to update the background/label.
    // JP: この遷移にStageDataが渡されている場合、それを使って背景・ラベルを更新する。
    private void ApplyStageVisuals()
    {
        StageData stageData = SceneChangeManager.Instance.GetNextStageData();
        if (stageData == null) return; // EN: no stage data provided, keep defaults / JP: StageDataが渡されていない場合、デフォルトのままにする

        if (backgroundImage != null && stageData.loadingBackground != null)
        {
            backgroundImage.sprite = stageData.loadingBackground;
        }

        if (labelText != null && !string.IsNullOrEmpty(stageData.displayName))
        {
            labelText.text = stageData.displayName;
        }
    }

    // EN: Loads the target scene in the background and activates it once ready.
    // JP: 目的のシーンをバックグラウンドで読み込み、準備ができ次第アクティブにする。
    private IEnumerator LoadSceneRoutine()
    {
        string nextScene = SceneChangeManager.Instance.GetNextScene(); // EN: get target scene name / JP: 遷移するシーンの名前を取得

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(nextScene);
        asyncOperation.allowSceneActivation = false; // EN: prevent auto-activation so we control the timing / JP: 読み込み完了後、勝手に遷移しないようにする

        float elapsedTime = 0.0f;

        while (!asyncOperation.isDone)
        {
            elapsedTime += Time.unscaledDeltaTime;

            // EN: Unity reports progress up to 0.9 while allowSceneActivation is false; 0.9 means "ready".
            // JP: allowSceneActivationがfalseの間、進行度は最大0.9までしか上がらない。0.9で「準備完了」を意味する。
            bool isLoadReady = asyncOperation.progress >= 0.9f;
            bool isMinTimeReached = elapsedTime >= minLoadTime;

            if (isLoadReady && isMinTimeReached)
            {
                Debug.Log("Finished loading scene: " + nextScene + " / シーン:" + nextScene + "のロードが完了しました。");
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }

        // EN: The new scene is now active - tell SceneChangeManager the transition is complete.
        // JP: 新しいシーンがアクティブになったので、SceneChangeManagerに遷移完了を伝える。
        SceneChangeManager.Instance.FinishLoadingTransition();
    }
}