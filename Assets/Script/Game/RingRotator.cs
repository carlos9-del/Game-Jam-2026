using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RingRotator : MonoBehaviour
{
    [Header("回転速度の設定")]
    public float minSpeed = 30f;        // 最低速度
    public float maxSpeed = 180f;       // 最高速度
    public float accelTime = 5f;        // 最低から最高まで加速する時間（秒）

    // 現在の回転速度（BallSpawnerがこれを読む）
    public float CurrentSpeed { get; private set; }

    // 現在の回転方向（1 = 右/RB, -1 = 左/LB, 0 = なし）
    private int currentDir = 0;

    private Rigidbody2D rb;

    // 旧Input Managerのボタン（LB=4, RB=5）
    private const KeyCode LB = KeyCode.JoystickButton4;
    private const KeyCode RB = KeyCode.JoystickButton5;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        CurrentSpeed = 0f;
    }

    void Update()
    {
        // 入力の判定はUpdateで行う（取りこぼしを防ぐ）
        HandleInput();
    }

    void FixedUpdate()
    {
        // 物理的な回転はFixedUpdateで行う（貫通対策）
        ApplyRotation();
    }

    void HandleInput()
    {
        float accelPerSec = (maxSpeed - minSpeed) / accelTime;

        // 押した瞬間に方向を採用（同時押しでも後押しが勝つ）
        if (Input.GetKeyDown(RB)) currentDir = 1;
        if (Input.GetKeyDown(LB)) currentDir = -1;

        bool rbHeld = Input.GetKey(RB);
        bool lbHeld = Input.GetKey(LB);

        // どちらも押していない → 速度を即ゼロにする（回転停止）
        if (!rbHeld && !lbHeld)
        {
            CurrentSpeed = 0f;
            currentDir = 0;
            return;
        }

        // 押している方向に応じて速度を変化させる
        if (currentDir == 1 && rbHeld)
        {
            CurrentSpeed += accelPerSec * Time.deltaTime;
        }
        else if (currentDir == -1 && lbHeld)
        {
            CurrentSpeed += accelPerSec * Time.deltaTime;
        }
        else if (rbHeld && currentDir == -1)
        {
            // 逆方向を押した → 速度半減して方向転換
            CurrentSpeed *= 0.5f;
            currentDir = 1;
        }
        else if (lbHeld && currentDir == 1)
        {
            CurrentSpeed *= 0.5f;
            currentDir = -1;
        }

        // 押している間は最低30?最高maxSpeedに収める
        CurrentSpeed = Mathf.Clamp(CurrentSpeed, minSpeed, maxSpeed);
    }

    void ApplyRotation()
    {
        if (currentDir == 0) return;

        float rotateAmount = -currentDir * CurrentSpeed * Time.fixedDeltaTime;

        // 物理エンジン経由で回転（transform.Rotateではなくこれを使う）
        rb.MoveRotation(rb.rotation + rotateAmount);
    }
}