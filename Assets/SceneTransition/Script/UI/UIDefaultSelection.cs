using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

// =============================================
// Default UI selection / UIデフォルト選択クラス
// =============================================
// EN: Attach to any empty object in a scene with UI buttons (e.g. Title, Result).
//     Does NOT select anything when the scene starts - nothing is highlighted for
//     mouse-only players. The moment the player presses a navigation input (arrow keys,
//     gamepad stick/D-pad) for the first time, the default button becomes selected,
//     giving keyboard/gamepad navigation a starting point from then on.
// JP: UIボタンがあるシーン（例：Title、Result）の空オブジェクトにアタッチする。
//     シーン開始時には何も選択しない - マウスのみでプレイする場合、何もハイライトされない。
//     プレイヤーが初めてナビゲーション入力（矢印キー、ゲームパッドのスティック・D-pad）を
//     押した瞬間に、デフォルトのボタンが選択され、以降キーボード・ゲームパッドの
//     ナビゲーションの起点となる。
public class UIDefaultSelection : MonoBehaviour
{
    [Header("Button To Select On First Navigation Input")]
    [SerializeField] private GameObject defaultSelected;

    private bool hasSelectedOnce = false;

    private void Update()
    {
        if (hasSelectedOnce || defaultSelected == null) return;

        if (WasNavigationInputPressed())
        {
            EventSystem.current.SetSelectedGameObject(defaultSelected);
            hasSelectedOnce = true;
        }
    }

    // EN: Checks whether any navigation input (arrow keys, gamepad stick/D-pad) was pressed this frame.
    // JP: ナビゲーション入力（矢印キー、ゲームパッドのスティック・D-pad）がこのフレームで押されたかを確認する。
    private bool WasNavigationInputPressed()
    {
        bool arrowPressed = Keyboard.current != null && (
            Keyboard.current.upArrowKey.wasPressedThisFrame ||
            Keyboard.current.downArrowKey.wasPressedThisFrame ||
            Keyboard.current.leftArrowKey.wasPressedThisFrame ||
            Keyboard.current.rightArrowKey.wasPressedThisFrame);

        bool gamepadPressed = Gamepad.current != null && (
            Gamepad.current.dpad.up.wasPressedThisFrame ||
            Gamepad.current.dpad.down.wasPressedThisFrame ||
            Gamepad.current.dpad.left.wasPressedThisFrame ||
            Gamepad.current.dpad.right.wasPressedThisFrame ||
            Gamepad.current.leftStick.ReadValue().magnitude > 0.5f);

        return arrowPressed || gamepadPressed;
    }
}