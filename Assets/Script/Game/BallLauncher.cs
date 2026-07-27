using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BallLauncher : MonoBehaviour
{
    [Header("回転の設定")]
    public Vector2 center = Vector2.zero;   // 回転中心
    public float startRadius = 0.3f;        // 開始時の半径（中心付近）
    public float maxRadius = 3f;            // 最大半径（外側の限界）
    public float expandSpeed = 0.8f;        // 半径が広がる速さ（単位／秒）
    public float angularSpeed = 180f;       // 角速度（度／秒）

    [Header("射出の設定")]
    public float launchMultiplier = 2f;    // 接線速度にかける倍率

    private Rigidbody2D rb;
    private BallSpawner spawner;
    private float currentAngle = 0f;        // 現在の角度（度）
    private float currentRadius;            // 現在の半径
    private bool isLaunched = false;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        // 半径を開始値にセット
        currentRadius = startRadius;
    }

    void Update()
    {
        if (!isLaunched)
        {
            SpiralOutward();

            if (Input.GetKeyDown(KeyCode.J))
            {
                Launch();
            }
        }
    }

    // 中心から外へ広がりながら回る（螺旋）
    void SpiralOutward()
    {
        // 角度を進める
        currentAngle += angularSpeed * Time.deltaTime;

        // 半径を少しずつ広げる（最大値で止める）
        currentRadius += expandSpeed * Time.deltaTime;
        currentRadius = Mathf.Min(currentRadius, maxRadius);

        float rad = currentAngle * Mathf.Deg2Rad;
        float x = center.x + Mathf.Cos(rad) * currentRadius;
        float y = center.y + Mathf.Sin(rad) * currentRadius;

        rb.position = new Vector2(x, y);
    }

    // 接線方向へ射出する
    void Launch()
    {
        isLaunched = true;
        rb.bodyType = RigidbodyType2D.Dynamic;

        float rad = currentAngle * Mathf.Deg2Rad;
        Vector2 tangent = new Vector2(-Mathf.Sin(rad), Mathf.Cos(rad));

        // 現在の接線速度を計算する： v = ω × r
        // angularSpeed（度／秒）をラジアン／秒に変換してから半径を掛ける
        float angularSpeedRad = angularSpeed * Mathf.Deg2Rad;
        float tangentSpeed = angularSpeedRad * currentRadius;

        // 発射倍率をかけて調整できるようにする
        rb.linearVelocity = tangent * tangentSpeed * launchMultiplier;
        // スポナーに「もう回っていない」と伝える
        if (spawner != null)
        {
            spawner.OnBallLaunched(this.gameObject);
        }
    }
    public void SetStartAngle(float angleDeg)
    {
        currentAngle = angleDeg;
    }
    public void SetSpawner(BallSpawner s)
    {
        spawner = s;
    }
}