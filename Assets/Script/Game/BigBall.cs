using UnityEngine;

public class BigBall : MonoBehaviour
{
    [Header("膨張の設定")]
    public float targetScale = 3f;       // 最終的に何倍まで膨らむか（調整可）
    public float growSpeed = 2f;         // 膨らむ速さ

    [Header("爆発")]
    public GameObject explosionEffect;   // 爆発エフェクト（syoumetuを使う）

    private Vector3 finalScale;          // 目標の大きさ
    private bool exploded = false;

    void Start()
    {
        // 開始時の大きさを基準に、目標をtargetScale倍にする
        finalScale = transform.localScale * targetScale;
    }

    void Update()
    {
        if (exploded) return;

        // 少しずつ目標サイズへ膨らむ
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            finalScale,
            growSpeed * Time.deltaTime
        );

        // 目標サイズにほぼ達したら爆発する
        if (Vector3.Distance(transform.localScale, finalScale) < 0.05f)
        {
            Explode();
        }
    }

    void Explode()
    {
        exploded = true;

        // 爆発エフェクトを出す（syoumetu）
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // 自分を消す
        Destroy(gameObject);
    }
}