using UnityEngine;

// =============================================
// Stage score display / ステージスコア表示クラス
// =============================================
// EN: Lives in the Stage scene. Continuously checks ScoreManager's current score and refreshes
//     the digit display only when it actually changes (avoids unnecessary sprite reassignment).
// JP: Stageシーンに配置する。ScoreManagerの現在のスコアを継続的に確認し、実際に変化した時のみ
//     桁の表示を更新する（不要なスプライトの再設定を避ける）。
public class StageScoreDisplay : MonoBehaviour
{
    [Header("Score Digits Display (renders the number)")]
    [SerializeField] private ScoreDigitsDisplay scoreDigitsDisplay;

    private int lastDisplayedScore = -1; // EN: -1 ensures the first frame always refreshes / JP: -1にすることで、最初のフレームで必ず更新されるようにする

    private void Update()
    {
        int currentScore = ScoreManager.Instance.GetScore();

        if (currentScore != lastDisplayedScore)
        {
            if (scoreDigitsDisplay != null) scoreDigitsDisplay.Refresh(currentScore);
            lastDisplayedScore = currentScore;
        }
    }
}