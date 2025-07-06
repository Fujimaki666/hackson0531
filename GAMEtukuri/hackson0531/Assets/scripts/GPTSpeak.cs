using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CHATGPT.OpenAI;
using Cysharp.Threading.Tasks;
using System.Text.RegularExpressions;
using TMPro;

public class GPTSpeak : MonoBehaviour
{
    [SerializeField] private string openAIApiKey;// OpenAIのAPIキー
    [SerializeField] private string modelVersion = "gpt-4";// 使用するChatGPTモデル
    [SerializeField] private int maxTokens = 150;// 生成する最大トークン数
    [SerializeField] private float temperature = 0.5f;// 応答のバリエーション
    [TextArea]
    [SerializeField] private string initialSystemMessage = "語尾に「にゃ」をつけて";//プロンプト
    [SerializeField] private TMP_Text responseText;//応答を表示
    [SerializeField] private TMP_InputField questionInputField;//質問を入力
                                                               //  [SerializeField] private SBV2SpeechStyle3 speechStyle3; SBV2で読み上げるときに使う
                                                               //  [SerializeField] private VRMFaceEmotion vrmFaceEmotion;　表情とその強さに従ってアニメーションを再生させるときに使う
    //[SerializeField]  private GPTVisualSpawner gptVisualSpawner;

    [SerializeField] private MoodItemDatabase moodItemDatabase;
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private GameObject happyroom;
    [SerializeField] private GameObject sadroom;

    private int Count = 0;
    private ChatGPTConnection chatGPTConnection;//ChatGPTと接続
    private const string MoodTagPattern = @"\[mood:(happy|sad)\]";

    private const string FaceTagPattern = @"\[face:([^\]_]+)_?(\d*)\]"; // 表情の強さも出力
    private const string InterestTagPattern = @"\[interest:(\d)\]"; // 質問に対する関心度合いを出力

    private int inputCount = 0;
    private const int maxItems = 5;

    void Start()
    {
        chatGPTConnection = new ChatGPTConnection(openAIApiKey, initialSystemMessage, modelVersion, maxTokens, temperature);
    }

    //UniTaskを使っているので、ボタンクリックに反応して質問を送信するためのラッパーメソッド
    public void SendQuestionWrapper()
    {
        SendQuestion(questionInputField.text);
    }

    //質問をChatGPTに送信し、応答を受け取る非同期メソッド
    public async UniTask SendQuestion(string question)
    {
        var response = await chatGPTConnection.RequestAsync(question);
        string responseContent = response.choices[0].message.content;
        // 感情タグを抽出
        string mood = "unknown";
        var moodMatch = Regex.Match(responseContent, MoodTagPattern);
        if (moodMatch.Success)
        {
            mood = moodMatch.Groups[1].Value; // "happy" or "sad"
            Debug.Log("感情: " + mood);
        }
        // 関心タグを抽出
        var interestMatch = Regex.Match(responseContent, InterestTagPattern);
        int interestLevel = -1; // 関心レベルの初期値を無効値に
        if (interestMatch.Success)
        {
            interestLevel = int.Parse(interestMatch.Groups[1].Value);
            Debug.Log($"関心レベル: {interestLevel}");
        }

        // 関心レベルが0の場合、返答を括弧で囲む(読み上げしない)
        if (interestLevel == 0)
        {
            responseContent = $"({responseContent})";
        }
        // タグを削除した純粋な返答を表示
        string cleanedResponse = Regex.Replace(responseContent, MoodTagPattern, "").Trim();
        //responseText.text = cleanedResponse;


        //返答からタグ類を削除して純粋な返答のみにする
        //string cleanedResponse = ExtractAndLogFaceTags(responseContent, interestLevel);

        responseText.text = cleanedResponse;
        // speechStyle3.ReadText(cleanedResponse);　読み上げのときに使う
       
        switch (mood)
        {
            case "happy":
                Count++;
                if (Count == 1)
                {
                    happyroom.SetActive(true);
                }
                //responseText.color = Color.yellow;
                if (moodItemDatabase != null && inventoryUI != null)
                {
                    

                    //inventoryUI.DisplayItems(moodItemDatabase.happyItems);
                    inventoryUI.AddItemByMood("happy");
                }
                break;

            case "sad":
                Count++;
                if (Count == 1)
                {
                    sadroom.SetActive(true);
                }
                //responseText.color = Color.cyan;
                if (moodItemDatabase != null && inventoryUI != null)
                {
                    //inventoryUI.DisplayItems(moodItemDatabase.sadItems);
                    inventoryUI.AddItemByMood("sad");
                }
                break;

            default:
                responseText.color = Color.black;
                inventoryUI.ClearItems();
                break;
        }
        //gptVisualSpawner.HandleResponse(responseContent);


    }
   
    //表情タグを抽出
    private string ExtractAndLogFaceTags(string input, int interestLevel)
    {
        var matches = Regex.Matches(input, FaceTagPattern);
        var uniqueTags = new HashSet<string>(); // HashSetを使用して重複を避ける

        foreach (Match match in matches)
        {
            if (uniqueTags.Add(match.Value)) // 既に同じ値がない場合にのみ追加
            {
                Debug.Log("表情タグ全部: " + match.Value);
                // 表情タグから表情の名前と強度を抽出
                string emotionTag = match.Groups[1].Value;
                string emotionIntensityString = match.Groups[2].Value; // 表情の強度を表す文字列
                if (int.TryParse(emotionIntensityString, out int emotionIntensity))
                {
                    // 変換に成功した場合、emotionIntensity には変換された整数値が格納される
                    Debug.Log($"表情: {emotionTag}, 強度: {emotionIntensity}");
                    // ここで emotionIntensity を使用する
                }
                else
                {
                    // 変換に失敗した場合の処理
                    Debug.LogWarning($"表情の強度 '{emotionIntensityString}' を整数に変換できませんでした。");
                    // 変換に失敗した場合の処理をここに記述する
                }


                /*
                    //タグをトリガーにして表情やアニメーションを制御する
                    if (vrmFaceEmotion != null)
                    {
                        vrmFaceEmotion.ChangeExpressionBasedOnEmotionTag(emotionTag);
                    }
                    else
                    {
                        Debug.LogWarning("VRMFaceEmotion component is not set or found.");
                    }

                    if (speechStyle3 != null)
                    {
                        speechStyle3.SetStyleAndIntensityBasedOnEmotionTag(emotionTag, emotionIntensity);
                        speechStyle3.SetVolumeBasedOnInterestLevel(interestLevel, emotionIntensity);


                    }
                    else
                    {
                        Debug.LogWarning("speechStyle component is not set or found.");
                    }



                    if (vrmStateAnimController != null)
                    {
                        vrmStateAnimController.ChangeAnimationBasedOnEmotion(emotionTag);
                    }
                    else
                    {
                        Debug.LogWarning("VRMStateAnimController component is not set or found.");
                    }
                    */
            }
        }

        // まず FaceTagPattern にマッチする部分を削除
        var tempInput = Regex.Replace(input, FaceTagPattern, "");

        // 次に InterestTagPattern にマッチする部分を削除
        var cleanedInput = Regex.Replace(tempInput, InterestTagPattern, "");

        // 結果をコンソールに出力
        Debug.Log("ChatGPTの返答（表情タグ除去）: " + cleanedInput);
        return cleanedInput;
    }



}