using UnityEngine;

// =============================================
// Score manager / スコア管理クラス
// =============================================
// EN: Holds the current score and persists across scenes (Stage -> Result), so the score
//     earned in Stage is still available once Result loads.
// JP: 現在のスコアを保持し、シーンをまたいで（Stage -> Result）保持されるようにする。
//     Stageで獲得したスコアが、Resultが読み込まれた後も参照できるようにする。
public class ScoreManager : MonoBehaviour
{
    // EN: The one and only instance, accessible from anywhere. / JP: どこからでもアクセスできる唯一のインスタンス。
    public static ScoreManager Instance;

    private int currentScore = 0; // EN: the current score / JP: 現在のスコア

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // EN: Without this, ScoreManager is destroyed when Stage unloads, and Result
            //     would always read a fresh score of 0 instead of the real value.
            // JP: これがないと、Stageがアンロードされた際にScoreManagerが破棄され、
            //     Resultは常に新しい（0の）スコアを読み取ってしまう。
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // EN: Adds to the current score (called from your scoring logic). / JP: 得点を加算する（あなたの得点処理から呼ぶ）。
    public void AddScore(int amount)
    {
        currentScore += amount;
    }

    // EN: Gets the current score (Result reads this to display it). / JP: 現在のスコアを取得する（相手はこれを読んで表示する）。
    public int GetScore()
    {
        return currentScore;
    }
}

//using UnityEngine;

//public class ScoreManager : MonoBehaviour
//{
//    // どこからでもアクセスできる唯一のインスタンス
//    public static ScoreManager Instance;

//    private int currentScore = 0;   // 現在のスコア

//    void Awake()
//    {
//        if (Instance == null) Instance = this;
//        else Destroy(gameObject);
//    }

//    // 得点を加算する（あなたの得点処理から呼ぶ）
//    public void AddScore(int amount)
//    {
//        currentScore += amount;
//    }

//    // 現在のスコアを取得する（相手はこれを読んで表示する）
//    public int GetScore()
//    {
//        return currentScore;
//    }
//}