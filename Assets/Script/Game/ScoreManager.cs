using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    // どこからでもアクセスできる唯一のインスタンス
    public static ScoreManager Instance;

    private int currentScore = 0;   // 現在のスコア

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 得点を加算する（あなたの得点処理から呼ぶ）
    public void AddScore(int amount)
    {
        currentScore += amount;
    }

    // 現在のスコアを取得する（相手はこれを読んで表示する）
    public int GetScore()
    {
        return currentScore;
    }
}