
// 全ての状態が継承する基底クラス
public abstract class PlayerState
{
    // この状態を持つプレイヤー本体への参照
   protected PlayerController player;

    // コンストラクタでプレイヤー本体を受け取る
    public PlayerState(PlayerController player)
    {
        this.player = player;
    }

    // 状態に入った瞬間に1回だけ呼ばれる
    public virtual void Enter() { }

    // 状態中、毎フレーム呼ばれる（入力処理・状態遷移の判定）
    public virtual void Tick() { }

    // 物理演算に関わる処理（FixedUpdateで呼ばれる）
    public virtual void FixedTick() { }

    // 状態から出る瞬間に1回だけ呼ばれる
    public virtual void Exit() { }
}