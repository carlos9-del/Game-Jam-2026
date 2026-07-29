using UnityEngine;
using System.Collections.Generic;

public class BallSpawner : MonoBehaviour
{
    [Header("生成の設定")]
    public GameObject[] ballPrefabs;       // 4種類のボールPrefab（順番：0=Red,1=Yellow,2=Blue,3=Green）
    public Vector2 center = Vector2.zero;  // 回転中心
    public float spawnInterval = 1.5f;     // 生成間隔（秒）
    public int maxSpinningBalls = 8;       // 「回っている」ボールの上限

    [Header("回転速度による自動発射")]
    public RingRotator innerRing;       // 里層の壁（速度をここから読む）
    public float speedAtSlow = 30f;     // この速度のとき…
    public float intervalAtSlow = 1f;   // …この間隔で発射（秒）
    public float speedAtFast = 180f;    // この速度のとき…
    public float intervalAtFast = 0.3f; // …この間隔で発射（秒）

    private float launchTimer = 0f;     // 発射タイマー

    [Header("射出の設定")]
    public float chargeRate = 5f;          // 長押し中、1秒あたり何個ぶん溜まるか
    public int maxChargeCount = 8;         // 一度に射出できる最大数

    [Header("発射エフェクト")]
    public GameObject launchEffectPrefab;   // 発射時のエフェクトPrefab
    public float effectCooldown = 0.2f;     // エフェクトを出せる最短間隔（秒）

    private float lastEffectTime = -999f;    // 前回エフェクトを出した時刻
    // ===== 内部の状態 =====
    private float timer = 0f;              // 生成タイマー
    private int spawnCounter = 0;          // 何個目を生成したか（角度をずらすため）

    private float chargeTime = 0f;         // 長押し中の溜め時間
    private bool isCharging = false;       // 溜め中か

    // 「回っている」ボールだけを管理するリスト（生成順）
    private List<GameObject> spinningBalls = new List<GameObject>();

    void Update()
    {
        // --- 生成の処理（既存のまま）---
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

        // --- 回転速度に応じた自動発射 ---
        HandleAutoLaunch();
    }

    void PlayLaunchEffect()
    {
        if (launchEffectPrefab == null) return;

        // 前回から effectCooldown 秒たっていなければ、今回は出さない
        if (Time.time - lastEffectTime < effectCooldown)
        {
            return;
        }

        // エフェクトを生成して、今の時刻を記録する
        Instantiate(launchEffectPrefab, center, Quaternion.identity);
        lastEffectTime = Time.time;
    }

    // ===== 生成 =====
    void SpawnBall()
    {
        // Prefabが設定されていなければ何もしない（安全対策）
        if (ballPrefabs == null || ballPrefabs.Length == 0) return;

        // 4種類からランダムに1つ選ぶ
        int index = Random.Range(0, ballPrefabs.Length);
        GameObject prefab = ballPrefabs[index];

        GameObject newBall = Instantiate(prefab, center, Quaternion.identity);

        BallLauncher launcher = newBall.GetComponent<BallLauncher>();
        if (launcher != null)
        {
            // 出生角度をずらす（黄金角137.5度で綺麗に散らばる）
            float angle = spawnCounter * 137.5f;
            launcher.SetStartAngle(angle);

            // 自分（スポナー）を教えておく
            launcher.SetSpawner(this);

            // 選んだ番号に対応する色をセットする（染色はしない、色情報だけ）
            launcher.SetColorType(IndexToColor(index));
        }

        spinningBalls.Add(newBall);
        spawnCounter++;
    }

    // 配列の番号を色の種類に変換する
    BallColorType IndexToColor(int index)
    {
        switch (index)
        {
            case 0: return BallColorType.Red;
            case 1: return BallColorType.Yellow;
            case 2: return BallColorType.Blue;
            case 3: return BallColorType.Green;
            default: return BallColorType.Red;
        }
    }

    //// ===== 射出の入力処理（Aボタン：キーボードのJでも仮テスト可）=====
    //void HandleLaunchInput()
    //{
    //    // 押し始め：溜め開始
    //    if (Input.GetKeyDown(KeyCode.JoystickButton5) || Input.GetKeyDown(KeyCode.J))
    //    {
    //        isCharging = true;
    //        chargeTime = 0f;
    //    }

    //    // 押している間：溜め続ける
    //    if (isCharging &&
    //        (Input.GetKey(KeyCode.JoystickButton5) || Input.GetKey(KeyCode.J)))
    //    {
    //        chargeTime += Time.deltaTime;
    //    }

    //    // 離した：溜めた分だけ一斉射出
    //    if (isCharging &&
    //        (Input.GetKeyUp(KeyCode.JoystickButton5) || Input.GetKeyUp(KeyCode.J)))
    //    {
    //        isCharging = false;

    //        // 溜め時間から射出数を決める（最低1個、最大maxChargeCount個）
    //        int launchCount = 1 + Mathf.FloorToInt(chargeTime * chargeRate);
    //        launchCount = Mathf.Clamp(launchCount, 1, maxChargeCount);

    //        LaunchOldest(launchCount);

    //        PlayLaunchEffect();
    //    }
    //}

    // 古い順にcount個のボールを射出する
    void LaunchOldest(int count)
    {
        int launched = 0;

        // Launchするとリストから外れるので、コピーを作って回す
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

    // ===== ボールが射出された時に呼ばれる =====
    public void OnBallLaunched(GameObject ball)
    {
        // 回っているリストから外す（射出後は上限にカウントしない）
        spinningBalls.Remove(ball);
    }
    void HandleAutoLaunch()
    {
        if (innerRing == null) return;

        float speed = innerRing.CurrentSpeed;

        // 速度が30未満なら発射しない（止まっている時は撃たない）
        if (speed < speedAtSlow)
        {
            launchTimer = 0f;   // タイマーもリセットしておく
            return;
        }

        // 速度から発射間隔を求める
        float t = Mathf.InverseLerp(speedAtSlow, speedAtFast, speed);
        float interval = Mathf.Lerp(intervalAtSlow, intervalAtFast, t);

        launchTimer += Time.deltaTime;
        if (launchTimer >= interval)
        {
            LaunchOne();
            launchTimer = 0f;
        }
    }

    // 一番古い球を1個だけ発射する
    void LaunchOne()
    {
        // spinningBallsの先頭が最も古い
        foreach (GameObject ballObj in spinningBalls)
        {
            if (ballObj == null) continue;

            BallLauncher ball = ballObj.GetComponent<BallLauncher>();
            if (ball != null)
            {
                ball.Launch();

                // 発射エフェクト（syasyutu）を中心で再生する
                PlayLaunchEffect();

                return;   // 1個だけ発射して終わり
            }
        }
    }
}