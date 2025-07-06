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
    [SerializeField] private string openAIApiKey;// OpenAI��API�L�[
    [SerializeField] private string modelVersion = "gpt-4";// �g�p����ChatGPT���f��
    [SerializeField] private int maxTokens = 150;// ��������ő�g�[�N����
    [SerializeField] private float temperature = 0.5f;// �����̃o���G�[�V����
    [TextArea]
    [SerializeField] private string initialSystemMessage = "����Ɂu�ɂ�v������";//�v�����v�g
    [SerializeField] private TMP_Text responseText;//������\��
    [SerializeField] private TMP_InputField questionInputField;//��������
                                                               //  [SerializeField] private SBV2SpeechStyle3 speechStyle3; SBV2�œǂݏグ��Ƃ��Ɏg��
                                                               //  [SerializeField] private VRMFaceEmotion vrmFaceEmotion;�@�\��Ƃ��̋����ɏ]���ăA�j���[�V�������Đ�������Ƃ��Ɏg��
    //[SerializeField]  private GPTVisualSpawner gptVisualSpawner;

    [SerializeField] private MoodItemDatabase moodItemDatabase;
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private GameObject happyroom;
    [SerializeField] private GameObject sadroom;

    private int Count = 0;
    private ChatGPTConnection chatGPTConnection;//ChatGPT�Ɛڑ�
    private const string MoodTagPattern = @"\[mood:(happy|sad)\]";

    private const string FaceTagPattern = @"\[face:([^\]_]+)_?(\d*)\]"; // �\��̋������o��
    private const string InterestTagPattern = @"\[interest:(\d)\]"; // ����ɑ΂���֐S�x�������o��

    private int inputCount = 0;
    private const int maxItems = 5;

    void Start()
    {
        chatGPTConnection = new ChatGPTConnection(openAIApiKey, initialSystemMessage, modelVersion, maxTokens, temperature);
    }

    //UniTask���g���Ă���̂ŁA�{�^���N���b�N�ɔ������Ď���𑗐M���邽�߂̃��b�p�[���\�b�h
    public void SendQuestionWrapper()
    {
        SendQuestion(questionInputField.text);
    }

    //�����ChatGPT�ɑ��M���A�������󂯎��񓯊����\�b�h
    public async UniTask SendQuestion(string question)
    {
        var response = await chatGPTConnection.RequestAsync(question);
        string responseContent = response.choices[0].message.content;
        // ����^�O�𒊏o
        string mood = "unknown";
        var moodMatch = Regex.Match(responseContent, MoodTagPattern);
        if (moodMatch.Success)
        {
            mood = moodMatch.Groups[1].Value; // "happy" or "sad"
            Debug.Log("����: " + mood);
        }
        // �֐S�^�O�𒊏o
        var interestMatch = Regex.Match(responseContent, InterestTagPattern);
        int interestLevel = -1; // �֐S���x���̏����l�𖳌��l��
        if (interestMatch.Success)
        {
            interestLevel = int.Parse(interestMatch.Groups[1].Value);
            Debug.Log($"�֐S���x��: {interestLevel}");
        }

        // �֐S���x����0�̏ꍇ�A�ԓ������ʂň͂�(�ǂݏグ���Ȃ�)
        if (interestLevel == 0)
        {
            responseContent = $"({responseContent})";
        }
        // �^�O���폜���������ȕԓ���\��
        string cleanedResponse = Regex.Replace(responseContent, MoodTagPattern, "").Trim();
        //responseText.text = cleanedResponse;


        //�ԓ�����^�O�ނ��폜���ď����ȕԓ��݂̂ɂ���
        //string cleanedResponse = ExtractAndLogFaceTags(responseContent, interestLevel);

        responseText.text = cleanedResponse;
        // speechStyle3.ReadText(cleanedResponse);�@�ǂݏグ�̂Ƃ��Ɏg��
       
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
   
    //�\��^�O�𒊏o
    private string ExtractAndLogFaceTags(string input, int interestLevel)
    {
        var matches = Regex.Matches(input, FaceTagPattern);
        var uniqueTags = new HashSet<string>(); // HashSet���g�p���ďd���������

        foreach (Match match in matches)
        {
            if (uniqueTags.Add(match.Value)) // ���ɓ����l���Ȃ��ꍇ�ɂ̂ݒǉ�
            {
                Debug.Log("�\��^�O�S��: " + match.Value);
                // �\��^�O����\��̖��O�Ƌ��x�𒊏o
                string emotionTag = match.Groups[1].Value;
                string emotionIntensityString = match.Groups[2].Value; // �\��̋��x��\��������
                if (int.TryParse(emotionIntensityString, out int emotionIntensity))
                {
                    // �ϊ��ɐ��������ꍇ�AemotionIntensity �ɂ͕ϊ����ꂽ�����l���i�[�����
                    Debug.Log($"�\��: {emotionTag}, ���x: {emotionIntensity}");
                    // ������ emotionIntensity ���g�p����
                }
                else
                {
                    // �ϊ��Ɏ��s�����ꍇ�̏���
                    Debug.LogWarning($"�\��̋��x '{emotionIntensityString}' �𐮐��ɕϊ��ł��܂���ł����B");
                    // �ϊ��Ɏ��s�����ꍇ�̏����������ɋL�q����
                }


                /*
                    //�^�O���g���K�[�ɂ��ĕ\���A�j���[�V�����𐧌䂷��
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

        // �܂� FaceTagPattern �Ƀ}�b�`���镔�����폜
        var tempInput = Regex.Replace(input, FaceTagPattern, "");

        // ���� InterestTagPattern �Ƀ}�b�`���镔�����폜
        var cleanedInput = Regex.Replace(tempInput, InterestTagPattern, "");

        // ���ʂ��R���\�[���ɏo��
        Debug.Log("ChatGPT�̕ԓ��i�\��^�O�����j: " + cleanedInput);
        return cleanedInput;
    }



}