using UnityEngine;

// =============================================
// Stage data definition / ステージデータ定義クラス
// =============================================
// EN: Holds all per-stage information in a single reusable asset.
//     Create one asset per stage (right-click in Project window -> Create -> Stage Data).
//     When new stages are added later, no other script needs to change -
//     just create a new asset and fill in its fields.
// JP: 各ステージ固有の情報を1つのアセットにまとめて保持する。
//     ステージごとに1つのアセットを作成する（Projectウィンドウで右クリック -> Create -> Stage Data）。
//     今後ステージを追加する際も、他のスクリプトを変更する必要はなく、
//     新しいアセットを作成しフィールドを設定するだけでよい。
[CreateAssetMenu(fileName = "New Stage Data", menuName = "Stage Data")]
public class StageData : ScriptableObject
{
    [Header("Scene Name (must match the exact name in Build Settings)")]
    public string sceneName;

    [Header("Display Name (shown on the loading screen)")]
    public string displayName;

    [Header("Loading Screen Background")]
    public Sprite loadingBackground;

    [Header("Stage BGM")]
    public AudioClip bgmClip;

    // EN: Add more per-stage fields here later as needed
    //     (e.g. BGM clip, difficulty, par time, etc.)
    // JP: 今後必要になったステージ固有の項目をここに追加していく
    //     （例：BGMクリップ、難易度、目標タイムなど）
}