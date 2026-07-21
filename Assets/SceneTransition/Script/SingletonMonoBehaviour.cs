using UnityEngine;

// =============================================
// Singleton base class / シングルトン基底クラス
// =============================================
// EN: Generic base class that makes any MonoBehaviour subclass into a singleton.
//     Ensures only one instance exists across all scenes, and survives scene loads.
// JP: 任意のMonoBehaviourをシングルトン化するための汎用基底クラス。
//     シーンをまたいでも唯一のインスタンスが存在するように保証する。
public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    // EN: Access point for the single instance. Creates one automatically if none exists.
    // JP: 唯一のインスタンスにアクセスするためのプロパティ。存在しない場合は自動生成する。
    public static T Instance
    {
        get
        {
            // EN: If no instance reference is cached yet, try to find one in the scene.
            // JP: インスタンスがまだ存在しない場合、シーン内から既存のインスタンスを探す。
            if (_instance == null)
            {
                // EN: FindFirstObjectByType is the Unity 6 replacement for the deprecated FindObjectOfType.
                // JP: FindFirstObjectByTypeは、Unity 6で非推奨になったFindObjectOfTypeの代替メソッド。
                _instance = FindFirstObjectByType<T>();

                // EN: Still not found - create a new GameObject and attach the component.
                // JP: それでも見つからない場合、新しいGameObjectを作成しT型のコンポーネントを追加する。
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(T).Name);
                    _instance = singletonObject.AddComponent<T>();
                }
            }
            return _instance;
        }
    }

    // EN: Standard singleton setup - destroy duplicates, keep this one alive across scenes.
    // JP: 標準的なシングルトンの初期化処理。重複したインスタンスを破棄し、シーンをまたいで保持する。
    protected virtual void Awake()
    {
        // EN: If an instance already exists and it's not this one, destroy this duplicate.
        // JP: 既に別のインスタンスが存在する場合、この重複したインスタンスを破棄する。
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // EN: Register this as the one and only instance.
        // JP: このインスタンスを唯一のものとして設定する。
        _instance = this as T;

        // EN: DontDestroyOnLoad only works on root-level objects. Detaching here lets this object
        //     be nested under an organizational parent (e.g. "Managers") in the Editor for tidiness,
        //     while still working correctly at runtime.
        // JP: DontDestroyOnLoadはルートレベルのオブジェクトにのみ有効。ここで親から切り離すことで、
        //     エディタ上では整理用の親オブジェクト（例："Managers"）の子として配置しつつ、
        //     実行時には正しく動作するようにする。
        transform.SetParent(null);

        // EN: Prevent this GameObject from being destroyed when a new scene loads.
        // JP: シーンが切り替わっても破棄されないように設定する。
        DontDestroyOnLoad(gameObject);

        // EN: Hook for subclasses to run their own initialization logic.
        // JP: 子クラスで初期化処理を記述できるようにするための仮想メソッド。
        Init();
    }

    // EN: Override in subclasses to add custom initialization logic.
    // JP: 初期化処理をオーバーライドできるように定義された仮想メソッド。
    protected virtual void Init()
    {
        // EN: Subclasses implement their setup here.
        // JP: 子クラスで初期化処理を記述する。
    }
}