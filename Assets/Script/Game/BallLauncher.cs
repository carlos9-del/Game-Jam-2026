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
    public float launchSpeed = 15f;     // 射出スピード（固定・調整可能）

    [Header("反射の設定")]
    public int reflectCount = 0;            // 反射した回数（＝サイズ段階）
    public int maxReflect = 5;              // 反射の上限
    public float sizeStep = 0.2f;           // 1回の反射で増えるサイズ

    [Header("吸い込み螺旋の設定")]
    public float shrinkSpeed = 1.5f;        // 半径が縮む速さ
    public float suckAngularSpeed = 360f;   // 吸い込み中の回転速度（度／秒）
    public float captureRadius = 0.3f;      // この半径まで縮んだら「到達」

    [Header("色の設定")]
    public BallColorType ballColor;         // このボールの色

    // ===== 内部の状態 =====
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector3 baseScale;              // 元の大きさ

    private float currentAngle = 0f;        // 現在の角度（度）
    private float currentRadius;            // 現在の半径
    private bool isLaunched = false;        // 射出済みか
    private float launchSpeedFixed;   // 射出時の速さ（反射後もこれを保つ）
    private BallSpawner spawner;            // 自分を生成したスポナー

    // ===== 吸い込み状態（黒洞・ゴール共通）=====
    private bool isSucked = false;          // 吸い込み螺旋の最中か
    private Vector2 suckCenter;             // 螺旋の中心
    private float suckAngle;                // 螺旋の現在の角度
    private float suckRadius;               // 螺旋の現在の半径
    private bool destroyWhenReach = false;  // 中心到達で消すか
    private System.Action onReachCenter;    // 中心到達時に呼ぶ処理

    private bool isInGoal = false;          // ゴール処理中か

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

    }
    void Start()
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
        currentRadius = startRadius;
        baseScale = transform.localScale;
    }
    void Update()
    {
        if (isSucked)
        {
            SpiralIntoCenter();
            return;
        }

        // 蓄積中はただ回るだけ（射出はスポナーが命令する）
        if (!isLaunched)
        {
            SpiralOutward();
        }
    }


    // ===== 蓄積：中心から外へ広がりながら回る =====
    void SpiralOutward()
    {
        currentAngle += angularSpeed * Time.deltaTime;

        currentRadius += expandSpeed * Time.deltaTime;
        currentRadius = Mathf.Min(currentRadius, maxRadius);

        float rad = currentAngle * Mathf.Deg2Rad;
        float x = center.x + Mathf.Cos(rad) * currentRadius;
        float y = center.y + Mathf.Sin(rad) * currentRadius;
        rb.position = new Vector2(x, y);
    }

    // ===== 射出：接線方向へ飛ばす =====
    public void Launch()
    {
        if (isLaunched || isSucked) return;
        if (rb == null) rb = GetComponent<Rigidbody2D>();

        isLaunched = true;
        rb.bodyType = RigidbodyType2D.Dynamic;

        // 方向は接線方向のまま（射出角度は回転で決まる）
        float rad = currentAngle * Mathf.Deg2Rad;
        Vector2 tangent = new Vector2(-Mathf.Sin(rad), Mathf.Cos(rad));

        // 速さは固定値を使う（半径に依存しない）
        rb.linearVelocity = tangent * launchSpeed;

        // 射出時の速さを覚えておく（反射後もこの速さを保つ）
        launchSpeedFixed = launchSpeed;

        if (spawner != null)
        {
            spawner.OnBallLaunched(this.gameObject);
        }
    }
    // ===== 反射：壁に当たった時 =====
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isLaunched) return;

        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReflect();

            // 反射後、速度の大きさを一定に保つ
            KeepSpeedConstant();
        }
    }

    // 反射で方向は変わっても、速さ（大きさ）は変えない
    void KeepSpeedConstant()
    {
        // 物理エンジンが計算した「反射方向」はそのまま使う
        // 「速さ」だけを狙った値に固定する
        Vector2 dir = rb.linearVelocity.normalized;
        rb.linearVelocity = dir * launchSpeedFixed;
    }
    void AddReflect()
    {
        if (reflectCount >= maxReflect) return;

        reflectCount++;
        float scale = 1f + sizeStep * reflectCount;
        transform.localScale = baseScale * scale;
    }

    // ===== 吸い込み螺旋：黒洞・ゴール共通の入口 =====
    // center : 螺旋の中心
    // destroy: 中心到達でこのボールを消すか
    // onReach: 中心到達時に呼びたい処理（無ければnull）
    public void StartSuckSpiral(Vector2 center, bool destroy, System.Action onReach)
    {
        if (isSucked) return;
        isSucked = true;

        suckCenter = center;
        destroyWhenReach = destroy;
        onReachCenter = onReach;

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;

        // 入った瞬間の位置から半径・角度を逆算（滑らかに繋ぐ）
        Vector2 offset = rb.position - center;
        suckRadius = offset.magnitude;
        suckAngle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
    }

    // 中心へ向かって内向きに螺旋する
    void SpiralIntoCenter()
    {
        suckAngle += suckAngularSpeed * Time.deltaTime;
        suckRadius -= shrinkSpeed * Time.deltaTime;

        // 中心に到達したら
        if (suckRadius <= captureRadius)
        {
            suckRadius = captureRadius;

            // 到達時の処理を1回だけ呼ぶ
            if (onReachCenter != null)
            {
                onReachCenter.Invoke();
                onReachCenter = null;
            }

            if (destroyWhenReach)
            {
                Destroy(gameObject);
                return;
            }
        }

        float rad = suckAngle * Mathf.Deg2Rad;
        float x = suckCenter.x + Mathf.Cos(rad) * suckRadius;
        float y = suckCenter.y + Mathf.Sin(rad) * suckRadius;
        rb.position = new Vector2(x, y);
    }



    // ===== 外部インターフェース =====

    public void SetStartAngle(float angleDeg)
    {
        currentAngle = angleDeg;
    }

    public void SetSpawner(BallSpawner s)
    {
        spawner = s;
    }

    public void SetColorType(BallColorType color)
    {
        ballColor = color;

        if (sr == null) sr = GetComponent<SpriteRenderer>();
        sr.color = GetColorValue(color);
    }

    Color GetColorValue(BallColorType type)
    {
        switch (type)
        {
            case BallColorType.Red: return Color.red;
            case BallColorType.Yellow: return Color.yellow;
            case BallColorType.Blue: return Color.blue;
            case BallColorType.Green: return Color.green;
            default: return Color.white;
        }
    }

    public BallColorType GetColor()
    {
        return ballColor;
    }

    public int GetSizeStage()
    {
        return reflectCount;
    }

    public bool IsLaunched()
    {
        return isLaunched;
    }

    // ゴール処理中か（二重処理防止）
    public bool IsInGoal()
    {
        return isInGoal;
    }

    public void SetInGoal(bool value)
    {
        isInGoal = value;
    }
}