using UnityEngine;

// 落下状態：空中で下降中
public class FallState : PlayerState
{
    public FallState(PlayerController player) : base(player) { }

    public override void Tick()
    {
        // 着地したら、入力に応じて Idle か Move へ
        if (player.IsGrounded())
        {
            float moveInput = Input.GetAxisRaw("Horizontal");
            if (Mathf.Abs(moveInput) > 0.01f)
                player.StateMachine.ChangeState(player.MoveState);
            else
                player.StateMachine.ChangeState(player.IdleState);
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