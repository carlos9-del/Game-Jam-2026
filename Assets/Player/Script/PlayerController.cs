using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("データ")]
    [SerializeField] private PlayerConfig config;   // パラメータを持つ ScriptableObject

    [Header("接地判定")]
    [SerializeField] private Transform groundCheck; // 足元の判定位置

    // 外部（状態クラス）から使う参照とデータ
    public PlayerConfig Config => config;
    public Rigidbody2D Rb { get; private set; }

    // 状態機械
    public PlayerStateMachine StateMachine { get; private set; }

    // 各状態のインスタンス（使い回すので最初に作っておく）
    public IdleState IdleState { get; private set; }
    public MoveState MoveState { get; private set; }
    public JumpState JumpState { get; private set; }
    public FallState FallState { get; private set; }

    private void Awake()
    {
        // コンポーネントをキャッシュ
        Rb = GetComponent<Rigidbody2D>();

        // 重力倍率をデータから反映
        Rb.gravityScale = config.gravityScale;

        // 状態機械と各状態を生成
        StateMachine = new PlayerStateMachine();
        IdleState = new IdleState(this);
        MoveState = new MoveState(this);
        JumpState = new JumpState(this);
        FallState = new FallState(this);
    }

    private void Start()
    {
        // 最初は待機状態からスタート
        StateMachine.ChangeState(IdleState);
    }

    private void Update()
    {
        // 入力・状態遷移の判定
        StateMachine.Tick();
    }

    private void FixedUpdate()
    {
        // 物理系の処理
        StateMachine.FixedTick();

        ApplyBetterGravity();
    }

    // 足元に円を描いて、地面レイヤーに触れているか判定する
    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(
            groundCheck.position,
            config.groundCheckRadius,
            config.groundLayer
        );
    }
    // 上昇/落下で重力を変えて、キビキビしたジャンプ感にする
    private void ApplyBetterGravity()
    {
        if (Rb.linearVelocity.y < 0)
        {
            // 落下中：重力を強めて素早く落とす
            Rb.linearVelocity += Vector2.up * Physics2D.gravity.y
                * (config.fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (Rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            // 上昇中にジャンプキーを離した：重力を強めて低いジャンプにする
            Rb.linearVelocity += Vector2.up * Physics2D.gravity.y
                * (config.lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }
    // Sceneビューに判定円を表示（デバッグ用）
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null || config == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, config.groundCheckRadius);
    }
}