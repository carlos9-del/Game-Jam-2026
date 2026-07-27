using UnityEngine;

public class RingRotator : MonoBehaviour
{
    [Header("回転の設定")]
    public float rotateSpeed = 90f;     // 回転速度（度／秒）

    // どちらのボタンで回すか
    public enum ControlButton { L_Button, R_Button }
    public ControlButton controlButton = ControlButton.R_Button;

    // 回転方向（時計回り or 反時計回り）
    public enum RotateDirection { Clockwise, CounterClockwise }
    public RotateDirection direction = RotateDirection.Clockwise;

    // 旧Input Managerでのボタン名
    private const string L_BUTTON = "joystick button 4";  // L1 / LB
    private const string R_BUTTON = "joystick button 5";  // R1 / RB

    void Update()
    {
        // 担当ボタンが押されているか調べる
        bool isPressed = false;
        for (int i = 0; i < 20; i++)
        {
            if (Input.GetKeyDown("joystick button " + i))
            {
                Debug.Log("押されたボタン番号: " + i);
            }
        }
        if (controlButton == ControlButton.R_Button)
        {
            isPressed = Input.GetKey(KeyCode.JoystickButton5);  // R1
        }
        else
        {
            isPressed = Input.GetKey(KeyCode.JoystickButton4);  // L1
        }

        // 押している間だけ回す
        if (isPressed)
        {
            // 時計回りはマイナス、反時計回りはプラス
            float dir = (direction == RotateDirection.Clockwise) ? -1f : 1f;

            // Z軸まわりに回転（中心が原点なので中心を軸に公転する）
            transform.Rotate(0f, 0f, rotateSpeed * dir * Time.deltaTime);
        }
    }
}