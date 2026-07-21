using UnityEngine;

// 待機状態：地面で止まっている
public class IdleState : PlayerState
{
    public IdleState(PlayerController player) : base(player) { }

    public override void Tick() 
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

        // ジャンプ入力があれば Jump へ
        if (Input.GetButtonDown("Jump") && player.IsGrounded())
        {
            player.StateMachine.ChangeState(player.JumpState);
            return;
        }

        // 横入力があれば Move へ
        if (Mathf.Abs(moveInput) > 0.01f)
        {
            player.StateMachine.ChangeState(player.MoveState);
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
        // 横方向の速度を減速させて止める
        float x = Mathf.MoveTowards(player.Rb.linearVelocity.x, 0f,
            player.Config.deceleration * Time.fixedDeltaTime);
        player.Rb.linearVelocity = new Vector2(x, player.Rb.linearVelocity.y);
    }
}