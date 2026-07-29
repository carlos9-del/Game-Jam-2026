using System.Collections;
using UnityEngine;

// =============================================
// Result audio sequence / リザルト音声シーケンスクラス
// =============================================
// EN: Lives in the Result scene. Plays a scripted sequence of sounds:
//     1. Plays resultSE first, alone.
//     2. Once resultSE finishes, starts the BGM and characterSE together (BGM ducked lower).
//        characterSE is chosen based on which ball color was collected the most (from ColorCounter).
//     3. Once characterSE finishes, plays congratsSE (BGM ducked again, possibly a different amount).
//     4. Once congratsSE finishes, BGM returns to its normal volume for the rest of the scene.
//     Each step's timing is driven by the actual length of its clip, so it stays in sync
//     even if clips are swapped later. Stops the BGM automatically when Result unloads.
// JP: Resultシーンに配置する。以下の順序でサウンドを再生する。
//     1. まずresultSEのみを再生する。
//     2. resultSEが終わったら、BGMとcharacterSEを同時に開始する（BGMは音量を下げる）。
//        characterSEは、最も多く集められたボールの色に応じて選ばれる（ColorCounterから取得）。
//     3. characterSEが終わったら、congratsSEを再生する（BGMは再度、別の量で音量を下げる）。
//     4. congratsSEが終わったら、BGMはシーンの残り時間、通常の音量に戻る。
//     各ステップのタイミングは、クリップの実際の長さに基づいて制御されるため、
//     後でクリップを差し替えても同期がずれない。Resultがアンロードされる際、自動的にBGMを停止する。
public class ResultAudioSequence : MonoBehaviour
{
    [Header("Step 1 - Result SE (plays first, alone)")]
    [SerializeField] private AudioClip resultSE;
    [SerializeField][Range(0f, 1f)] private float resultSEVolume = 1.0f;

    [Header("Step 2 - Result BGM (starts after Result SE finishes)")]
    [SerializeField] private AudioClip resultBGM;
    [SerializeField][Range(0f, 1f)] private float resultBGMNormalVolume = 1.0f;
    [SerializeField][Range(0f, 1f)] private float resultBGMVolumeDuringCharacterSE = 0.3f;
    [SerializeField][Range(0f, 1f)] private float resultBGMVolumeDuringCongratsSE = 0.3f;

    [Header("Step 2 - Character SE (Red, Yellow, Blue, Green - matches ColorCounter's order)")]
    [SerializeField] private AudioClip redCharacterSE;
    [SerializeField] private AudioClip yellowCharacterSE;
    [SerializeField] private AudioClip blueCharacterSE;
    [SerializeField] private AudioClip greenCharacterSE;
    [SerializeField][Range(0f, 1f)] private float characterSEVolume = 1.0f;

    [Header("Step 2 - Character Images (one per color, shown when that color wins - hidden by default)")]
    [SerializeField] private UnityEngine.UI.Image redCharacterImage;
    [SerializeField] private UnityEngine.UI.Image yellowCharacterImage;
    [SerializeField] private UnityEngine.UI.Image blueCharacterImage;
    [SerializeField] private UnityEngine.UI.Image greenCharacterImage;

    [Header("Step 3 - Congrats SE (plays after Character SE finishes)")]
    [SerializeField] private AudioClip congratsSE;
    [SerializeField][Range(0f, 1f)] private float congratsSEVolume = 1.0f;

    private void Start()
    {
        StartCoroutine(PlaySequence());
    }

    // EN: Runs the full sequence: Result SE -> BGM + Character SE (ducked) -> Congrats SE (ducked) -> normal BGM.
    // JP: シーケンス全体を実行する：Result SE -> BGM + Character SE（音量ダウン） -> Congrats SE（音量ダウン） -> 通常のBGM。
    private IEnumerator PlaySequence()
    {
        // EN: Hide all character images from the very start, so none show during Result SE.
        // JP: 開始直後から全てのキャラクター画像を非表示にし、Result SE再生中は表示されないようにする。
        SetImageActive(redCharacterImage, false);
        SetImageActive(yellowCharacterImage, false);
        SetImageActive(blueCharacterImage, false);
        SetImageActive(greenCharacterImage, false);

        // EN: Step 1 - Result SE alone.
        // JP: ステップ1 - Result SEのみ。
        SoundManager.Instance.PlaySE(resultSE, resultSEVolume);
        if (resultSE != null) yield return new WaitForSecondsRealtime(resultSE.length);

        // EN: Step 2 - Start BGM and Character SE(s) together, BGM ducked.
        //     If multiple colors are tied for the highest count, all of their SE play together.
        // JP: ステップ2 - BGMとCharacter SEを同時に開始する。BGMは音量を下げる。
        //     複数の色が最多タイの場合、それらのSEをすべて同時に再生する。
        SoundManager.Instance.PlayBGM(resultBGM, resultBGMVolumeDuringCharacterSE);

        float characterSEWaitTime = PlayMostCollectedCharacterSounds();
        if (characterSEWaitTime > 0) yield return new WaitForSecondsRealtime(characterSEWaitTime);

        // EN: Step 3 - Congrats SE, BGM ducked again (possibly a different amount).
        // JP: ステップ3 - Congrats SE。BGMは再度音量を下げる（別の量の場合もある）。
        SoundManager.Instance.SetBGMVolume(resultBGMVolumeDuringCongratsSE);
        SoundManager.Instance.PlaySE(congratsSE, congratsSEVolume);
        if (congratsSE != null) yield return new WaitForSecondsRealtime(congratsSE.length);

        // EN: Step 4 - Back to normal BGM volume for the rest of the scene.
        // JP: ステップ4 - シーンの残り時間、通常のBGM音量に戻る。
        SoundManager.Instance.SetBGMVolume(resultBGMNormalVolume);
    }

    // EN: Plays the character SE for every color tied for the highest count (all at once if
    //     there's a tie), and returns the longest clip's length, to know how long to wait.
    // JP: 最も多く集められた色（複数タイの場合は全て）のcharacter SEを再生し（同点の場合は同時に再生）、
    //     待機時間を判断するために、その中で最も長いクリップの長さを返す。
    private float PlayMostCollectedCharacterSounds()
    {
        int redCount = ColorCounter.Instance.GetCount(BallColorType.Red);
        int yellowCount = ColorCounter.Instance.GetCount(BallColorType.Yellow);
        int blueCount = ColorCounter.Instance.GetCount(BallColorType.Blue);
        int greenCount = ColorCounter.Instance.GetCount(BallColorType.Green);

        int highestCount = Mathf.Max(redCount, yellowCount, blueCount, greenCount);

        // EN: Hide all character images first, then only show the winning one(s) below.
        // JP: まず全てのキャラクター画像を非表示にし、その後、勝った色の画像のみ表示する。
        SetImageActive(redCharacterImage, false);
        SetImageActive(yellowCharacterImage, false);
        SetImageActive(blueCharacterImage, false);
        SetImageActive(greenCharacterImage, false);

        System.Collections.Generic.List<string> winningColors = new System.Collections.Generic.List<string>();
        float longestLength = 0f;

        if (redCount == highestCount) longestLength = Mathf.Max(longestLength, PlayCharacterSE(redCharacterSE, redCharacterImage, "Red", winningColors));
        if (yellowCount == highestCount) longestLength = Mathf.Max(longestLength, PlayCharacterSE(yellowCharacterSE, yellowCharacterImage, "Yellow", winningColors));
        if (blueCount == highestCount) longestLength = Mathf.Max(longestLength, PlayCharacterSE(blueCharacterSE, blueCharacterImage, "Blue", winningColors));
        if (greenCount == highestCount) longestLength = Mathf.Max(longestLength, PlayCharacterSE(greenCharacterSE, greenCharacterImage, "Green", winningColors));

        Debug.Log("Ball counts - Red: " + redCount + ", Yellow: " + yellowCount +
            ", Blue: " + blueCount + ", Green: " + greenCount +
            " | Winning color(s): " + string.Join(", ", winningColors));

        return longestLength;
    }

    // EN: Plays one character SE, shows its matching character image, and records its color
    //     name - returns the clip's length (0 if no clip).
    // JP: 1つのcharacter SEを再生し、対応するキャラクター画像を表示し、色の名前を記録する。
    //     クリップの長さを返す（クリップが無ければ0）。
    private float PlayCharacterSE(AudioClip clip, UnityEngine.UI.Image characterImage, string colorName, System.Collections.Generic.List<string> winningColors)
    {
        winningColors.Add(colorName);
        SetImageActive(characterImage, true);
        if (clip == null) return 0f;
        SoundManager.Instance.PlaySE(clip, characterSEVolume);
        return clip.length;
    }

    // EN: Shows or hides a character image, safely skipping if the reference wasn't assigned.
    // JP: キャラクター画像を表示・非表示にする。参照が設定されていない場合は安全にスキップする。
    private void SetImageActive(UnityEngine.UI.Image image, bool active)
    {
        if (image == null) return;
        image.gameObject.SetActive(active);
    }

    // EN: Stops the BGM and any playing SE when this scene unloads, so nothing keeps
    //     playing into the next scene.
    // JP: このシーンがアンロードされる際にBGMと再生中のSEを停止し、
    //     次のシーンで鳴り続けないようにする。
    private void OnDestroy()
    {
        SoundManager.Instance.StopBGM();
        SoundManager.Instance.StopSE();
    }
}


//using System.Collections;
//using UnityEngine;

//// =============================================
//// Result audio sequence / リザルト音声シーケンスクラス
//// =============================================
//// EN: Lives in the Result scene. Plays a scripted sequence of sounds:
////     1. Plays resultSE first, alone.
////     2. Once resultSE finishes, starts the BGM and characterSE together (BGM ducked lower).
////        characterSE is chosen based on which ball color was collected the most (from ColorCounter).
////     3. Once characterSE finishes, plays congratsSE (BGM ducked again, possibly a different amount).
////     4. Once congratsSE finishes, BGM returns to its normal volume for the rest of the scene.
////     Each step's timing is driven by the actual length of its clip, so it stays in sync
////     even if clips are swapped later. Stops the BGM automatically when Result unloads.
//// JP: Resultシーンに配置する。以下の順序でサウンドを再生する。
////     1. まずresultSEのみを再生する。
////     2. resultSEが終わったら、BGMとcharacterSEを同時に開始する（BGMは音量を下げる）。
////        characterSEは、最も多く集められたボールの色に応じて選ばれる（ColorCounterから取得）。
////     3. characterSEが終わったら、congratsSEを再生する（BGMは再度、別の量で音量を下げる）。
////     4. congratsSEが終わったら、BGMはシーンの残り時間、通常の音量に戻る。
////     各ステップのタイミングは、クリップの実際の長さに基づいて制御されるため、
////     後でクリップを差し替えても同期がずれない。Resultがアンロードされる際、自動的にBGMを停止する。
//public class ResultAudioSequence : MonoBehaviour
//{
//    [Header("Step 1 - Result SE (plays first, alone)")]
//    [SerializeField] private AudioClip resultSE;
//    [SerializeField][Range(0f, 1f)] private float resultSEVolume = 1.0f;

//    [Header("Step 2 - Result BGM (starts after Result SE finishes)")]
//    [SerializeField] private AudioClip resultBGM;
//    [SerializeField][Range(0f, 1f)] private float resultBGMNormalVolume = 1.0f;
//    [SerializeField][Range(0f, 1f)] private float resultBGMVolumeDuringCharacterSE = 0.3f;
//    [SerializeField][Range(0f, 1f)] private float resultBGMVolumeDuringCongratsSE = 0.3f;

//    [Header("Step 2 - Character SE (Red, Yellow, Blue, Green - matches ColorCounter's order)")]
//    [SerializeField] private AudioClip redCharacterSE;
//    [SerializeField] private AudioClip yellowCharacterSE;
//    [SerializeField] private AudioClip blueCharacterSE;
//    [SerializeField] private AudioClip greenCharacterSE;
//    [SerializeField][Range(0f, 1f)] private float characterSEVolume = 1.0f;

//    [Header("Step 3 - Congrats SE (plays after Character SE finishes)")]
//    [SerializeField] private AudioClip congratsSE;
//    [SerializeField][Range(0f, 1f)] private float congratsSEVolume = 1.0f;

//    private void Start()
//    {
//        StartCoroutine(PlaySequence());
//    }

//    // EN: Runs the full sequence: Result SE -> BGM + Character SE (ducked) -> Congrats SE (ducked) -> normal BGM.
//    // JP: シーケンス全体を実行する：Result SE -> BGM + Character SE（音量ダウン） -> Congrats SE（音量ダウン） -> 通常のBGM。
//    private IEnumerator PlaySequence()
//    {
//        // EN: Step 1 - Result SE alone.
//        // JP: ステップ1 - Result SEのみ。
//        SoundManager.Instance.PlaySE(resultSE, resultSEVolume);
//        if (resultSE != null) yield return new WaitForSecondsRealtime(resultSE.length);

//        // EN: Step 2 - Start BGM and Character SE(s) together, BGM ducked.
//        //     If multiple colors are tied for the highest count, all of their SE play together.
//        // JP: ステップ2 - BGMとCharacter SEを同時に開始する。BGMは音量を下げる。
//        //     複数の色が最多タイの場合、それらのSEをすべて同時に再生する。
//        SoundManager.Instance.PlayBGM(resultBGM, resultBGMVolumeDuringCharacterSE);

//        float characterSEWaitTime = PlayMostCollectedCharacterSounds();
//        if (characterSEWaitTime > 0) yield return new WaitForSecondsRealtime(characterSEWaitTime);

//        // EN: Step 3 - Congrats SE, BGM ducked again (possibly a different amount).
//        // JP: ステップ3 - Congrats SE。BGMは再度音量を下げる（別の量の場合もある）。
//        SoundManager.Instance.SetBGMVolume(resultBGMVolumeDuringCongratsSE);
//        SoundManager.Instance.PlaySE(congratsSE, congratsSEVolume);
//        if (congratsSE != null) yield return new WaitForSecondsRealtime(congratsSE.length);

//        // EN: Step 4 - Back to normal BGM volume for the rest of the scene.
//        // JP: ステップ4 - シーンの残り時間、通常のBGM音量に戻る。
//        SoundManager.Instance.SetBGMVolume(resultBGMNormalVolume);
//    }

//    // EN: Plays the character SE for every color tied for the highest count (all at once if
//    //     there's a tie), and returns the longest clip's length, to know how long to wait.
//    // JP: 最も多く集められた色（複数タイの場合は全て）のcharacter SEを再生し（同点の場合は同時に再生）、
//    //     待機時間を判断するために、その中で最も長いクリップの長さを返す。
//    private float PlayMostCollectedCharacterSounds()
//    {
//        int redCount = ColorCounter.Instance.GetCount(BallColorType.Red);
//        int yellowCount = ColorCounter.Instance.GetCount(BallColorType.Yellow);
//        int blueCount = ColorCounter.Instance.GetCount(BallColorType.Blue);
//        int greenCount = ColorCounter.Instance.GetCount(BallColorType.Green);

//        int highestCount = Mathf.Max(redCount, yellowCount, blueCount, greenCount);

//        System.Collections.Generic.List<string> winningColors = new System.Collections.Generic.List<string>();
//        float longestLength = 0f;

//        if (redCount == highestCount) longestLength = Mathf.Max(longestLength, PlayCharacterSE(redCharacterSE, "Red", winningColors));
//        if (yellowCount == highestCount) longestLength = Mathf.Max(longestLength, PlayCharacterSE(yellowCharacterSE, "Yellow", winningColors));
//        if (blueCount == highestCount) longestLength = Mathf.Max(longestLength, PlayCharacterSE(blueCharacterSE, "Blue", winningColors));
//        if (greenCount == highestCount) longestLength = Mathf.Max(longestLength, PlayCharacterSE(greenCharacterSE, "Green", winningColors));

//        Debug.Log("Ball counts - Red: " + redCount + ", Yellow: " + yellowCount +
//            ", Blue: " + blueCount + ", Green: " + greenCount +
//            " | Winning color(s): " + string.Join(", ", winningColors));

//        return longestLength;
//    }

//    // EN: Plays one character SE and records its color name, returning its length (0 if no clip).
//    // JP: 1つのcharacter SEを再生し、その色の名前を記録する。長さを返す（クリップが無ければ0）。
//    private float PlayCharacterSE(AudioClip clip, string colorName, System.Collections.Generic.List<string> winningColors)
//    {
//        winningColors.Add(colorName);
//        if (clip == null) return 0f;
//        SoundManager.Instance.PlaySE(clip, characterSEVolume);
//        return clip.length;
//    }

//    // EN: Stops the BGM and any playing SE when this scene unloads, so nothing keeps
//    //     playing into the next scene.
//    // JP: このシーンがアンロードされる際にBGMと再生中のSEを停止し、
//    //     次のシーンで鳴り続けないようにする。
//    private void OnDestroy()
//    {
//        SoundManager.Instance.StopBGM();
//        SoundManager.Instance.StopSE();
//    }
//}