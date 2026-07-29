using UnityEngine;
using System.Collections.Generic;

public class BlackHole : MonoBehaviour
{
    [Header("吸引の設定")]
    public float pullRange = 3f;            // スキャン範囲

    [Header("色の設定")]
    public BallColorType holeColor;         // この穴（ゴール）の色

    [Header("同色結算の演出")]
    public GameObject bigBallPrefab;    // 結算時に出す大きい球のPrefab

    [Header("同じ色の蓄積")]
    public int requiredCount = 4;           // 同色が何個で結算するか

    [Header("スコア計算")]
    public int baseScore = 1;               // 基礎値
    public int sizeMultiplier = 1;          // サイズ段階にかける倍率
    public int sameColorBonus = 10;          // 同じ色の時の倍率

    // 同じ色のボールを溜めておくリスト
    private List<BallLauncher> sameColorBalls = new List<BallLauncher>();

    void FixedUpdate()
    {
        // 範囲内のボールを探す
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, pullRange);

        foreach (Collider2D hit in hits)
        {
            BallLauncher ball = hit.GetComponent<BallLauncher>();
            if (ball == null) continue;
            if (!ball.IsLaunched()) continue;   // 飛んでいる球だけ
            if (ball.IsInGoal()) continue;      // 二重処理防止

            ball.SetInGoal(true);

            // 色で分岐する
            if (ball.GetColor() == holeColor)
            {
                AcceptSameColor(ball);       // 同じ色
            }
            else
            {
                AcceptDifferentColor(ball);  // 違う色
            }
        }
    }

    // ===== 同じ色：中心に留めて溜める =====
    void AcceptSameColor(BallLauncher ball)
    {
        // 中心へ螺旋させる。中心に着いたら OnSameColorReached を呼ぶ（消さずに留める）
        ball.StartSuckSpiral(transform.position, false, () => OnSameColorReached(ball));

        Debug.Log(holeColor + "：同色ボールが中心へ向かう…");
    }

    // 溜まった同色ボールをまとめて得点化
    void SettleSameColor()
    {
        int total = 0;

        foreach (BallLauncher ball in sameColorBalls)
        {
            if (ball == null) continue;

            // 得点は各球のサイズ段階から計算（従来通り）
            int stage = ball.GetSizeStage();
            total += ((baseScore + stage * sizeMultiplier) * sameColorBonus) * 10;

            ColorCounter.Instance.AddCount(ball.GetColor());
            // 4個は「そっと」消す（個別の消滅エフェクトは出さない）
            Destroy(ball.gameObject);
        }

        Debug.Log(holeColor + "：同色そろって結算！ +" + total);
        SendScore(total);

        sameColorBalls.Clear();

        // 中心に大きい球を生成する（以降の演出は大球が担当）
        if (bigBallPrefab != null)
        {
            Instantiate(bigBallPrefab, transform.position, Quaternion.identity);
        }
    }

    // ===== 違う色：螺旋で吸い込んで即結算 =====
    void AcceptDifferentColor(BallLauncher ball)
    {
        int stage = ball.GetSizeStage();
        int score = (baseScore) * 0 + 1; //(baseScore + stage * sizeMultiplier) * 1;

        Debug.Log(holeColor + "：違う色 即結算 +" + score);
        SendScore(score);

        ColorCounter.Instance.AddCount(ball.GetColor());
        ball.StartSuckSpiral(transform.position, true, null);
    }

    // 時間切れ時：揃わなかった残りは0点で消す
    public void ClearRemaining()
    {
        foreach (BallLauncher ball in sameColorBalls)
        {
            if (ball != null)
                ball.PlaySyoumetuEffect();
            Destroy(ball.gameObject);
        }
        sameColorBalls.Clear();
    }

    void SendScore(int amount)
    {
        Debug.Log("得点を送信：+" + amount);
        ScoreManager.Instance.AddScore(amount);  // ← 本物のScoreManagerが来たら有効化
    }

    void OnSameColorReached(BallLauncher ball)
    {
        // ここで初めて「就位した」とカウントする
        sameColorBalls.Add(ball);

        Debug.Log(holeColor + "：中心に到着 " + sameColorBalls.Count + "/" + requiredCount);

        // 中心に着いた球が必要数そろったら、まとめて結算（爆発）
        if (sameColorBalls.Count > requiredCount)
        {
            SettleSameColor();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pullRange);
    }
}