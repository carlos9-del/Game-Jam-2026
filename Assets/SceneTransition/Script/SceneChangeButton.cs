using UnityEngine;

// =============================================
// Scene change button / シーン遷移ボタンクラス
// =============================================
// EN: Attach to any button that should trigger a scene transition.
//     Works together with ButtonAction, which handles hover/click visuals and SE.
//     If useLoadingScreen is checked, the transition routes through the "Loading" scene first.
// JP: シーン遷移を行うボタンにアタッチする。
//     ホバー・クリック時の演出やSE再生を行うButtonActionと連携して動作する。
//     useLoadingScreenにチェックが入っている場合、「Loading」シーンを経由して遷移する。
[RequireComponent(typeof(ButtonAction))]
public class SceneChangeButton : MonoBehaviour
{
    [Header("Target Scene Name (for any button that's just menu navigation)")]
    [SerializeField] private string targetScene;

    [Header("Stage Data (only for buttons that actually enter a stage)")]
    [SerializeField] private StageData targetStageData;

    [Header("Use Loading Screen For This Transition")]
    [SerializeField] private bool useLoadingScreen = false;

    private ButtonAction buttonAction;

    private void Awake()
    {
        buttonAction = GetComponent<ButtonAction>();
    }

    private void OnEnable()
    {
        // EN: Hook this button's click behavior into ButtonAction's callback.
        // JP: ButtonActionのコールバックに、このボタンのクリック処理を登録する。
        buttonAction.onClick = HandleClick;
    }

    // EN: Called when the button is clicked. Decides which transition method to use.
    // JP: ボタンがクリックされた時に呼び出される。どちらの遷移方法を使うかを判断する。
    private void HandleClick()
    {
        // EN: If StageData is assigned, use its scene name; otherwise fall back to the plain string field.
        // JP: StageDataが設定されている場合はそのシーン名を使用し、未設定の場合は文字列フィールドを使用する。
        string sceneToLoad = targetStageData != null ? targetStageData.sceneName : targetScene;

        if (useLoadingScreen)
        {
            SceneChangeManager.Instance.SceneChangeWithLoading(sceneToLoad, targetStageData);
        }
        else
        {
            SceneChangeManager.Instance.SceneChange(sceneToLoad);
        }
    }
}