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

    [Header("反射の設定")]
    public int reflectCount = 0;        // 反射した回数（＝サイズ段階）
    public int maxReflect = 5;          // 反射の上限
    public float sizeStep = 0.2f;       // 1回の反射で増えるサイズ

    [Header("射出の設定")]
    public float launchMultiplier = 2f;    // 接線速度にかける倍率

    private Rigidbody2D rb;
    private BallSpawner spawner;
    private float currentAngle = 0f;        // 現在の角度（度）
    private float currentRadius;            // 現在の半径
    private bool isLaunched = false;
    private Vector3 baseScale;          // 元の大きさを覚えておく


    [Header("ブラックホール吸い込み")]
    public float shrinkSpeed = 1.5f;    // 半径が縮む速さ（吸い込みの強さ）
    public float suckAngularSpeed = 360f; // 吸い込み中の回転速度（度／秒）
    public float captureRadius = 0.3f;  // この半径まで縮んだら?み込まれる

    private bool isSucked = false;      // ブラックホールに吸われ中か
    private BlackHole currentHole;      // 吸っているブラックホール
    private float suckAngle;            // 吸い込み螺旋の現在の角度
    private float suckRadius;           // 吸い込み螺旋の現在の半径

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        currentRadius = startRadius;

        // 元のサイズを保存しておく
        baseScale = transform.localScale;
    }

    void Update()
    {
        // ブラックホールに吸われ中なら、それだけを処理する
        if (isSucked)
        {
            SpiralIntoHole();
            return;
        }

        if (!isLaunched)
        {
            SpiralOutward();
            if (Input.GetKeyDown(KeyCode.JoystickButton0))
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
    // 何かに衝突した瞬間に呼ばれる
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 射出前（回っている間）は数えない
        if (!isLaunched) return;

        // 壁に当たった時だけ数える（ボール同士は数えない）
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReflect();
        }
    }

    // 反射回数を増やしてサイズを大きくする
    void AddReflect()
    {
        // 上限に達していたら何もしない
        if (reflectCount >= maxReflect) return;

        reflectCount++;

        // サイズ段階に応じて大きくする
        // 例：0回→1.0倍、5回→2.0倍（1.0 + 0.2 × 回数）
        float scale = 1f + sizeStep * reflectCount;
        transform.localScale = baseScale * scale;
    }

    // ブラックホールに吸い込まれ始める
    public void StartSuckedByBlackHole(BlackHole hole)
    {
        // すでに吸われ中なら二重処理しない
        if (isSucked) return;

        Debug.Log("吸い込み開始！ 現在の速度: " + rb.linearVelocity.magnitude);

        isSucked = true;
        currentHole = hole;

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;

        // 今の位置からブラックホール中心への相対位置を求める
        Vector2 holeCenter = hole.transform.position;
        Vector2 offset = rb.position - holeCenter;

        // 現在の半径と角度を、入った瞬間の位置から逆算する
        // これで螺旋が今いる場所から滑らかに始まる
        suckRadius = offset.magnitude;
        suckAngle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
    }
    // ブラックホールへ向かって内向きに螺旋する
    void SpiralIntoHole()
    {
        if (currentHole == null) return;

        // 角度を進める（回りながら）
        suckAngle += suckAngularSpeed * Time.deltaTime;

        // 半径を少しずつ縮める
        suckRadius -= shrinkSpeed * Time.deltaTime;

        // 最小半径で止める（消さずに、その場で回り続ける）
        suckRadius = Mathf.Max(suckRadius, captureRadius);

        // ブラックホール中心を基準に螺旋位置を計算する
        Vector2 holeCenter = currentHole.transform.position;
        float rad = suckAngle * Mathf.Deg2Rad;
        float x = holeCenter.x + Mathf.Cos(rad) * suckRadius;
        float y = holeCenter.y + Mathf.Sin(rad) * suckRadius;

        rb.position = new Vector2(x, y);
    }
}