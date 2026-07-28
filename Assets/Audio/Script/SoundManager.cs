using System.Collections;
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

    // EN: Temporarily lowers the BGM volume, then restores it after the given duration.
    //     Useful for briefly making an SE stand out without permanently changing BGM volume.
    // JP: BGMの音量を一時的に下げ、指定した時間の後に元の音量に戻す。
    //     SEを一時的に目立たせたい時に、BGMの音量を恒久的に変更することなく使用できる。
    public void DuckBGM(float duckedVolume, float duration)
    {
        StartCoroutine(DuckBGMRoutine(duckedVolume, duration));
    }

    private IEnumerator DuckBGMRoutine(float duckedVolume, float duration)
    {
        float originalVolume = bgmAudioSource.volume;
        bgmAudioSource.volume = duckedVolume;
        yield return new WaitForSeconds(duration);
        bgmAudioSource.volume = originalVolume;
    }

    // EN: Directly sets the BGM's volume, with no automatic restore - stays at this volume
    //     until changed again. Useful for a lasting volume change (e.g. lowered for the rest
    //     of the stage once time is low), as opposed to DuckBGM's brief, self-restoring dip.
    // JP: BGMの音量を直接設定する。自動的な復元は行われず、再度変更するまでこの音量のままになる。
    //     恒久的な音量変更（例：残り時間が少なくなってからステージ終了まで下げたままにする）に使用する。
    //     DuckBGMの短時間で自動的に元に戻る効果とは異なる。
    public void SetBGMVolume(float volume)
    {
        bgmAudioSource.volume = volume;
    }
}


//using UnityEngine;

//// =============================================
//// Sound management class / サウンド管理クラス
//// =============================================
//// EN: Manages and plays sound effects (SE) and background music (BGM) for the whole game.
////     Clips are passed in directly (drag-and-drop AudioClip references), not loaded by name.
////     No volume settings / mixer routing - this is intentionally simple for now.
//// JP: ゲーム全体の効果音（SE）とBGMの再生を管理する。
////     クリップは名前で読み込むのではなく、直接参照（ドラッグ＆ドロップ）で渡される。
////     音量設定・ミキサーは今回は意図的に省略している。
//public class SoundManager : SingletonMonoBehaviour<SoundManager>
//{
//    private AudioSource seAudioSource;
//    private AudioSource bgmAudioSource;

//    protected override void Init()
//    {
//        seAudioSource = gameObject.AddComponent<AudioSource>();
//        bgmAudioSource = gameObject.AddComponent<AudioSource>();

//        bgmAudioSource.loop = true; // EN: BGM should loop continuously / JP: BGMはループ再生させる
//    }

//    // EN: Plays a one-shot sound effect at the given volume (0-1). Volume defaults to 1 if not specified.
//    // JP: 指定した音量（0～1）でSEを1回再生する。省略した場合は1（デフォルト）になる。
//    public void PlaySE(AudioClip clip, float volume = 1.0f)
//    {
//        if (clip == null) return; // EN: nothing assigned, skip silently / JP: クリップが未設定の場合は何もしない
//        seAudioSource.PlayOneShot(clip, volume);
//    }

//    // EN: Plays background music at the given volume (0-1), replacing any currently playing BGM.
//    //     Volume defaults to 1 if not specified.
//    // JP: 指定した音量（0～1）でBGMを再生する。現在再生中のBGMは停止・置き換えられる。
//    //     省略した場合は1（デフォルト）になる。
//    public void PlayBGM(AudioClip clip, float volume = 1.0f)
//    {
//        if (clip == null) return; // EN: nothing assigned, skip silently / JP: クリップが未設定の場合は何もしない
//        bgmAudioSource.clip = clip;
//        bgmAudioSource.volume = volume;
//        bgmAudioSource.Play();
//    }

//    // EN: Stops the currently playing BGM. / JP: 現在再生中のBGMを停止する。
//    public void StopBGM()
//    {
//        bgmAudioSource.Stop();
//    }

//    // EN: Pauses the currently playing BGM without resetting its playback position. Used when the game is paused.
//    // JP: 再生位置を保持したままBGMを一時停止する。ゲームがポーズされた際に使用する。
//    public void PauseBGM()
//    {
//        bgmAudioSource.Pause();
//    }

//    // EN: Resumes BGM from where it was paused. Used when the game is unpaused.
//    // JP: 一時停止した位置からBGMを再開する。ポーズ解除時に使用する。
//    public void ResumeBGM()
//    {
//        bgmAudioSource.UnPause();
//    }
//}