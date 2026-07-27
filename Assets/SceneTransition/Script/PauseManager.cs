using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

// =============================================
// Pause manager / ポーズ管理クラス
// =============================================
// EN: Lives in the Stage scene. Toggles the pause overlay on/off via Escape (keyboard)
//     or Start (gamepad), stops/resumes gameplay time, and stops/resumes BGM while paused.
//     Uses the new Input System package directly (Keyboard/Gamepad), no InputAction asset needed.
// JP: Stageシーンに配置する。Escapeキー（キーボード）またはStartボタン（ゲームパッド）で
//     ポーズ画面の表示・非表示を切り替え、ゲーム内時間とBGMを一時停止・再開する。
//     新しいInput Systemパッケージを直接使用しており（Keyboard/Gamepad）、
//     InputActionアセットの作成は不要。
public class PauseManager : MonoBehaviour
{
    [Header("Canvas Name (must match the canvas object's name under UI)")]
    [SerializeField] private string pauseCanvasName = "PauseCanvas";

    [Header("Button To Select When Pause Opens (for gamepad/keyboard navigation)")]
    [SerializeField] private GameObject defaultPauseSelection;

    private bool isPaused = false;

    private void Update()
    {
        if (WasPauseInputPressed())
        {
            TogglePause();
        }
    }

    // EN: Checks whether the pause input (Escape key or Gamepad Start button) was pressed this frame.
    // JP: ポーズ入力（Escapeキーまたはゲームパッドのスタートボタン）がこのフレームで押されたかを確認する。
    private bool WasPauseInputPressed()
    {
        bool escapePressed = Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
        bool startPressed = Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame;
        return escapePressed || startPressed;
    }

    // EN: Switches between paused and resumed states. / JP: ポーズ状態と再開状態を切り替える。
    public void TogglePause()
    {
        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    // EN: Enters the paused state - shows the pause canvas, freezes game time, stops BGM.
    // JP: ポーズ状態にする - ポーズキャンバスを表示し、ゲーム内時間を停止し、BGMを停止する。
    public void Pause()
    {
        isPaused = true;
        UIManager.Instance.Show(pauseCanvasName);
        Time.timeScale = 0.0f;
        SoundManager.Instance.PauseBGM();

        // EN: Select a default button so gamepad/keyboard navigation has a starting point.
        // JP: ゲームパッド・キーボードのナビゲーションの起点となるよう、デフォルトのボタンを選択する。
        if (defaultPauseSelection != null)
        {
            EventSystem.current.SetSelectedGameObject(defaultPauseSelection);
        }
    }

    // EN: Resumes gameplay - hides the pause canvas, resumes game time, resumes BGM.
    //     This is the method to call from the "Continue" button's OnClick event.
    // JP: ゲームを再開する - ポーズキャンバスを非表示にし、ゲーム内時間を再開し、BGMを再開する。
    //     「Continue」ボタンのOnClickイベントから呼び出すメソッド。
    public void Resume()
    {
        isPaused = false;
        UIManager.Instance.Hide(pauseCanvasName);
        Time.timeScale = 1.0f;
        SoundManager.Instance.ResumeBGM();

        // EN: Clear selection so nothing is left highlighted on the (now hidden) pause buttons.
        // JP: 非表示になったポーズ画面のボタンに選択状態が残らないよう、選択をクリアする。
        EventSystem.current.SetSelectedGameObject(null);
    }

    // EN: Goes back to the Title scene. Resets time scale first so Title doesn't start paused.
    //     Call this from the "Back to Title" button's OnClick event.
    // JP: Titleシーンに戻る。Titleがポーズ状態で始まらないよう、先にタイムスケールを戻す。
    //     「Back to Title」ボタンのOnClickイベントから呼び出す。
    public void BackToTitle()
    {
        Time.timeScale = 1.0f;
        SoundManager.Instance.StopBGM();
        SceneChangeManager.Instance.SceneChange("Title");
    }

    // EN: Quits the application. Call this from the "Quit" button's OnClick event.
    //     In the Unity Editor, this stops Play mode instead (so you can test it without building).
    //     In a real build, this quits the application normally.
    // JP: アプリケーションを終了する。「Quit」ボタンのOnClickイベントから呼び出す。
    //     Unityエディタ上では、代わりにPlayモードを停止する（ビルドせずにテストできるように）。
    //     実際のビルドでは、通常通りアプリケーションを終了する。
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
    }
}


//using UnityEngine;
//using UnityEngine.InputSystem;

//// =============================================
//// Pause manager / ポーズ管理クラス
//// =============================================
//// EN: Lives in the Stage scene. Toggles the pause overlay on/off via Escape (keyboard)
////     or Start (gamepad), stops/resumes gameplay time, and stops/resumes BGM while paused.
////     Uses the new Input System package directly (Keyboard/Gamepad), no InputAction asset needed.
//// JP: Stageシーンに配置する。Escapeキー（キーボード）またはStartボタン（ゲームパッド）で
////     ポーズ画面の表示・非表示を切り替え、ゲーム内時間とBGMを一時停止・再開する。
////     新しいInput Systemパッケージを直接使用しており（Keyboard/Gamepad）、
////     InputActionアセットの作成は不要。
//public class PauseManager : MonoBehaviour
//{
//    [Header("Canvas Name (must match the canvas object's name under UI)")]
//    [SerializeField] private string pauseCanvasName = "PauseCanvas";

//    private bool isPaused = false;

//    private void Update()
//    {
//        if (WasPauseInputPressed())
//        {
//            TogglePause();
//        }
//    }

//    // EN: Checks whether the pause input (Escape key or Gamepad Start button) was pressed this frame.
//    // JP: ポーズ入力（Escapeキーまたはゲームパッドのスタートボタン）がこのフレームで押されたかを確認する。
//    private bool WasPauseInputPressed()
//    {
//        bool escapePressed = Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
//        bool startPressed = Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame;
//        return escapePressed || startPressed;
//    }

//    // EN: Switches between paused and resumed states. / JP: ポーズ状態と再開状態を切り替える。
//    public void TogglePause()
//    {
//        if (isPaused)
//        {
//            Resume();
//        }
//        else
//        {
//            Pause();
//        }
//    }

//    // EN: Enters the paused state - shows the pause canvas, freezes game time, stops BGM.
//    // JP: ポーズ状態にする - ポーズキャンバスを表示し、ゲーム内時間を停止し、BGMを停止する。
//    public void Pause()
//    {
//        isPaused = true;
//        UIManager.Instance.Show(pauseCanvasName);
//        Time.timeScale = 0.0f;
//        SoundManager.Instance.PauseBGM();
//    }

//    // EN: Resumes gameplay - hides the pause canvas, resumes game time, resumes BGM.
//    //     This is the method to call from the "Continue" button's OnClick event.
//    // JP: ゲームを再開する - ポーズキャンバスを非表示にし、ゲーム内時間を再開し、BGMを再開する。
//    //     「Continue」ボタンのOnClickイベントから呼び出すメソッド。
//    public void Resume()
//    {
//        isPaused = false;
//        UIManager.Instance.Hide(pauseCanvasName);
//        Time.timeScale = 1.0f;
//        SoundManager.Instance.ResumeBGM();
//    }

//    // EN: Goes back to the Title scene. Resets time scale first so Title doesn't start paused.
//    //     Call this from the "Back to Title" button's OnClick event.
//    // JP: Titleシーンに戻る。Titleがポーズ状態で始まらないよう、先にタイムスケールを戻す。
//    //     「Back to Title」ボタンのOnClickイベントから呼び出す。
//    public void BackToTitle()
//    {
//        Time.timeScale = 1.0f;
//        SoundManager.Instance.StopBGM();
//        SceneChangeManager.Instance.SceneChange("Title");
//    }

//    // EN: Quits the application. Call this from the "Quit" button's OnClick event.
//    //     In the Unity Editor, this stops Play mode instead (so you can test it without building).
//    //     In a real build, this quits the application normally.
//    // JP: アプリケーションを終了する。「Quit」ボタンのOnClickイベントから呼び出す。
//    //     Unityエディタ上では、代わりにPlayモードを停止する（ビルドせずにテストできるように）。
//    //     実際のビルドでは、通常通りアプリケーションを終了する。
//    public void QuitGame()
//    {
//#if UNITY_EDITOR
//        UnityEditor.EditorApplication.isPlaying = false;
//#else
//		Application.Quit();
//#endif
//    }
//}