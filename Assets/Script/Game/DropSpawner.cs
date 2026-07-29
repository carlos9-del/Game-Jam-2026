using UnityEngine;

public class DropSpawner : MonoBehaviour
{
    [Header("落下ボールの設定")]
    public GameObject[] dropPrefabs;    // 4種類の落下専用Prefab（0=Red,1=Yellow,2=Blue,3=Green）

    [Header("落下タイミング")]
    public int scoreStep = 200;         // 何点ごとに1個落とすか

    [Header("落下する範囲")]
    public float dropY = 6f;            // 落とす高さ（画面上方）
    public float rangeLeft = -4f;       // 落下範囲の左端
    public float rangeRight = 4f;       // 落下範囲の右端

    // 前回までに「200の倍数を何個超えたか」
    private int lastMilestone = 0;

    void Update()
    {
        // ScoreManagerが無ければ何もしない（安全対策）
        if (ScoreManager.Instance == null) return;

        int score = ScoreManager.Instance.GetScore();

        // 今のスコアは200の倍数をいくつ超えているか
        int currentMilestone = score / scoreStep;

        // 前回より増えた分だけ落とす（複数またいでも対応）
        while (lastMilestone < currentMilestone)
        {
            DropOne();
            lastMilestone++;
        }
    }

    // ボールを1個、ランダムな水平位置に落とす
    void DropOne()
    {
        if (dropPrefabs == null || dropPrefabs.Length == 0) return;

        // 4種類からランダムに選ぶ
        int index = Random.Range(0, dropPrefabs.Length);
        GameObject prefab = dropPrefabs[index];

        // 範囲内のランダムな水平位置を決める
        float randomX = Random.Range(rangeLeft, rangeRight);
        Vector2 dropPos = new Vector2(randomX, dropY);

        // 生成する（あとは重力で落ちる）
        Instantiate(prefab, dropPos, Quaternion.identity);

        Debug.Log("落下ボール生成：" + prefab.name + " at x=" + randomX);
    }
    void OnDrawGizmosSelected()
    {
        // 落下範囲を線で描く（Sceneビューで確認用）
        Gizmos.color = Color.green;

        Vector3 left = new Vector3(rangeLeft, dropY, 0);
        Vector3 right = new Vector3(rangeRight, dropY, 0);

        // 落下する高さの横線を引く
        Gizmos.DrawLine(left, right);

        // 両端に小さいマークを付ける
        Gizmos.DrawWireSphere(left, 0.2f);
        Gizmos.DrawWireSphere(right, 0.2f);
    }
}