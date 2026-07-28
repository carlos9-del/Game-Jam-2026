using UnityEngine;

// =============================================
// Scene BGM player / シーンBGM再生クラス
// =============================================
// EN: Attach to an empty object in any scene that should play its own BGM (e.g. Title, Result).
//     Starts playing automatically when the scene loads, and stops automatically when the scene
//     unloads - so it never keeps playing into the next scene.
// JP: 自分自身のBGMを再生したいシーン（例：Title、Result）の空オブジェクトにアタッチする。
//     シーンが読み込まれると自動的に再生を開始し、シーンがアンロードされると自動的に停止する
//     - そのため、次のシーンにBGMが鳴り続けることはない。
public class SceneBGMPlayer : MonoBehaviour
{
    [Header("BGM Clip To Play On Scene Start")]
    [SerializeField] private AudioClip bgmClip;

    [Header("BGM Volume (0-1)")]
    [SerializeField][Range(0f, 1f)] private float bgmVolume = 1.0f;

    private void Start()
    {
        SoundManager.Instance.PlayBGM(bgmClip, bgmVolume);
    }

    // EN: Stops the BGM when this scene unloads, so it doesn't keep playing into the next scene.
    // JP: このシーンがアンロードされる際にBGMを停止し、次のシーンで鳴り続けないようにする。
    private void OnDestroy()
    {
        SoundManager.Instance.StopBGM();
    }
}