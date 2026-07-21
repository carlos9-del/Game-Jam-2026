using UnityEngine;

// ジャンプ状態：上昇中
public class JumpState : PlayerState
{
    public JumpState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        // 入った瞬間に上向きの初速を与える（1回だけ）
        player.Rb.linearVelocity = new Vector2(player.Rb.linearVelocity.x,
            player.Config.jumpForce);
    }

    public override void Tick()
    {
        // 上昇が止まって落下に転じたら Fall へ
        if (player.Rb.linearVelocity.y <= 0f)
        {
            player.StateMachine.ChangeState(player.FallState);
        }
    }

    public override void FixedTick()
    {
        // 空中でも左右に動けるようにする
        float moveInput = Input.GetAxisRaw("Horizontal");
        float targetSpeed = moveInput * player.Config.moveSpeed;
        float x = Mathf.MoveTowards(player.Rb.linearVelocity.x, targetSpeed,
            player.Config.acceleration * Time.fixedDeltaTime);
        player.Rb.linearVelocity = new Vector2(x, player.Rb.linearVelocity.y);
    }
}