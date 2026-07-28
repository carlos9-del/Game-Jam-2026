using UnityEngine;
using UnityEngine.UI;

// =============================================
// Score display / スコア表示クラス
// =============================================
// EN: Lives in the Result scene. Reads the final score from ScoreManager, hands it to a
//     ScoreDigitsDisplay component to render the digits, and calculates + displays the rank
//     (S/A/B/C) based on the score.
// JP: Resultシーンに配置する。ScoreManagerから最終スコアを取得し、ScoreDigitsDisplayコンポーネントに
//     渡して桁を表示させ、スコアに応じてランク（S/A/B/C）を計算・表示する。
public class ScoreDisplay : MonoBehaviour
{
    [Header("Score Digits Display (renders the number)")]
    [SerializeField] private ScoreDigitsDisplay scoreDigitsDisplay;

    [Header("Rank Image Slot")]
    [SerializeField] private Image rankImageSlot;

    [Header("Rank Sprites (index 0 = S, 1 = A, 2 = B, 3 = C)")]
    [SerializeField] private Sprite[] rankSprites = new Sprite[4];

    [Header("Rank Thresholds (minimum score needed for each rank)")]
    [SerializeField] private int rankSMinScore = 15000;
    [SerializeField] private int rankAMinScore = 10000;
    [SerializeField] private int rankBMinScore = 5000;

    private void Start()
    {
        int finalScore = ScoreManager.Instance.GetScore();

        if (scoreDigitsDisplay != null) scoreDigitsDisplay.Refresh(finalScore);
        DisplayRank(finalScore);

        // EN: Save this score to the persistent high score list.
        // JP: このスコアを、永続保存されるハイスコアリストに保存する。
        HighScoreManager.Instance.SaveScore(finalScore);

        Debug.Log("Saved score: " + finalScore);
    }

    // EN: Calculates the rank based on the score and displays the matching sprite.
    // JP: スコアに応じてランクを判定し、対応するスプライトを表示する。
    private void DisplayRank(int score)
    {
        if (rankImageSlot == null || rankSprites == null || rankSprites.Length < 4) return;

        int rankIndex;
        if (score >= rankSMinScore) rankIndex = 0;       // S
        else if (score >= rankAMinScore) rankIndex = 1;  // A
        else if (score >= rankBMinScore) rankIndex = 2;  // B
        else rankIndex = 3;                              // C

        rankImageSlot.sprite = rankSprites[rankIndex];
    }
}

//using UnityEngine;
//using UnityEngine.UI;

//// =============================================
//// Score display / スコア表示クラス
//// =============================================
//// EN: Lives in the Result scene. Reads the final score from ScoreManager, hands it to a
////     ScoreDigitsDisplay component to render the digits, and calculates + displays the rank
////     (S/A/B/C) based on the score.
//// JP: Resultシーンに配置する。ScoreManagerから最終スコアを取得し、ScoreDigitsDisplayコンポーネントに
////     渡して桁を表示させ、スコアに応じてランク（S/A/B/C）を計算・表示する。
//public class ScoreDisplay : MonoBehaviour
//{
//    [Header("Score Digits Display (renders the number)")]
//    [SerializeField] private ScoreDigitsDisplay scoreDigitsDisplay;

//    [Header("Rank Image Slot")]
//    [SerializeField] private Image rankImageSlot;

//    [Header("Rank Sprites (index 0 = S, 1 = A, 2 = B, 3 = C)")]
//    [SerializeField] private Sprite[] rankSprites = new Sprite[4];

//    [Header("Rank Thresholds (minimum score needed for each rank)")]
//    [SerializeField] private int rankSMinScore = 15000;
//    [SerializeField] private int rankAMinScore = 10000;
//    [SerializeField] private int rankBMinScore = 5000;

//    private void Start()
//    {
//        int finalScore = ScoreManager.Instance.GetScore();

//        if (scoreDigitsDisplay != null) scoreDigitsDisplay.Refresh(finalScore);
//        DisplayRank(finalScore);
//    }

//    // EN: Calculates the rank based on the score and displays the matching sprite.
//    // JP: スコアに応じてランクを判定し、対応するスプライトを表示する。
//    private void DisplayRank(int score)
//    {
//        if (rankImageSlot == null || rankSprites == null || rankSprites.Length < 4) return;

//        int rankIndex;
//        if (score >= rankSMinScore) rankIndex = 0;       // S
//        else if (score >= rankAMinScore) rankIndex = 1;  // A
//        else if (score >= rankBMinScore) rankIndex = 2;  // B
//        else rankIndex = 3;                              // C

//        rankImageSlot.sprite = rankSprites[rankIndex];
//    }
//}