using UnityEngine;

public class RingRotator : MonoBehaviour
{
    [Header("回転の設定")]
    public float rotateSpeed = 90f;     // 回転速度（度／秒）

    // どちらのスティックで回すか
    public enum ControlStick { L_Stick, R_Stick }
    public ControlStick controlStick = ControlStick.R_Stick;

    [Header("入力の調整")]
    public float deadZone = 0.2f;       // これ以下の傾きは無視する（誤作動防止）

    // Input Managerで設定した軸の名前
    private const string R_STICK_AXIS = "RStickHorizontal";  // 右スティック横
    private const string L_STICK_AXIS = "LStickHorizontal";  // 左スティック横

    void Update()
    {
        // 担当スティックの横方向の傾きを読む（-1.0?+1.0）
        float input = 0f;

        if (controlStick == ControlStick.R_Stick)
        {
            input = Input.GetAxis(R_STICK_AXIS);
        }
        else
        {
            input = Input.GetAxis(L_STICK_AXIS);
        }

        // 小さすぎる傾きは無視する（スティックの遊び対策）
        if (Mathf.Abs(input) < deadZone) return;

        // 傾きの符号で回転方向が決まる
        // 左に倒す（input < 0）→ 反時計回り（プラス回転）
        // 右に倒す（input > 0）→ 時計回り（マイナス回転）
        // なので -input を使う
        float rotateAmount = -input * rotateSpeed * Time.deltaTime;

        // Z軸まわりに回転（中心が原点なので中心を軸に公転する）
        transform.Rotate(0f, 0f, rotateAmount);
    }
}