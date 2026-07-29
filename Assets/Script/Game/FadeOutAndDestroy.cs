using UnityEngine;

// 時間経過で透明になって消えるスクリプト（どの物体にも使える）
//[RequireComponent(typeof(SpriteRenderer))]
public class FadeOutAndDestroy : MonoBehaviour
{
    [Header("フェードの設定")]
    public float delay = 0f;        // 消え始めるまでの待ち時間（秒）
    public float fadeTime = 1f;     // 透明になるまでの時間（秒）

    private SpriteRenderer sr;
    private float timer = 0f;        // 経過時間

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        // まだ待ち時間中なら何もしない
        if (timer < delay) return;

        // 待ち時間が終わってからの経過時間
        float fadeElapsed = timer - delay;

        // フェードの進み具合（0→1）
        float t = fadeElapsed / fadeTime;

        // alpha を 1 から 0 へ減らす
        float alpha = Mathf.Lerp(1f, 0f, t);

        // 色はそのまま、透明度だけ変える
        Color c = sr.color;
        c.a = alpha;
        sr.color = c;

        // 完全に透明になったら自分を消す
        if (t >= 1f)
        {
            Destroy(gameObject);
        }
    }
}