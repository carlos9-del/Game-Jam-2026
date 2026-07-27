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
//     When time is low, the digits grow larger and shake briefly on each tick for tension.
// JP: Stageシーンに配置する。設定した秒数からカウントダウンし、テキストではなくスプライト数字（0-9）で表示する。
//     0に到達すると、Resultシーンへ遷移する。
//     WaitForSecondsを使用しているため（Time.timeScaleの影響を受ける）、ポーズ時は自動的に一時停止する。
//     特別なルール：最後の1秒（「1」が表示されている間）は、通常の2倍の長さになる。
//     残り時間が少なくなると、緊迫感を出すために数字が大きくなり、切り替わるたびに軽く揺れる。
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

    [Header("Digits Container (parent of the digit slots, used for the low-time scale/shake effect)")]
    [SerializeField] private RectTransform digitsContainer;

    [Header("Time Remaining That Triggers The Low-Time Effect (seconds)")]
    [SerializeField] private int lowTimeThreshold = 10;

    [Header("Digit Scale Multiplier While Time Is Low")]
    [SerializeField] private float lowTimeScale = 1.3f;

    [Header("Shake Strength (pixels)")]
    [SerializeField] private float shakeStrength = 8.0f;

    [Header("Shake Duration Per Tick (seconds)")]
    [SerializeField] private float shakeDuration = 0.2f;

    private Vector2 digitsDefaultPosition;
    private Vector3 digitsDefaultScale;

    private void Awake()
    {
        if (digitsContainer != null)
        {
            digitsDefaultPosition = digitsContainer.anchoredPosition;
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
            yield return new WaitForSeconds(waitTime);

            remaining--;
            UpdateDisplay(remaining);

            // EN: Trigger the tension effect once time is low.
            // JP: 残り時間が少なくなったら、緊迫感を出す演出を発動する。
            if (remaining > 0 && remaining <= lowTimeThreshold && digitsContainer != null)
            {
                StartCoroutine(ShakeRoutine()); 
            }
        }

        SceneChangeManager.Instance.SceneChange("Result");
    }

    // EN: Briefly shakes and enlarges the digits container - called on each tick while time is low.
    // JP: 数字をまとめたコンテナを一時的に拡大・振動させる - 残り時間が少ない間、毎ティック呼び出される。
    private IEnumerator ShakeRoutine()
    {
        digitsContainer.localScale = digitsDefaultScale * lowTimeScale;

        float elapsedTime = 0.0f;
        while (elapsedTime < shakeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime; // EN: use unscaled time so the shake still plays even if timeScale is briefly 0 / JP: timeScaleが0の場合でも揺れが再生されるよう、unscaledDeltaTimeを使用する
            Vector2 shakeOffset = Random.insideUnitCircle * shakeStrength;
            digitsContainer.anchoredPosition = digitsDefaultPosition + shakeOffset;
            yield return null;
        }

        digitsContainer.anchoredPosition = digitsDefaultPosition;
    }

    // EN: Updates the digit slots to show the given value (0-999).
    //     Leading zeros are hidden - "9" instead of "09", "99" instead of "099".
    // JP: 指定された値（0～999）を表示するように、桁スロットを更新する。
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
//// JP: Stageシーンに配置する。設定した秒数からカウントダウンし、テキストではなくスプライト数字（0-9）で表示する。
////     0に到達すると、Resultシーンへ遷移する。
////     WaitForSecondsを使用しているため（Time.timeScaleの影響を受ける）、ポーズ時は自動的に一時停止する。
////     特別なルール：最後の1秒（「1」が表示されている間）は、通常の2倍の長さになる。
//public class CountdownTimer : MonoBehaviour
//{
//    [Header("Starting Time (seconds)")]
//    [SerializeField] private int startSeconds = 60;

//    [Header("Digit Sprites (index 0 = '0', index 9 = '9')")]
//    [SerializeField] private Sprite[] digitSprites = new Sprite[10];

//    [Header("Digit Display Slots (left = tens, right = ones)")]
//    [SerializeField] private Image tensDigitSlot;
//    [SerializeField] private Image onesDigitSlot;

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
//            yield return new WaitForSeconds(waitTime);

//            remaining--;
//            UpdateDisplay(remaining);
//        }

//        SceneChangeManager.Instance.SceneChange("Result");
//    }

//    // EN: Updates the two digit slots to show the given value (00-99).
//    // JP: 指定された値（00～99）を表示するように、2つの桁スロットを更新する。
//    private void UpdateDisplay(int value)
//    {
//        if (digitSprites == null || digitSprites.Length < 10) return;

//        value = Mathf.Clamp(value, 0, 99);
//        int tens = value / 10;
//        int ones = value % 10;

//        if (tensDigitSlot != null) tensDigitSlot.sprite = digitSprites[tens];
//        if (onesDigitSlot != null) onesDigitSlot.sprite = digitSprites[ones];
//    }
//}