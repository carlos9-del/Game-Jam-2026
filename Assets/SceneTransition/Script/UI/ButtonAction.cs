using UnityEngine;
using UnityEngine.EventSystems;

// =============================================
// Button action class / ボタン動作クラス
// =============================================
// EN: Handles basic hover/click feedback (scale changes) and click SE for any UI button.
//     Other scripts (like SceneChangeButton) assign their own logic to the onClick delegate
//     to define what actually happens when the button is clicked.
//
//     Example of assigning click behavior:
//     buttonAction.onClick = () => { /* logic to run on click */ };
//
// JP: 任意のUIボタンに対して、ホバー・クリック時の基本的な演出（拡大縮小）とクリックSEを行う。
//     SceneChangeButtonなど、他のスクリプトがonClickデリゲートに独自の処理を登録することで、
//     ボタンがクリックされた際の実際の動作を定義する。
//
//     クリック時の処理を登録する例：
//     buttonAction.onClick = () => { /* クリック時に実行する処理 */ };
public class ButtonAction : MonoBehaviour,
    IPointerClickHandler,
    IPointerDownHandler,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerUpHandler
{
    private Vector2 defaultScale; // EN: the button's original scale / JP: ボタンの元の大きさ

    [Header("Scale Multiplier While Hovering")]
    [SerializeField] private float hoverScale = 1.15f;

    [Header("Scale Multiplier While Held Down")]
    [SerializeField] private float holdScale = 1.08f;

    [Header("Sound Effect To Play On Click")]
    [SerializeField] private AudioClip clickSE;

    // EN: Assign a method here to define what happens when this button is clicked.
    // JP: このボタンがクリックされた時の処理を定義するため、外部から代入する。
    public System.Action onClick;

    private void Awake()
    {
        defaultScale = transform.localScale; // EN: cache original scale / JP: ボタンの元のサイズを保存
    }

    // EN: Called the moment the button is clicked. / JP: ボタンがクリックされた時に呼び出される。
    public void OnPointerClick(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySE(clickSE);
        onClick?.Invoke();
    }

    // EN: Called the instant the pointer is pressed down on the button. / JP: ボタンがクリックされた瞬間に呼び出される。
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale = defaultScale * holdScale;
    }

    // EN: Called while the pointer hovers over the button. / JP: ボタンの上にカーソルがある時に呼び出される。
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = defaultScale * hoverScale;
    }

    // EN: Called the instant the pointer leaves the button. / JP: ボタンの上からカーソルが離れた瞬間に呼び出される。
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = defaultScale;
    }

    // EN: Called the instant the pointer is released over the button. / JP: ボタンからクリックを離した瞬間に呼び出される。
    public void OnPointerUp(PointerEventData eventData)
    {
        transform.localScale = defaultScale * hoverScale;
    }
}