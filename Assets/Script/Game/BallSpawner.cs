using UnityEngine;
using System.Collections.Generic;

public class BallSpawner : MonoBehaviour
{
    [Header("生成の設定")]
    public GameObject ballPrefab;          // 生成するボールのPrefab
    public Vector2 center = Vector2.zero;  // 回転中心
    public float spawnInterval = 1.5f;     // 生成間隔（秒）
    public int maxSpinningBalls = 8;       // 「回っている」ボールの上限

    private float timer = 0f;
    private int spawnCounter = 0;

    // 「回っている」ボールだけを管理するリスト
    private List<GameObject> spinningBalls = new List<GameObject>();

    void Update()
    {
        // 消えた（Destroyされた）ボールを取り除く
        spinningBalls.RemoveAll(ball => ball == null);

        // 回っているボールが上限未満なら生成する
        if (spinningBalls.Count < maxSpinningBalls)
        {
            timer += Time.deltaTime;

            if (timer >= spawnInterval)
            {
                SpawnBall();
                timer = 0f;
            }
        }
    }

    void SpawnBall()
    {
        GameObject newBall = Instantiate(ballPrefab, center, Quaternion.identity);

        float angle = spawnCounter * 137.5f;   // 黄金角でずらす

        BallLauncher launcher = newBall.GetComponent<BallLauncher>();
        if (launcher != null)
        {
            launcher.SetStartAngle(angle);
            launcher.SetSpawner(this);   // 自分を教えておく
        }

        spinningBalls.Add(newBall);
        spawnCounter++;
    }

    // ボールが射出されたら呼ばれる：回っているリストから外す
    public void OnBallLaunched(GameObject ball)
    {
        spinningBalls.Remove(ball);
    }
}