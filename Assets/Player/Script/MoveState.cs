using UnityEngine;

// 移動状態：地面で左右に動いている
public class MoveState : PlayerState
{
    public MoveState(PlayerController player) : base(player) { }

    private float moveInput;

    public override void Tick()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        // ジャンプ入力があれば Jump へ
        if (Input.GetButtonDown("Jump") && player.IsGrounded())
        {
            player.StateMachine.ChangeState(player.JumpState);
            return;
        }

        // 横入力が無くなったら Idle へ
        if (Mathf.Abs(moveInput) < 0.01f)
        {
            player.StateMachine.ChangeState(player.IdleState);
            return;
        }

        // 地面から落ちたら Fall へ
        if (!player.IsGrounded())
        {
            player.StateMachine.ChangeState(player.FallState);
        }
    }

    public override void FixedTick()
    {
        // 目標速度へ向けて加速する
        float targetSpeed = moveInput * player.Config.moveSpeed;
        float x = Mathf.MoveTowards(player.Rb.linearVelocity.x, targetSpeed,
            player.Config.acceleration * Time.fixedDeltaTime);
        player.Rb.linearVelocity = new Vector2(x, player.Rb.linearVelocity.y);
    }
}