using UnityEngine;

// プレイヤーを追従するカメラ（デッドゾーン + 境界制限つき）
public class CameraFollow : MonoBehaviour
{
    [Header("追従対象")]
    [SerializeField] private Transform target;   // プレイヤー

    [Header("追従の滑らかさ")]
    [SerializeField] private float smoothTime = 0.2f;  // 小さいほど速く追いつく

    [Header("デッドゾーン（この範囲内なら追わない）")]
    [SerializeField] private Vector2 deadZoneSize = new Vector2(2f, 1.5f);

    [Header("境界制限")]
    [SerializeField] private bool useBounds = true;
    [SerializeField] private Vector2 minBounds;  // マップ左下の座標
    [SerializeField] private Vector2 maxBounds;  // マップ右上の座標

    private Vector3 velocity = Vector3.zero;  // SmoothDamp用の内部速度
    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    // カメラ更新は LateUpdate で行う（対象が動き終わった後に追従する）
    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPos = transform.position;

        // --- デッドゾーン判定 ---
        // 対象とカメラの差がデッドゾーンを超えた分だけ、目標位置を動かす
        float deltaX = target.position.x - transform.position.x;
        float deltaY = target.position.y - transform.position.y;

        if (Mathf.Abs(deltaX) > deadZoneSize.x / 2f)
        {
            // はみ出した方向に、はみ出した分だけ目標を寄せる
            targetPos.x = target.position.x - Mathf.Sign(deltaX) * (deadZoneSize.x / 2f);
        }
        if (Mathf.Abs(deltaY) > deadZoneSize.y / 2f)
        {
            targetPos.y = target.position.y - Mathf.Sign(deltaY) * (deadZoneSize.y / 2f);
        }

        // --- 滑らかに追従 ---
        Vector3 smoothed = Vector3.SmoothDamp(
            transform.position, targetPos, ref velocity, smoothTime);

        // --- 境界制限 ---
        if (useBounds)
        {
            // カメラが映す範囲の半分（縦横）
            float halfHeight = cam.orthographicSize;
            float halfWidth = halfHeight * cam.aspect;

            smoothed.x = Mathf.Clamp(smoothed.x,
                minBounds.x + halfWidth, maxBounds.x - halfWidth);
            smoothed.y = Mathf.Clamp(smoothed.y,
                minBounds.y + halfHeight, maxBounds.y - halfHeight);
        }

        // Z は固定（2Dなのでカメラは手前に置いたまま）
        smoothed.z = transform.position.z;
        transform.position = smoothed;
    }

    // Sceneビューにデッドゾーンと境界を表示（調整用）
    private void OnDrawGizmos()
    {
        // デッドゾーン（黄色）
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position,
            new Vector3(deadZoneSize.x, deadZoneSize.y, 0));

        // 境界（シアン）
        if (useBounds)
        {
            Gizmos.color = Color.cyan;
            Vector3 center = new Vector3(
                (minBounds.x + maxBounds.x) / 2f,
                (minBounds.y + maxBounds.y) / 2f, 0);
            Vector3 size = new Vector3(
                maxBounds.x - minBounds.x,
                maxBounds.y - minBounds.y, 0);
            Gizmos.DrawWireCube(center, size);
        }
    }
}