using UnityEngine;

// =============================================
// Sound management class / サウンド管理クラス
// =============================================
// EN: Manages and plays sound effects (SE) and background music (BGM) for the whole game.
//     Clips are passed in directly (drag-and-drop AudioClip references), not loaded by name.
//     No volume settings / mixer routing - this is intentionally simple for now.
// JP: ゲーム全体の効果音（SE）とBGMの再生を管理する。
//     クリップは名前で読み込むのではなく、直接参照（ドラッグ＆ドロップ）で渡される。
//     音量設定・ミキサーは今回は意図的に省略している。
public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    private AudioSource seAudioSource;
    private AudioSource bgmAudioSource;

    protected override void Init()
    {
        seAudioSource = gameObject.AddComponent<AudioSource>();
        bgmAudioSource = gameObject.AddComponent<AudioSource>();

        bgmAudioSource.loop = true; // EN: BGM should loop continuously / JP: BGMはループ再生させる
    }

    // EN: Plays a one-shot sound effect at the given volume (0-1). Volume defaults to 1 if not specified.
    // JP: 指定した音量（0～1）でSEを1回再生する。省略した場合は1（デフォルト）になる。
    public void PlaySE(AudioClip clip, float volume = 1.0f)
    {
        if (clip == null) return; // EN: nothing assigned, skip silently / JP: クリップが未設定の場合は何もしない
        seAudioSource.PlayOneShot(clip, volume);
    }

    // EN: Plays background music at the given volume (0-1), replacing any currently playing BGM.
    //     Volume defaults to 1 if not specified.
    // JP: 指定した音量（0～1）でBGMを再生する。現在再生中のBGMは停止・置き換えられる。
    //     省略した場合は1（デフォルト）になる。
    public void PlayBGM(AudioClip clip, float volume = 1.0f)
    {
        if (clip == null) return; // EN: nothing assigned, skip silently / JP: クリップが未設定の場合は何もしない
        bgmAudioSource.clip = clip;
        bgmAudioSource.volume = volume;
        bgmAudioSource.Play();
    }

    // EN: Stops the currently playing BGM. / JP: 現在再生中のBGMを停止する。
    public void StopBGM()
    {
        bgmAudioSource.Stop();
    }

    // EN: Pauses the currently playing BGM without resetting its playback position. Used when the game is paused.
    // JP: 再生位置を保持したままBGMを一時停止する。ゲームがポーズされた際に使用する。
    public void PauseBGM()
    {
        bgmAudioSource.Pause();
    }

    // EN: Resumes BGM from where it was paused. Used when the game is unpaused.
    // JP: 一時停止した位置からBGMを再開する。ポーズ解除時に使用する。
    public void ResumeBGM()
    {
        bgmAudioSource.UnPause();
    }
}