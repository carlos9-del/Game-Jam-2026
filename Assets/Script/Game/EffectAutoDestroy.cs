using UnityEngine;

// アニメーションが1回終わったら自分を消す
public class EffectAutoDestroy : MonoBehaviour
{
    public float lifeTime = 1f;   // 何秒後に消えるか（アニメの長さに合わせる）

    void Start()
    {
        // lifeTime秒後に自動で消える
        Destroy(gameObject, lifeTime);
    }
}