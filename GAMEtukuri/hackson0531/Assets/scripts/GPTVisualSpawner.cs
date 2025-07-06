using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GPTVisualSpawner : MonoBehaviour
{
    [Header("ChatGPT連携")]
    [SerializeField] private GPTSpeak gptSpeak;

    [Header("感情プレハブ")]
    [SerializeField] private GameObject happyPrefab;
    [SerializeField] private GameObject sadPrefab;

    [Header("生成位置（Canvas内）")]
    [SerializeField] private Transform spawnParent;
    [SerializeField] private Vector2 spawnPosition = new Vector2(0, 0);

    private const string MoodTagPattern = @"\[mood:(happy|sad)\]";

    /// <summary>
    /// GPTからの返答に応じてオブジェクトを生成
    /// </summary>
    /// <param name="responseText">GPTの返答</param>
    public void HandleResponse(string responseText)
    {
        string mood = ExtractMood(responseText);
        Debug.Log("検出された気分: " + mood);

        GameObject prefabToSpawn = null;

        switch (mood)
        {
            case "happy":
                prefabToSpawn = happyPrefab;
                break;
            case "sad":
                prefabToSpawn = sadPrefab;
                break;
        }

        if (prefabToSpawn != null)
        {
            Vector3 worldPos = spawnParent.TransformPoint(spawnPosition);
            GameObject obj = Instantiate(prefabToSpawn, worldPos, Quaternion.identity, spawnParent);

            // Draggable を追加してプレイヤーが動かせるように
            if (obj.GetComponent<Draggable>() == null)
            {
                obj.AddComponent<Draggable>();
            }
        }
    }

    private string ExtractMood(string content)
    {
        var match = Regex.Match(content, MoodTagPattern);
        if (match.Success)
        {
            return match.Groups[1].Value;
        }
        return "";
    }
}
