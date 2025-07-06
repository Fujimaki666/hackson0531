using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GPTVisualSpawner : MonoBehaviour
{
    [Header("ChatGPT�A�g")]
    [SerializeField] private GPTSpeak gptSpeak;

    [Header("����v���n�u")]
    [SerializeField] private GameObject happyPrefab;
    [SerializeField] private GameObject sadPrefab;

    [Header("�����ʒu�iCanvas���j")]
    [SerializeField] private Transform spawnParent;
    [SerializeField] private Vector2 spawnPosition = new Vector2(0, 0);

    private const string MoodTagPattern = @"\[mood:(happy|sad)\]";

    /// <summary>
    /// GPT����̕ԓ��ɉ����ăI�u�W�F�N�g�𐶐�
    /// </summary>
    /// <param name="responseText">GPT�̕ԓ�</param>
    public void HandleResponse(string responseText)
    {
        string mood = ExtractMood(responseText);
        Debug.Log("���o���ꂽ�C��: " + mood);

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

            // Draggable ��ǉ����ăv���C���[����������悤��
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
