using UnityEngine;

public class BlackHole : MonoBehaviour
{
    [Header("吸引の設定")]
    public float pullRange = 3f;        // 吸引が効く範囲（この中に入ると引っ張られる）
    public float pullStrength = 20f;    // 吸引力の強さ
    public float captureRange = 0.5f;   // この距離まで近づくと?み込む

    [Header("スコア")]
    public int scoreValue = 10;         // ?み込んだ時の得点

    void FixedUpdate()
    {
        // 範囲内のボールを探す
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, pullRange);

        foreach (Collider2D hit in hits)
        {
            // ボール本体のスクリプトを取得する
            BallLauncher ball = hit.GetComponent<BallLauncher>();
            if (ball == null) continue;

            // 吸い込みを開始させる（以降はボール側が螺旋制御する）
            // すでに吸われ中なら、ボール側で二重処理を防いでいる
            ball.StartSuckedByBlackHole(this);
        }
    }


    // ボールを?み込む
    public void OnBallCaptured(GameObject ball)
    {
      //  Debug.Log("黒洞がボールを?み込んだ！ +" + scoreValue);
      //  Destroy(ball);
    }

    // Sceneビューで範囲を可視化する（デバッグ用）
    void OnDrawGizmosSelected()
    {
        // 吸引範囲：黄色
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pullRange);

        // ?み込み範囲：赤
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, captureRange);
    }
}