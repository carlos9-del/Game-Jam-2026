using UnityEngine;
using UnityEngine.UI;

// =============================================
// Score digits display / スコア桁表示クラス
// =============================================
// EN: Reusable component that renders a number (0-99999) across 5 digit slots, hiding leading
//     zeros. Used by both the Stage scene (live-updating score) and the Result scene (final score).
//     Does not read the score itself - call Refresh(value) whenever the displayed number should update.
// JP: 数値（0～99999）を5つの桁スロットに表示する再利用可能なコンポーネント。先頭の0は非表示にする。
//     Stageシーン（リアルタイム更新するスコア）とResultシーン（最終スコア）の両方で使用する。
//     スコア自体は読み取らない - 表示する数値を更新したい時にRefresh(value)を呼び出す。
public class ScoreDigitsDisplay : MonoBehaviour
{
    [Header("Digit Sprites (index 0 = '0', index 9 = '9')")]
    [SerializeField] private Sprite[] digitSprites = new Sprite[10];

    [Header("Digit Slots (left to right)")]
    [SerializeField] private Image tenThousandsDigitSlot;
    [SerializeField] private Image thousandsDigitSlot;
    [SerializeField] private Image hundredsDigitSlot;
    [SerializeField] private Image tensDigitSlot;
    [SerializeField] private Image onesDigitSlot;

    // EN: Updates the digit slots to always show all 5 digits (0-99999), including leading zeros.
    // JP: 常に5桁すべてを表示するように桁スロットを更新する（先頭の0も含む）。
    public void Refresh(int value)
    {
        if (digitSprites == null || digitSprites.Length < 10) return;

        value = Mathf.Clamp(value, 0, 99999);
        int tenThousands = (value / 10000) % 10;
        int thousands = (value / 1000) % 10;
        int hundreds = (value / 100) % 10;
        int tens = (value / 10) % 10;
        int ones = value % 10;

        SetDigitSlot(tenThousandsDigitSlot, tenThousands);
        SetDigitSlot(thousandsDigitSlot, thousands);
        SetDigitSlot(hundredsDigitSlot, hundreds);
        SetDigitSlot(tensDigitSlot, tens);
        SetDigitSlot(onesDigitSlot, ones);
    }

    // EN: Assigns the correct sprite to a digit slot. / JP: 桁スロットに対応するスプライトを設定する。
    private void SetDigitSlot(Image slot, int digit)
    {
        if (slot == null) return;
        slot.sprite = digitSprites[digit];
    }
}