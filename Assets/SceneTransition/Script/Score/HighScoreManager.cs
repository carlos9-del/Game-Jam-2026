using System.Collections.Generic;
using UnityEngine;

// =============================================
// High score data / ハイスコアデータ
// =============================================
// EN: Serializable wrapper holding the list of saved scores, used for JSON storage.
// JP: 保存されたスコアのリストを保持する、JSON保存用のシリアライズ可能なラッパークラス。
[System.Serializable]
public class HighScoreData
{
    public List<int> scores = new List<int>();
}

// =============================================
// High score manager / ハイスコア管理クラス
// =============================================
// EN: Persistent singleton for saving and loading the top scores. Persists across game sessions
//     using PlayerPrefs (survives closing and reopening the game, unlike scene-to-scene data).
//     Place in the same scene as your other persistent managers (e.g. Title).
// JP: 上位スコアを保存・読み込みするための永続的なシングルトン。
//     PlayerPrefsを使用してゲームセッションをまたいで保持される
//     （シーン間のデータとは異なり、ゲームを閉じて再度開いても保持される）。
//     他の永続的なマネージャーと同じシーン（例：Title）に配置する。
public class HighScoreManager : SingletonMonoBehaviour<HighScoreManager>
{
    [Header("Number Of Top Scores To Keep")]
    [SerializeField] private int maxEntries = 5;

    private const string SaveKey = "HighScores";

    // EN: Adds a new score to the saved list, keeps only the top N (highest first), and saves it.
    // JP: 新しいスコアを保存済みリストに追加し、上位N件（高い順）のみを保持して保存する。
    public void SaveScore(int newScore)
    {
        HighScoreData data = LoadScores();
        data.scores.Add(newScore);
        data.scores.Sort((a, b) => b.CompareTo(a)); // EN: descending order / JP: 降順に並び替え

        if (data.scores.Count > maxEntries)
        {
            data.scores.RemoveRange(maxEntries, data.scores.Count - maxEntries);
        }

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    // EN: Loads the saved top scores. Returns an empty list if nothing has been saved yet.
    // JP: 保存されている上位スコアを読み込む。まだ何も保存されていない場合は空のリストを返す。
    public HighScoreData LoadScores()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            string json = PlayerPrefs.GetString(SaveKey);
            return JsonUtility.FromJson<HighScoreData>(json);
        }

        return new HighScoreData();
    }
}