using UnityEngine;
using UnityEngine.UI;

// =============================================
// Rank row / ランク行クラス
// =============================================
// EN: Attach to the root of the rank row prefab. Exposes references to this row's label image
//     and digit display, so RankSceneDisplay can configure each instantiated row.
// JP: ランク行のプレハブのルートにアタッチする。この行のラベル画像と桁表示への参照を公開し、
//     RankSceneDisplayが生成した各行を設定できるようにする。
public class RankRow : MonoBehaviour
{
    [Header("This Row's Label Image (e.g. 'Top1:')")]
    [SerializeField] private Image topLabelImage;

    [Header("This Row's Score Digits Display")]
    [SerializeField] private ScoreDigitsDisplay scoreDigitsDisplay;

    // EN: Configures this row with the given label sprite and score value.
    // JP: 指定されたラベルスプライトとスコア値でこの行を設定する。
    public void Setup(Sprite labelSprite, int score)
    {
        if (topLabelImage != null) topLabelImage.sprite = labelSprite;
        if (scoreDigitsDisplay != null) scoreDigitsDisplay.Refresh(score);
    }
}