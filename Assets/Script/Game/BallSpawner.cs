using UnityEngine;
using System.Collections.Generic;

public class BallSpawner : MonoBehaviour
{
    [Header("生成の設定")]
    public GameObject ballPrefab;          // 生成するボールのPrefab
    public Vector2 center = Vector2.zero;  // 回転中心
    public float spawnInterval = 1.5f;     // 生成間隔（秒）
    public int maxSpinningBalls = 8;       // 「回っている」ボールの上限

    [Header("射出の設定")]
    public float chargeRate = 5f;       // 長押し中、1秒あたり何個ぶん溜まるか
    public int maxChargeCount = 8;      // 一度に射出できる最大数

    private float chargeTime = 0f;      // 長押し中の溜め時間
    private bool isCharging = false;    // 溜め中か
    private float timer = 0f;
    private int spawnCounter = 0;

    // 「回っている」ボールだけを管理するリスト
    private List<GameObject> spinningBalls = new List<GameObject>();

    void Update()
    {

        // --- 生成の処理（既存）---
        spinningBalls.RemoveAll(ball => ball == null);
        if (spinningBalls.Count < maxSpinningBalls)
        {
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                SpawnBall();
                timer = 0f;
            }
        }

        // --- 射出の処理（Aボタン：キーボードのJで仮テスト）---
        HandleLaunchInput();
    }

    void HandleLaunchInput()
    {
        // 押し始め：溜め開始
        if (Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            isCharging = true;
            chargeTime = 0f;
        }

        // 押している間：溜め続ける
        if (isCharging && Input.GetKey(KeyCode.JoystickButton0))
        {
            chargeTime += Time.deltaTime;
        }

        // 離した：溜めた分だけ一斉射出
        if (isCharging && Input.GetKeyUp(KeyCode.JoystickButton0))
        {
            isCharging = false;

            // 溜め時間から射出数を決める（最低1個、最大maxChargeCount個）
            int launchCount = 1 + Mathf.FloorToInt(chargeTime * chargeRate);
            launchCount = Mathf.Clamp(launchCount, 1, maxChargeCount);

            LaunchOldest(launchCount);
        }
    }

    // 古い順にcount個のボールを射出する
    void LaunchOldest(int count)
    {
        int launched = 0;

        // spinningBallsは生成順に並んでいる → 先頭が最も古い
        // 射出するとOnBallLaunchedでリストから外れるので、コピーを作って回す
        List<GameObject> snapshot = new List<GameObject>(spinningBalls);

        foreach (GameObject ballObj in snapshot)
        {
            if (launched >= count) break;
            if (ballObj == null) continue;

            BallLauncher ball = ballObj.GetComponent<BallLauncher>();
            if (ball != null)
            {
                ball.Launch();
                launched++;
            }
        }

        Debug.Log("射出数：" + launched);
    }

    void SpawnBall()
    {
        GameObject newBall = Instantiate(ballPrefab, center, Quaternion.identity);

        BallLauncher launcher = newBall.GetComponent<BallLauncher>();
        if (launcher != null)
        {
            // 出生角度をずらす（黄金角137.5度で綺麗に散らばる）
            float angle = spawnCounter * 137.5f;
            launcher.SetStartAngle(angle);

            // 自分（スポナー）を教えておく
            launcher.SetSpawner(this);

            // ランダムで色を割り当てる（4色から1つ）
            BallColorType randomColor = GetRandomColor();
            launcher.SetColor(randomColor);
        }

        spinningBalls.Add(newBall);
        spawnCounter++;
    }

    // 4色からランダムに1つ選ぶ
    BallColorType GetRandomColor()
    {
        BallColorType[] colors = {
            BallColorType.Red,
            BallColorType.Yellow,
            BallColorType.Blue,
            BallColorType.Green
        };
        return colors[Random.Range(0, colors.Length)];
    }

    // ボールが射出されたら呼ばれる：回っているリストから外す
    public void OnBallLaunched(GameObject ball)
    {
        spinningBalls.Remove(ball);
    }
}