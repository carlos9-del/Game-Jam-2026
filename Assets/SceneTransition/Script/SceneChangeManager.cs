using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// =============================================
// Scene change manager / シーン遷移管理クラス
// =============================================
// EN: Handles all scene transitions. Two ways to change scenes:
//     - SceneChange(sceneName): loads the target scene directly, no loading screen.
//     - SceneChangeWithLoading(sceneName, stageData): routes through the "Loading" scene first,
//       optionally passing StageData so the loading screen can show stage-specific visuals.
// JP: 全てのシーン遷移を管理する。シーン遷移には2つの方法がある。
//     - SceneChange(sceneName): ローディング画面を挟まず、直接目的のシーンへ遷移する。
//     - SceneChangeWithLoading(sceneName, stageData): 「Loading」シーンを経由して遷移する。
//       StageDataを渡すことで、ローディング画面にステージ固有の情報を表示できる。
public class SceneChangeManager : SingletonMonoBehaviour<SceneChangeManager>
{
    private string nextScene;              // EN: name of the scene to transition to / JP: 遷移するシーンの名前
    private StageData nextStageData;       // EN: stage data for the loading screen (may be null) / JP: ローディング画面用のステージデータ（nullの場合あり）
    private bool isSceneChanging = false;  // EN: whether a scene transition is currently in progress / JP: シーン遷移中かどうか

    // EN: Gets the name of the scene currently being transitioned to. Used by SceneLoading.
    // JP: 現在遷移中のシーンの名前を取得する。SceneLoadingから呼び出される。
    public string GetNextScene()
    {
        return nextScene;
    }

    // EN: Gets the StageData tied to the current transition, if any. Used by SceneLoading.
    // JP: 現在の遷移に紐づくStageDataを取得する（存在する場合）。SceneLoadingから呼び出される。
    public StageData GetNextStageData()
    {
        return nextStageData;
    }

    // EN: Whether a scene transition is currently happening.
    // JP: シーン遷移中かどうかを取得する。
    public bool GetIsSceneChanging()
    {
        return isSceneChanging;
    }

    // EN: Checks whether the given scene name actually exists in Build Settings.
    //     Prevents typos from causing silent failures.
    // JP: 指定されたシーン名がBuild Settingsに実際に存在しているかを確認する。
    //     タイプミスによる失敗を防ぐ。
    private bool IsNextSceneExists(string targetScene)
    {
        if (string.IsNullOrWhiteSpace(targetScene))
        {
            Debug.LogError("Target scene name is empty. / 遷移するシーンが入力されていません。");
            return false;
        }

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i); // EN: get scene path / JP: シーンのパス取得
            string name = Path.GetFileNameWithoutExtension(path);   // EN: get scene name / JP: シーンの名前取得
            if (name == targetScene)
            {
                return true;
            }
        }

        Debug.LogError(targetScene + " does not exist in Build Settings. / " + targetScene + "は存在しないシーンです。");
        return false;
    }

    // EN: Loads the target scene directly, with no loading screen in between.
    //     Use this for quick transitions like Title -> Result, or button-based menu navigation.
    // JP: ローディング画面を挟まず、目的のシーンへ直接遷移する。
    //     Title -> Resultのような軽い遷移や、メニュー内のボタン遷移に使用する。
    public void SceneChange(string targetScene)
    {
        if (isSceneChanging) return; // EN: ignore if already changing scenes / JP: 既に遷移中の場合は無視する

        bool canChange = IsNextSceneExists(targetScene);
        if (canChange)
        {
            nextScene = targetScene;
            nextStageData = null;
            StartCoroutine(DirectSceneChangeRoutine());
        }
        else
        {
            Debug.LogError("Failed to change to " + targetScene + ". / " + targetScene + "への遷移は失敗しました。");
        }
    }

    // EN: Routes the transition through the "Loading" scene first.
    //     Use this for heavier transitions, e.g. entering a stage.
    //     stageData is optional - pass null if the loading screen doesn't need stage-specific visuals.
    // JP: 「Loading」シーンを経由して遷移する。
    //     ステージに入る時のような、比較的重い遷移で使用する。
    //     stageDataは任意 - ローディング画面にステージ固有の情報が不要な場合はnullを渡す。
    public void SceneChangeWithLoading(string targetScene, StageData stageData = null)
    {
        if (isSceneChanging) return; // EN: ignore if already changing scenes / JP: 既に遷移中の場合は無視する

        bool canChange = IsNextSceneExists(targetScene);
        if (canChange)
        {
            nextScene = targetScene;
            nextStageData = stageData;
            StartCoroutine(LoadingSceneChangeRoutine());
        }
        else
        {
            Debug.LogError("Failed to change to " + targetScene + ". / " + targetScene + "への遷移は失敗しました。");
        }
    }

    // EN: Loads the target scene directly (no Loading scene involved).
    // JP: 目的のシーンへ直接遷移する（Loadingシーンを経由しない）。
    private IEnumerator DirectSceneChangeRoutine()
    {
        isSceneChanging = true;
        yield return SceneManager.LoadSceneAsync(nextScene);
        OnSceneChangeFinished();
    }

    // EN: Loads the "Loading" scene, which will handle loading the real target scene.
    // JP: 「Loading」シーンへ遷移する。実際の目的シーンの読み込みはLoadingシーン側で行う。
    private IEnumerator LoadingSceneChangeRoutine()
    {
        isSceneChanging = true;
        yield return SceneManager.LoadSceneAsync("Loading");
        // EN: SceneLoading (inside the Loading scene) will call FinishLoadingTransition() once done.
        // JP: 以降の処理は、Loadingシーン内のSceneLoadingがFinishLoadingTransition()を呼び出して行う。
    }

    // EN: Called by SceneLoading once it has finished loading the real target scene and activated it.
    // JP: SceneLoadingが目的のシーンの読み込みとアクティブ化を完了した際に呼び出される。
    public void FinishLoadingTransition()
    {
        OnSceneChangeFinished();
    }

    // EN: Common cleanup run whenever a scene transition completes, regardless of route taken.
    // JP: 遷移方法にかかわらず、シーン遷移が完了した際に共通して実行される後処理。
    private void OnSceneChangeFinished()
    {
        Debug.Log("Arrived at scene: " + nextScene + " / " + nextScene + "へ遷移しました。");

        // EN: Let UIManager rescan the newly loaded scene's UI hierarchy.
        // JP: 新しく読み込まれたシーンのUI階層を、UIManagerに再取得させる。
        UIManager.Instance.SetUIGroup();

        isSceneChanging = false;
        nextScene = null;
        nextStageData = null;
    }
}