
// 現在の状態を保持し、状態の切り替えを管理するクラス
public class PlayerStateMachine
{
    // 今実行中の状態
    public PlayerState CurrentState { get; private set; }

    // 状態を切り替える
    public void ChangeState(PlayerState newState)
    {
        // 今の状態から抜ける処理
        CurrentState?.Exit();

        // 新しい状態に差し替える
        CurrentState = newState;

        // 新しい状態に入る処理
        CurrentState.Enter();
    }

    // 毎フレームの更新（PlayerControllerのUpdateから呼ぶ）
    public void Tick()
    {
        CurrentState?.Tick();
    }

    // 物理更新（PlayerControllerのFixedUpdateから呼ぶ）
    public void FixedTick()
    {
        CurrentState?.FixedTick();
    }
}