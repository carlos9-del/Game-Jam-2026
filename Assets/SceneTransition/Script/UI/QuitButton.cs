using UnityEngine;

// =============================================
// Quit button / ゲーム終了ボタンクラス
// =============================================
// EN: Attach to any button that should quit the application (e.g. a "Quit Game" button on Title).
//     Works together with ButtonAction, which handles hover/click visuals and SE.
// JP: アプリケーションを終了させるボタンにアタッチする（例：Titleの「ゲーム終了」ボタン）。
//     ホバー・クリック時の演出やSE再生を行うButtonActionと連携して動作する。
[RequireComponent(typeof(ButtonAction))]
public class QuitButton : MonoBehaviour
{
    private ButtonAction buttonAction;

    private void Awake()
    {
        buttonAction = GetComponent<ButtonAction>();
    }

    private void OnEnable()
    {
        buttonAction.onClick = HandleClick;
    }

    // EN: Quits the application. In the Unity Editor, this stops Play mode instead
    //     (so you can test it without building). In a real build, this quits normally.
    // JP: アプリケーションを終了する。Unityエディタ上では、代わりにPlayモードを停止する
    //     （ビルドせずにテストできるように）。実際のビルドでは、通常通り終了する。
    private void HandleClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
    }
}