using System.Collections;
using UnityEngine;

// =============================================
// Result audio sequence / リザルト音声シーケンスクラス
// =============================================
// EN: Lives in the Result scene. Plays a scripted sequence of sounds:
//     1. Plays resultSE first, alone.
//     2. Once resultSE finishes, starts the BGM and characterSE together (BGM ducked lower).
//     3. Once characterSE finishes, plays congratsSE (BGM ducked again, possibly a different amount).
//     4. Once congratsSE finishes, BGM returns to its normal volume for the rest of the scene.
//     Each step's timing is driven by the actual length of its clip, so it stays in sync
//     even if clips are swapped later. Stops the BGM automatically when Result unloads.
//     NOTE: characterSE selection is temporary - since the ball-color tracking system doesn't
//     exist yet, "Test Character Index" lets you manually preview each of the 4 slots. Once the
//     real tracking exists, replace the one line in PlayCharacterAndBGM() that reads it.
// JP: Resultシーンに配置する。以下の順序でサウンドを再生する。
//     1. まずresultSEのみを再生する。
//     2. resultSEが終わったら、BGMとcharacterSEを同時に開始する（BGMは音量を下げる）。
//     3. characterSEが終わったら、congratsSEを再生する（BGMは再度、別の量で音量を下げる）。
//     4. congratsSEが終わったら、BGMはシーンの残り時間、通常の音量に戻る。
//     各ステップのタイミングは、クリップの実際の長さに基づいて制御されるため、
//     後でクリップを差し替えても同期がずれない。Resultがアンロードされる際、自動的にBGMを停止する。
//     ※characterSEの選択は仮のもの - ボールの色を追跡するシステムがまだ存在しないため、
//     「Test Character Index」で4つのスロットを手動でプレビューできるようにしている。
//     実際の追跡システムができたら、PlayCharacterAndBGM()内の該当する1行を置き換えること。
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

    [Header("Step 2 - Character SE (index 0-3 = ball color that scored the most)")]
    [SerializeField] private AudioClip[] characterSounds = new AudioClip[4];
    [SerializeField][Range(0f, 1f)] private float characterSEVolume = 1.0f;

    [Header("TEMPORARY - manually pick which character SE to preview (0-3), until ball tracking exists")]
    [SerializeField] private int testCharacterIndex = 0;

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
        // EN: Step 1 - Result SE alone.
        // JP: ステップ1 - Result SEのみ。
        SoundManager.Instance.PlaySE(resultSE, resultSEVolume);
        if (resultSE != null) yield return new WaitForSecondsRealtime(resultSE.length);

        // EN: Step 2 - Start BGM and Character SE together, BGM ducked.
        // JP: ステップ2 - BGMとCharacter SEを同時に開始する。BGMは音量を下げる。
        SoundManager.Instance.PlayBGM(resultBGM, resultBGMVolumeDuringCharacterSE);

        AudioClip characterSE = GetCharacterSE();
        SoundManager.Instance.PlaySE(characterSE, characterSEVolume);
        if (characterSE != null) yield return new WaitForSecondsRealtime(characterSE.length);

        // EN: Step 3 - Congrats SE, BGM ducked again (possibly a different amount).
        // JP: ステップ3 - Congrats SE。BGMは再度音量を下げる（別の量の場合もある）。
        SoundManager.Instance.SetBGMVolume(resultBGMVolumeDuringCongratsSE);
        SoundManager.Instance.PlaySE(congratsSE, congratsSEVolume);
        if (congratsSE != null) yield return new WaitForSecondsRealtime(congratsSE.length);

        // EN: Step 4 - Back to normal BGM volume for the rest of the scene.
        // JP: ステップ4 - シーンの残り時間、通常のBGM音量に戻る。
        SoundManager.Instance.SetBGMVolume(resultBGMNormalVolume);
    }

    // EN: Gets which character SE to play. TEMPORARY: uses the manually-set test index until
    //     real ball-color tracking exists - replace this line with the real result once it does.
    // JP: どのCharacter SEを再生するかを取得する。仮実装：実際のボールの色の追跡システムが
    //     できるまでは、手動設定のテスト用インデックスを使用する - できたらこの行を置き換える。
    private AudioClip GetCharacterSE()
    {
        if (characterSounds == null || testCharacterIndex < 0 || testCharacterIndex >= characterSounds.Length)
        {
            return null;
        }
        return characterSounds[testCharacterIndex];
    }

    // EN: Stops the BGM when this scene unloads, so it doesn't keep playing into the next scene.
    // JP: このシーンがアンロードされる際にBGMを停止し、次のシーンで鳴り続けないようにする。
    private void OnDestroy()
    {
        SoundManager.Instance.StopBGM();
    }
}