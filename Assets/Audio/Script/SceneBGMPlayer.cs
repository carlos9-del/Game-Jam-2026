using UnityEngine;

// =============================================
// Scene BGM player / シーンBGM再生クラス
// =============================================
// EN: Attach to an empty object in any scene that should play its own BGM (e.g. Title, Result).
//     Starts playing automatically when the scene loads, and stops automatically when the scene
//     unloads - so it never keeps playing into the next scene.
//     Optionally also plays a one-time SE when the scene starts (e.g. a result fanfare).
//     Optionally, list scenes that share this same BGM without restarting/stopping when moving
//     between them (e.g. Title and Rank keep the same music playing back and forth, only
//     resetting when entering from somewhere outside that group).
// JP: 自分自身のBGMを再生したいシーン（例：Title、Result）の空オブジェクトにアタッチする。
//     シーンが読み込まれると自動的に再生を開始し、シーンがアンロードされると自動的に停止する
//     - そのため、次のシーンにBGMが鳴り続けることはない。
//     任意で、シーン開始時に1回だけ再生するSE（例：リザルトのファンファーレ）も設定できる。
//     任意で、同じBGMを共有し、再起動・停止せずに行き来できるシーンを指定できる
//     （例：TitleとRankは同じ曲を鳴らしたまま行き来し、そのグループ外から入った時のみリセットされる）。
public class SceneBGMPlayer : MonoBehaviour
{
    [Header("BGM Clip To Play On Scene Start")]
    [SerializeField] private AudioClip bgmClip;

    [Header("BGM Volume (0-1)")]
    [SerializeField][Range(0f, 1f)] private float bgmVolume = 1.0f;

    [Header("Optional One-Time SE To Play On Scene Start (e.g. a fanfare)")]
    [SerializeField] private AudioClip enterSE;

    [Header("Enter SE Volume (0-1)")]
    [SerializeField][Range(0f, 1f)] private float enterSEVolume = 1.0f;

    [Header("Scenes That Share This BGM Without Restarting/Stopping (e.g. \"Rank\" on Title's player)")]
    [SerializeField] private string[] continuousWithScenes;

    private void Start()
    {
        // EN: Only start the BGM fresh if it isn't already playing this same clip
        //     (e.g. arriving from a scene in continuousWithScenes, where it was left playing).
        // JP: 既に同じクリップが再生中でない場合のみ、BGMを最初から再生する
        //     （例：continuousWithScenesに含まれるシーンから来た場合、鳴らしたままにしておく）。
        if (!SoundManager.Instance.IsBGMPlaying(bgmClip))
        {
            SoundManager.Instance.PlayBGM(bgmClip, bgmVolume);
        }

        SoundManager.Instance.PlaySE(enterSE, enterSEVolume);
    }

    // EN: Stops the BGM when this scene unloads, unless heading to a scene in continuousWithScenes,
    //     in which case the BGM is left playing so it continues seamlessly there.
    // JP: このシーンがアンロードされる際にBGMを停止する。ただしcontinuousWithScenesに含まれる
    //     シーンへ向かう場合は、BGMを再生したままにして、そのシーンでシームレスに続くようにする。
    private void OnDestroy()
    {
        string nextScene = SceneChangeManager.Instance.GetNextScene();
        bool shouldContinue = continuousWithScenes != null && System.Array.IndexOf(continuousWithScenes, nextScene) >= 0;

        if (!shouldContinue)
        {
            SoundManager.Instance.StopBGM();
        }
    }
}


//using UnityEngine;

//// =============================================
//// Scene BGM player / シーンBGM再生クラス
//// =============================================
//// EN: Attach to an empty object in any scene that should play its own BGM (e.g. Title, Result).
////     Starts playing automatically when the scene loads, and stops automatically when the scene
////     unloads - so it never keeps playing into the next scene.
////     Optionally also plays a one-time SE when the scene starts (e.g. a result fanfare).
//// JP: 自分自身のBGMを再生したいシーン（例：Title、Result）の空オブジェクトにアタッチする。
////     シーンが読み込まれると自動的に再生を開始し、シーンがアンロードされると自動的に停止する
////     - そのため、次のシーンにBGMが鳴り続けることはない。
////     任意で、シーン開始時に1回だけ再生するSE（例：リザルトのファンファーレ）も設定できる。
//public class SceneBGMPlayer : MonoBehaviour
//{
//    [Header("BGM Clip To Play On Scene Start")]
//    [SerializeField] private AudioClip bgmClip;

//    [Header("BGM Volume (0-1)")]
//    [SerializeField][Range(0f, 1f)] private float bgmVolume = 1.0f;

//    [Header("Optional One-Time SE To Play On Scene Start (e.g. a fanfare)")]
//    [SerializeField] private AudioClip enterSE;

//    [Header("Enter SE Volume (0-1)")]
//    [SerializeField][Range(0f, 1f)] private float enterSEVolume = 1.0f;

//    private void Start()
//    {
//        SoundManager.Instance.PlayBGM(bgmClip, bgmVolume);
//        SoundManager.Instance.PlaySE(enterSE, enterSEVolume);
//    }

//    // EN: Stops the BGM when this scene unloads, so it doesn't keep playing into the next scene.
//    // JP: このシーンがアンロードされる際にBGMを停止し、次のシーンで鳴り続けないようにする。
//    private void OnDestroy()
//    {
//        SoundManager.Instance.StopBGM();
//    }
//}


////using UnityEngine;

////// =============================================
////// Scene BGM player / シーンBGM再生クラス
////// =============================================
////// EN: Attach to an empty object in any scene that should play its own BGM (e.g. Title, Result).
//////     Starts playing automatically when the scene loads, and stops automatically when the scene
//////     unloads - so it never keeps playing into the next scene.
////// JP: 自分自身のBGMを再生したいシーン（例：Title、Result）の空オブジェクトにアタッチする。
//////     シーンが読み込まれると自動的に再生を開始し、シーンがアンロードされると自動的に停止する
//////     - そのため、次のシーンにBGMが鳴り続けることはない。
////public class SceneBGMPlayer : MonoBehaviour
////{
////    [Header("BGM Clip To Play On Scene Start")]
////    [SerializeField] private AudioClip bgmClip;

////    [Header("BGM Volume (0-1)")]
////    [SerializeField][Range(0f, 1f)] private float bgmVolume = 1.0f;

////    private void Start()
////    {
////        SoundManager.Instance.PlayBGM(bgmClip, bgmVolume);
////    }

////    // EN: Stops the BGM when this scene unloads, so it doesn't keep playing into the next scene.
////    // JP: このシーンがアンロードされる際にBGMを停止し、次のシーンで鳴り続けないようにする。
////    private void OnDestroy()
////    {
////        SoundManager.Instance.StopBGM();
////    }
////}