using UnityEngine;
using System.Collections.Generic;

public class ColorCounter : MonoBehaviour
{
    // どこからでもアクセスできる唯一のインスタンス
    public static ColorCounter Instance;

    // 色ごとの個数を記録する辞書
    private Dictionary<BallColorType, int> counts = new Dictionary<BallColorType, int>();

    //void Awake()
    //{
    //    if (Instance == null) Instance = this;
    //    else { Destroy(gameObject); return; }

    //    // 4色ぶんを0で初期化しておく
    //    counts[BallColorType.Red] = 0;
    //    counts[BallColorType.Yellow] = 0;
    //    counts[BallColorType.Blue] = 0;
    //    counts[BallColorType.Green] = 0;
    //}
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            counts[BallColorType.Red] = 0;
            counts[BallColorType.Yellow] = 0;
            counts[BallColorType.Blue] = 0;
            counts[BallColorType.Green] = 0;
        }
        else { Destroy(gameObject); return; }
    }

    // ===== 計数を1増やす（球が穴に入った時に呼ぶ）=====
    public void AddCount(BallColorType color)
    {
        if (counts.ContainsKey(color))
        {
            counts[color]++;
        }
        else
        {
            counts[color] = 1;   // 念のため、無い色でも作る
        }

        Debug.Log(color + " の数：" + counts[color]);
    }

    // ===== 指定した色の現在の数を取得する =====
    public int GetCount(BallColorType color)
    {
        if (counts.ContainsKey(color)) return counts[color];
        return 0;
    }

    // ===== 全色の合計を取得する =====
    public int GetTotalCount()
    {
        int total = 0;
        foreach (var pair in counts)
        {
            total += pair.Value;
        }
        return total;
    }

    // ===== 全部リセットする（次のゲームなどで使う）=====
    public void ResetAll()
    {
        List<BallColorType> keys = new List<BallColorType>(counts.Keys);
        foreach (var key in keys)
        {
            counts[key] = 0;
        }
    }
}