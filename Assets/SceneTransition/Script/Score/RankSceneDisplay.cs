using UnityEngine;

// =============================================
// Rank scene display / ランクシーン表示クラス
// =============================================
// EN: Lives in the Rank scene. Loads the saved top scores and fills in each pre-placed row's
//     digit display. Rows are placed manually in the scene (position, size, and "TopX:" label
//     sprite are all set directly in the Editor) - this script only updates the numbers and
//     hides rows that have no saved score yet.
// JP: Rankシーンに配置する。保存された上位スコアを読み込み、あらかじめ配置された各行の
//     桁表示に反映する。行はシーン内に手動で配置する（位置・サイズ・「TopX:」ラベルの
//     スプライトは全てエディタ上で直接設定する）- このスクリプトは数値の更新と、
//     まだスコアが保存されていない行の非表示のみを行う。
public class RankSceneDisplay : MonoBehaviour
{
    [Header("Row Root Objects (index 0 = 1st place, ... last index = last place)")]
    [SerializeField] private GameObject[] rowRoots;

    [Header("Row Digit Displays (same order as Row Root Objects)")]
    [SerializeField] private ScoreDigitsDisplay[] rowDisplays;

    private void Start()
    {
        HighScoreData data = HighScoreManager.Instance.LoadScores();

        for (int i = 0; i < rowRoots.Length; i++)
        {
            bool hasScore = i < data.scores.Count;

            if (rowRoots[i] != null) rowRoots[i].SetActive(hasScore);

            if (hasScore && i < rowDisplays.Length && rowDisplays[i] != null)
            {
                rowDisplays[i].Refresh(data.scores[i]);
            }
        }
    }
}