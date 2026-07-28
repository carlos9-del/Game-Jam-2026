using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// =============================================
// Countdown timer / カウントダウンタイマークラス
// =============================================
// EN: Lives in the Stage scene. Counts down from a starting number of seconds, displayed using
//     sprite digits (0-9) instead of text. When it reaches 0, transitions to the Result scene.
//     Automatically respects pause, since it waits using WaitForSeconds (affected by Time.timeScale).
//     Special rule: the final second (displaying "1") lasts twice as long as a normal second.
//     When time is low, digits turn red and each number shrinks away to nothing right before
//     the next one appears.
// JP: Stageシーンに配置する。設定した秒数からカウントダウンし、テキストではなくスプライト数字（0-9）で表示する。
//     0に到達すると、Resultシーンへ遷移する。
//     WaitForSecondsを使用しているため（Time.timeScaleの影響を受ける）、ポーズ時は自動的に一時停止する。
//     特別なルール：最後の1秒（「1」が表示されている間）は、通常の2倍の長さになる。
//     残り時間が少なくなると、数字が赤く変わり、次の数字が表示される直前に縮んで消えるようになる。
public class CountdownTimer : MonoBehaviour
{
    [Header("Starting Time (seconds)")]
    [SerializeField] private int startSeconds = 60;

    [Header("Digit Sprites (index 0 = '0', index 9 = '9')")]
    [SerializeField] private Sprite[] digitSprites = new Sprite[10];

    [Header("Hundreds Digit Slot (optional - leave empty if you only need 2 digits)")]
    [SerializeField] private Image hundredsDigitSlot;

    [Header("Digit Display Slots (left = tens, right = ones)")]
    [SerializeField] private Image tensDigitSlot;
    [SerializeField] private Image onesDigitSlot;

    [Header("Digits Container (parent of the digit slots, used for the low-time shrink effect)")]
    [SerializeField] private RectTransform digitsContainer;

    [Header("Timer Inner Fill Image (Image Type must be set to Filled / Radial 360)")]
    [SerializeField] private Image timerFillImage;

    [Header("Time Remaining That Triggers The Low-Time Effect (seconds)")]
    [SerializeField] private int lowTimeThreshold = 10;

    [Header("Digit Scale Multiplier While Time Is Low (starting size before it shrinks away)")]
    [SerializeField] private float lowTimeScale = 1.3f;

    [Header("Digit Color While Time Is Low")]
    [SerializeField] private Color lowTimeColor = Color.red;

    [Header("Sound Effect To Play On Each Low-Time Tick")]
    [SerializeField] private AudioClip lowTimeTickSE;

    [Header("BGM Volume While Time Is Low (0-1, stays lowered until the stage ends)")]
    [SerializeField][Range(0f, 1f)] private float lowTimeBGMVolume = 0.3f;

    private Vector3 digitsDefaultScale;
    private bool isLowTimeColorApplied = false;

    private void Awake()
    {
        if (digitsContainer != null)
        {
            digitsDefaultScale = digitsContainer.localScale;
        }
    }

    private void Start()
    {
        StartCoroutine(CountdownRoutine());
    }

    // EN: Runs the countdown, updating the display once per second, then transitions to Result at 0.
    // JP: カウントダウンを実行し、1秒ごとに表示を更新し、0になったらResultシーンへ遷移する。
    private IEnumerator CountdownRoutine()
    {
        int remaining = startSeconds;
        UpdateDisplay(remaining);

        while (remaining > 0)
        {
            // EN: The final second (remaining == 1) lasts twice as long as normal.
            // JP: 最後の1秒（remaining == 1の時）は、通常の2倍の長さになる。
            float waitTime = (remaining == 1) ? 2.0f : 1.0f;

            // EN: Once time is low, turn digits red (once), play a tick SE, and shrink this number
            //     away from its enlarged size over its display duration.
            // JP: 残り時間が少なくなったら、数字を赤くし（一度だけ）、ティック音を再生し、
            //     この数字を拡大されたサイズから表示時間にかけて縮めて消す。
            if (remaining <= lowTimeThreshold)
            {
                if (!isLowTimeColorApplied)
                {
                    SetDigitsColor(lowTimeColor);
                    SoundManager.Instance.SetBGMVolume(lowTimeBGMVolume);
                    isLowTimeColorApplied = true;
                }

                SoundManager.Instance.PlaySE(lowTimeTickSE);

                if (digitsContainer != null)
                {
                    digitsContainer.localScale = digitsDefaultScale * lowTimeScale; // EN: start enlarged, then shrink away / JP: 拡大した状態から始まり、縮んで消える
                    StartCoroutine(ShrinkRoutine(waitTime));
                }
            }

            yield return new WaitForSeconds(waitTime);

            remaining--;
            UpdateDisplay(remaining);
        }

        SceneChangeManager.Instance.SceneChange("Result");
    }

    // EN: Shrinks the digits container from its enlarged size down to nothing over the given duration.
    // JP: 指定された時間をかけて、数字のコンテナを拡大されたサイズから徐々に縮めて消す。
    private IEnumerator ShrinkRoutine(float duration)
    {
        Vector3 startScale = digitsDefaultScale * lowTimeScale;
        float elapsedTime = 0.0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime; // EN: scaled time, so this pauses correctly along with the countdown / JP: スケールされた時間を使用するため、カウントダウンと同様に一時停止に対応する
            float t = elapsedTime / duration;
            digitsContainer.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            yield return null;
        }
    }

    // EN: Sets the color tint of all active digit slots (used to turn them red at low time).
    // JP: 表示中の全ての桁スロットの色を設定する（残り時間が少ない時に赤くするために使用する）。
    private void SetDigitsColor(Color color)
    {
        if (hundredsDigitSlot != null) hundredsDigitSlot.color = color;
        if (tensDigitSlot != null) tensDigitSlot.color = color;
        if (onesDigitSlot != null) onesDigitSlot.color = color;
    }

    // EN: Updates the digit slots to show the given value (0-999), and updates the radial fill.
    //     Leading zeros are hidden - "9" instead of "09", "99" instead of "099".
    // JP: 指定された値（0～999）を表示するように、桁スロットと放射状フィルを更新する。
    //     先頭の0は非表示になる - "09"ではなく"9"、"099"ではなく"99"と表示される。
    private void UpdateDisplay(int value)
    {
        if (digitSprites == null || digitSprites.Length < 10) return;

        value = Mathf.Clamp(value, 0, 999);
        int hundreds = value / 100;
        int tens = (value / 10) % 10;
        int ones = value % 10;

        bool showHundreds = hundreds > 0;
        bool showTens = showHundreds || tens > 0;

        if (hundredsDigitSlot != null)
        {
            hundredsDigitSlot.gameObject.SetActive(showHundreds);
            if (showHundreds) hundredsDigitSlot.sprite = digitSprites[hundreds];
        }

        if (tensDigitSlot != null)
        {
            // EN: Hide the tens digit too when the whole number is a single digit (e.g. "9", not "09").
            // JP: 数値全体が1桁の場合、十の位も非表示にする（例："09"ではなく"9"）。
            tensDigitSlot.gameObject.SetActive(showTens);
            if (showTens) tensDigitSlot.sprite = digitSprites[tens];
        }

        if (onesDigitSlot != null) onesDigitSlot.sprite = digitSprites[ones];

        // EN: Update the radial fill to match the remaining proportion of time (1.0 = full, 0.0 = empty).
        // JP: 残り時間の割合に合わせて放射状フィルを更新する（1.0 = 満タン、0.0 = 空）。
        if (timerFillImage != null && startSeconds > 0)
        {
            timerFillImage.fillAmount = (float)value / startSeconds;
        }
    }
}

//using System.Collections;
//using UnityEngine;
//using UnityEngine.UI;

//// =============================================
//// Countdown timer / カウントダウンタイマークラス
//// =============================================
//// EN: Lives in the Stage scene. Counts down from a starting number of seconds, displayed using
////     sprite digits (0-9) instead of text. When it reaches 0, transitions to the Result scene.
////     Automatically respects pause, since it waits using WaitForSeconds (affected by Time.timeScale).
////     Special rule: the final second (displaying "1") lasts twice as long as a normal second.
////     When time is low, digits turn red and each number shrinks away to nothing right before
////     the next one appears.
//// JP: Stageシーンに配置する。設定した秒数からカウントダウンし、テキストではなくスプライト数字（0-9）で表示する。
////     0に到達すると、Resultシーンへ遷移する。
////     WaitForSecondsを使用しているため（Time.timeScaleの影響を受ける）、ポーズ時は自動的に一時停止する。
////     特別なルール：最後の1秒（「1」が表示されている間）は、通常の2倍の長さになる。
////     残り時間が少なくなると、数字が赤く変わり、次の数字が表示される直前に縮んで消えるようになる。
//public class CountdownTimer : MonoBehaviour
//{
//    [Header("Starting Time (seconds)")]
//    [SerializeField] private int startSeconds = 60;

//    [Header("Digit Sprites (index 0 = '0', index 9 = '9')")]
//    [SerializeField] private Sprite[] digitSprites = new Sprite[10];

//    [Header("Hundreds Digit Slot (optional - leave empty if you only need 2 digits)")]
//    [SerializeField] private Image hundredsDigitSlot;

//    [Header("Digit Display Slots (left = tens, right = ones)")]
//    [SerializeField] private Image tensDigitSlot;
//    [SerializeField] private Image onesDigitSlot;

//    [Header("Digits Container (parent of the digit slots, used for the low-time shrink effect)")]
//    [SerializeField] private RectTransform digitsContainer;

//    [Header("Timer Inner Fill Image (Image Type must be set to Filled / Radial 360)")]
//    [SerializeField] private Image timerFillImage;

//    [Header("Time Remaining That Triggers The Low-Time Effect (seconds)")]
//    [SerializeField] private int lowTimeThreshold = 10;

//    [Header("Digit Scale Multiplier While Time Is Low (starting size before it shrinks away)")]
//    [SerializeField] private float lowTimeScale = 1.3f;

//    [Header("Digit Color While Time Is Low")]
//    [SerializeField] private Color lowTimeColor = Color.red;

//    [Header("Sound Effect To Play On Each Low-Time Tick")]
//    [SerializeField] private AudioClip lowTimeTickSE;

//    private Vector3 digitsDefaultScale;
//    private bool isLowTimeColorApplied = false;

//    private void Awake()
//    {
//        if (digitsContainer != null)
//        {
//            digitsDefaultScale = digitsContainer.localScale;
//        }
//    }

//    private void Start()
//    {
//        StartCoroutine(CountdownRoutine());
//    }

//    // EN: Runs the countdown, updating the display once per second, then transitions to Result at 0.
//    // JP: カウントダウンを実行し、1秒ごとに表示を更新し、0になったらResultシーンへ遷移する。
//    private IEnumerator CountdownRoutine()
//    {
//        int remaining = startSeconds;
//        UpdateDisplay(remaining);

//        while (remaining > 0)
//        {
//            // EN: The final second (remaining == 1) lasts twice as long as normal.
//            // JP: 最後の1秒（remaining == 1の時）は、通常の2倍の長さになる。
//            float waitTime = (remaining == 1) ? 2.0f : 1.0f;

//            // EN: Once time is low, turn digits red (once), play a tick SE, and shrink this number
//            //     away from its enlarged size over its display duration.
//            // JP: 残り時間が少なくなったら、数字を赤くし（一度だけ）、ティック音を再生し、
//            //     この数字を拡大されたサイズから表示時間にかけて縮めて消す。
//            if (remaining <= lowTimeThreshold)
//            {
//                if (!isLowTimeColorApplied)
//                {
//                    SetDigitsColor(lowTimeColor);
//                    isLowTimeColorApplied = true;
//                }

//                SoundManager.Instance.PlaySE(lowTimeTickSE);

//                if (digitsContainer != null)
//                {
//                    digitsContainer.localScale = digitsDefaultScale * lowTimeScale; // EN: start enlarged, then shrink away / JP: 拡大した状態から始まり、縮んで消える
//                    StartCoroutine(ShrinkRoutine(waitTime));
//                }
//            }

//            yield return new WaitForSeconds(waitTime);

//            remaining--;
//            UpdateDisplay(remaining);
//        }

//        SceneChangeManager.Instance.SceneChange("Result");
//    }

//    // EN: Shrinks the digits container from its enlarged size down to nothing over the given duration.
//    // JP: 指定された時間をかけて、数字のコンテナを拡大されたサイズから徐々に縮めて消す。
//    private IEnumerator ShrinkRoutine(float duration)
//    {
//        Vector3 startScale = digitsDefaultScale * lowTimeScale;
//        float elapsedTime = 0.0f;
//        while (elapsedTime < duration)
//        {
//            elapsedTime += Time.deltaTime; // EN: scaled time, so this pauses correctly along with the countdown / JP: スケールされた時間を使用するため、カウントダウンと同様に一時停止に対応する
//            float t = elapsedTime / duration;
//            digitsContainer.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
//            yield return null;
//        }
//    }

//    // EN: Sets the color tint of all active digit slots (used to turn them red at low time).
//    // JP: 表示中の全ての桁スロットの色を設定する（残り時間が少ない時に赤くするために使用する）。
//    private void SetDigitsColor(Color color)
//    {
//        if (hundredsDigitSlot != null) hundredsDigitSlot.color = color;
//        if (tensDigitSlot != null) tensDigitSlot.color = color;
//        if (onesDigitSlot != null) onesDigitSlot.color = color;
//    }

//    // EN: Updates the digit slots to show the given value (0-999), and updates the radial fill.
//    //     Leading zeros are hidden - "9" instead of "09", "99" instead of "099".
//    // JP: 指定された値（0～999）を表示するように、桁スロットと放射状フィルを更新する。
//    //     先頭の0は非表示になる - "09"ではなく"9"、"099"ではなく"99"と表示される。
//    private void UpdateDisplay(int value)
//    {
//        if (digitSprites == null || digitSprites.Length < 10) return;

//        value = Mathf.Clamp(value, 0, 999);
//        int hundreds = value / 100;
//        int tens = (value / 10) % 10;
//        int ones = value % 10;

//        bool showHundreds = hundreds > 0;
//        bool showTens = showHundreds || tens > 0;

//        if (hundredsDigitSlot != null)
//        {
//            hundredsDigitSlot.gameObject.SetActive(showHundreds);
//            if (showHundreds) hundredsDigitSlot.sprite = digitSprites[hundreds];
//        }

//        if (tensDigitSlot != null)
//        {
//            // EN: Hide the tens digit too when the whole number is a single digit (e.g. "9", not "09").
//            // JP: 数値全体が1桁の場合、十の位も非表示にする（例："09"ではなく"9"）。
//            tensDigitSlot.gameObject.SetActive(showTens);
//            if (showTens) tensDigitSlot.sprite = digitSprites[tens];
//        }

//        if (onesDigitSlot != null) onesDigitSlot.sprite = digitSprites[ones];

//        // EN: Update the radial fill to match the remaining proportion of time (1.0 = full, 0.0 = empty).
//        // JP: 残り時間の割合に合わせて放射状フィルを更新する（1.0 = 満タン、0.0 = 空）。
//        if (timerFillImage != null && startSeconds > 0)
//        {
//            timerFillImage.fillAmount = (float)value / startSeconds;
//        }
//    }
//}