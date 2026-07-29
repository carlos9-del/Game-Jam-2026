using UnityEngine;

// =============================================
// Stage manager / ステージ管理クラス
// =============================================
// EN: Lives in the Stage scene and handles stage-specific setup - starting with BGM.
//     Reads directly from its own assigned StageData, so it works correctly no matter how
//     this scene was entered (through Loading, direct load, or pressing Play from this scene).
//     Future stage-specific logic (pause handling, win/lose, score, etc.) can be added here.
// JP: Stageシーンに配置し、ステージ固有の初期化処理を行う（まずはBGMから）。
//     直接割り当てられたStageDataを読み込むため、このシーンにどう到達したか
//     （Loading経由、直接読み込み、このシーンから直接Playした場合など）に関わらず正しく動作する。
//     今後、ステージ固有の処理（ポーズ処理、勝敗判定、スコアなど）はここに追加していく。
public class StageManager : MonoBehaviour
{
    [Header("This Stage's Data")]
    [SerializeField] private StageData stageData;

    private void Start()
    {
        ResetGameData();
        PlayStageBGM();
    }

    // EN: Resets counters from any previous playthrough, so a new game starts clean.
    // JP: 前回のプレイからのカウンターをリセットし、新しいゲームがまっさらな状態で始まるようにする。
    private void ResetGameData()
    {
        ColorCounter.Instance.ResetAll();
    }

    // EN: Stops the BGM when this scene unloads (going to Result, back to Title, etc.),
    //     so it doesn't keep playing into other scenes. It will play again from the start
    //     next time Stage is entered, since PlayStageBGM() runs fresh in Start().
    // JP: このシーンがアンロードされる際（Resultへ遷移、Titleへ戻るなど）にBGMを停止し、
    //     他のシーンで鳴り続けないようにする。次にStageに入った際は、Start()が
    //     改めて実行されるため、BGMは最初から再生される。
    private void OnDestroy()
    {
        SoundManager.Instance.StopBGM();
    }

    // EN: Plays this stage's BGM, if one is assigned. / JP: このステージのBGMが設定されていれば再生する。
    private void PlayStageBGM()
    {
        if (stageData == null || stageData.bgmClip == null) return;
        SoundManager.Instance.PlayBGM(stageData.bgmClip, stageData.bgmVolume);
    }
}