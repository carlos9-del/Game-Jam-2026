using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// =============================================
// Loading animation / ローディングアニメーションクラス
// =============================================
// EN: Lives in the Loading scene. Plays through a sprite sheet's frames once, like a simple
//     frame-by-frame animation, on an Image. Exposes when it's finished so SceneLoading can
//     wait for it before transitioning to the real target scene.
// JP: Loadingシーンに配置する。スプライトシートのフレームを、コマ送りアニメーションのように
//     一度だけ再生する。SceneLoadingが実際の目的シーンへ遷移する前に待機できるよう、
//     再生完了のタイミングを公開する。
public class LoadingAnimation : MonoBehaviour
{
    [Header("Image To Display The Animation On")]
    [SerializeField] private Image targetImage;

    [Header("Animation Frames (in playback order)")]
    [SerializeField] private Sprite[] frames;

    [Header("Playback Speed (frames per second)")]
    [SerializeField] private float frameRate = 12.0f;

    // EN: True once the animation has played through all its frames. / JP: 全フレームの再生が終わるとtrueになる。
    public bool IsFinished { get; private set; } = false;

    private void Start()
    {
        StartCoroutine(PlayAnimation());
    }

    // EN: Plays through each frame once, in order, at the given frame rate.
    // JP: 指定したフレームレートで、各フレームを順番に一度だけ再生する。
    private IEnumerator PlayAnimation()
    {
        if (frames == null || frames.Length == 0 || targetImage == null)
        {
            IsFinished = true;
            yield break;
        }

        float frameDuration = 1.0f / frameRate;

        for (int i = 0; i < frames.Length; i++)
        {
            targetImage.sprite = frames[i];
            yield return new WaitForSecondsRealtime(frameDuration);
        }

        IsFinished = true;
    }
}