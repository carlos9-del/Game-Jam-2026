using UnityEngine;

// このアセットを右クリックメニューから作成できるようにする
[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Player/Player Config")]
public class PlayerConfig : ScriptableObject
{
    [Header("移動")]
    public float moveSpeed = 6f;         // 最大移動速度
    public float acceleration = 60f;     // 加速の速さ（大きいほどキビキビ）
    public float deceleration = 60f;     // 減速の速さ（大きいほどすぐ止まる）

    [Header("ジャンプ")]
    public float jumpForce = 16f;        // ジャンプの初速
    public float gravityScale = 5f;      // 上昇中の重力倍率
    public float fallMultiplier = 2f;    // 落下中に重力を強める倍率
    public float lowJumpMultiplier = 3f; // ジャンプキーを離した時に重力を強める倍率
    [Header("接地判定")]
    public float groundCheckRadius = 0.2f;  // 足元の判定円の半径
    public LayerMask groundLayer;           // 地面とみなすレイヤー

    [Header("操作感の補助")]
    public float coyoteTime = 0.1f;      // 地面を離れてもジャンプ可能な猶予時間
    public float jumpBufferTime = 0.1f;  // 着地前にジャンプ入力を先行受付する時間
}