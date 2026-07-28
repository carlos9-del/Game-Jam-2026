using UnityEngine;

public class SimpleRotator : MonoBehaviour
{
    [Header("回転の設定")]
    public float speed = 90f;       // 回転速度（度／秒）
    public bool clockwise = true;   // 時計回りにするか（チェックを外すと逆回転）

    void Update()
    {
        // 方向を決める（時計回りはマイナス、反時計回りはプラス）
        float dir = clockwise ? -1f : 1f;

        // Z軸まわりに回す
        transform.Rotate(0f, 0f, speed * dir * Time.deltaTime);
    }
}