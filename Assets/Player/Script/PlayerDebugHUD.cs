using UnityEngine;

// デバッグ用HUD：プレイヤーの状態を画面に表示する（一時的なツール）
[RequireComponent(typeof(PlayerController))]
public class PlayerDebugHUD : MonoBehaviour
{
    private PlayerController player;

    [Header("Rayの設定")]
    [SerializeField] private float rayLength = 0.5f;  // 下向きに飛ばすRayの長さ

    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    private void OnGUI()
    {
        // 表示スタイル（文字を大きく・見やすく）
        GUIStyle style = new GUIStyle();
        style.fontSize = 22;
        style.normal.textColor = Color.white;

        // 現在の状態名を取得
        string stateName = player.StateMachine.CurrentState != null
            ? player.StateMachine.CurrentState.GetType().Name
            : "None";

        // 接地判定
        bool grounded = player.IsGrounded();

        // 位置と速度
        Vector2 pos = player.transform.position;
        Vector2 vel = player.Rb.linearVelocity;

        // 画面左上に情報をまとめて表示
        string info =
            $"State     : {stateName}\n" +
            $"Grounded  : {grounded}\n" +
            $"Position  : ({pos.x:F2}, {pos.y:F2})\n" +
            $"Velocity  : ({vel.x:F2}, {vel.y:F2})";

        GUI.Label(new Rect(20, 20, 500, 200), info, style);
    }

    // Sceneビューに下向きのRayを描く（地面検出の可視化）
    private void OnDrawGizmos()
    {
        Vector3 origin = transform.position;

        // 下向きにRayを飛ばして地面に当たったか調べる
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, rayLength);

        // 当たったら緑、当たらなければ赤で線を描く
        Gizmos.color = hit.collider != null ? Color.green : Color.red;
        Gizmos.DrawLine(origin, origin + Vector3.down * rayLength);
    }
}