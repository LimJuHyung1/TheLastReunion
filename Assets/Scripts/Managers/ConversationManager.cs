using OpenAI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;


public class ConversationManager : MonoBehaviour
{
    public AudioClip[] typeSounds;
    public GameObject player;    

    public CameraManager cameraManager;
    public LogManager logManager;
    public UIManager uIManager;

    private bool isTalking = false;
    private bool isAbleToGoNext = false;
    private Coroutine displayCoroutine;
    private Queue<string> sentencesQueue = new Queue<string>();    

    // [SerializeField]Police npcRole;
    [SerializeField] NPCRole npcRole;

    // Start is called before the first frame update
    void Start()
    {        
        SoundManager.Instance.SetNullAudioMixerGroup();
    }

    void Update()
    {
        if (isTalking)
        {
            if(uIManager.GetAskField().text == "")
            {
                uIManager.GetEndConversationButton().gameObject.SetActive(uIManager.GetIsSkipping());
            }                
            else
                uIManager.GetEndConversationButton().gameObject.SetActive(false);            
        }            
    }

    /// <summary>
    /// Player ��ũ��Ʈ���� ���� npc ��ȯ ����
    /// </summary>
    /// <param NPC Object="col"></param>    
    public void GetNPCRole(NPCRole npc)
    {
        npcRole = npc;        
    }

    /// <summary>
    /// �ӽ÷� ��ư�� ���� npc �亯 �޾Ƴ�
    /// </summary>
    public void AddListenersResponse()
    {
        if(npcRole != null)
        {            
            uIManager.onEndEditAskField(npcRole.GetResponse);
            uIManager.onEndEditAskField(uIManager.SetNullInputField);
        }                        
    }

    public void RemoveNPCRole()
    {
        npcRole = null;
    }

    //-------------------------------------------------------------//

    public void ShowName()
    {
        if (npcRole != null)
        {
            switch (npcRole.currentCharacter.ToString())
            {
                case "Nason":
                    uIManager.ChangeNPCName("���̽�");
                    break;
                case "Mina":
                    uIManager.ChangeNPCName("�̳�");
                    break;
                case "Jenny":
                    uIManager.ChangeNPCName("����");
                    break;
                case "Police":
                    uIManager.ChangeNPCName("����");
                    break;

                default:
                    npcRole.currentCharacter.ToString(npcRole.currentCharacter.ToString());
                    break;
            }
        }        
        else
            Debug.LogError("ShowName - npc�� �����ϴ�!");
    }

    /// <summary>
    /// NPC ��ũ��Ʈ���� �����
    /// </summary>
    /// <param chatGPT Answer="answer"></param>
    public void ShowAnswer(string answer)
    {
        if (npcRole != null)
        {
            logManager.AddLog(npcRole, uIManager.GetAskFieldText(), answer);
            uIManager.SetInteractableAskField(false);
            SetAudio();

            sentencesQueue.Clear();

            // ���Խ��� �̿��� ������ '.', '?', '!' ���ڷ� �����ϸ鼭 �ش� ���� ����
            string[] sentences = System.Text.RegularExpressions.Regex.Split(answer, @"(?<=[.!?])");

            foreach (string part in sentences)
            {
                if (!string.IsNullOrWhiteSpace(part)) // ������ �ƴ� ��ȿ�� ���ڿ��� ó��
                {
                    string trimmedSentence = part.Trim();
                    sentencesQueue.Enqueue(trimmedSentence);
                }
            }

            if (displayCoroutine == null)
            {
                displayCoroutine = StartCoroutine(DisplaySentences());
            }
        }
        else
        {
            Debug.LogError("npc�� �����ϴ�!");
        }
    }

    /// <summary>
    /// NPC�� �亯�� �ѹ��� �� ȭ�鿡 ���
    /// </summary>
    /// <returns></returns>
    private IEnumerator DisplaySentences()
    {
        do
        {
            string sentence = sentencesQueue.Dequeue();
            isAbleToGoNext = false; // ���� �������� �Ѿ �� ������ ����

            ChatMessage message = new ChatMessage { Content = sentence };
            npcRole.PlayEmotion(message);

            yield return StartCoroutine(uIManager.ShowLine(uIManager.GetNPCAnswer(), sentence)); // �ؽ�Ʈ ǥ�� �ڷ�ƾ ����

            // ���콺 Ŭ���� ��ٸ��� ���� ���� �������� �Ѿ �� �ֵ��� ���
            yield return new WaitUntil(() => isAbleToGoNext);

            if (sentencesQueue.Count > 0 && isAbleToGoNext)
            {
                yield return new WaitUntil(() => Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)); // ���콺 Ŭ�� �Ǵ� ����Ű ���
                uIManager.ChangeIsSkipping(true);
            }
        }
        while (sentencesQueue.Count > 0);

        uIManager.SetInteractableAskField(true);
        uIManager.ChangeIsSkipping(true);
        displayCoroutine = null; // �ڷ�ƾ�� ������ ���� �ʱ�ȭ
    }

    public void IsAbleToGoNextTrue()
    {
        isAbleToGoNext = true;
    }

    //-------------------------------------------------------------//    

    /// <summary>
    /// NPC�� ���� dialogue ��� �Ҹ� ����
    /// </summary>
    void SetAudio()
    {
        if(npcRole != null)
        {
            switch (npcRole.currentCharacter.ToString())
            {
                case "Nason":
                    SoundManager.Instance.ChangeTextAudioClip(typeSounds[0]);                    
                    break;
                case "Mina":
                    SoundManager.Instance.ChangeTextAudioClip(typeSounds[1]);
                    break;
                case "Jenny":
                    SoundManager.Instance.ChangeTextAudioClip(typeSounds[2]);
                    break;

                default:
                    SoundManager.Instance.ChangeTextAudioClip(typeSounds[0]);
                    break;
            }
        }
        // Intro ����?
        else
        {
            SoundManager.Instance.ChangeTextAudioClip(typeSounds[0]);
        }
    }

    //-------------------------------------------------------------//

    // ��ȭ ���� �Լ�
    public void StartConversation()
    {        
        StartCoroutine(StartConversationCoroutine());
    }

    private IEnumerator StartConversationCoroutine()
    {
        // SwitchCameraWithFade �ڷ�ƾ�� ���� ������ ���
        yield return StartCoroutine(FadeUtility.Instance.SwitchCameraWithFade(uIManager.screen, cameraManager, player, npcRole));

        // �ڷ�ƾ�� ���� �Ŀ� ��ȭ UI Ȱ��ȭ
        uIManager.SetConversationUI(true);
        isTalking = true;        

        // ������ �۾� ����
        uIManager.SetSpeakingUI();  // �ε� �̹��� ��Ȱ��ȭ
        SetAudio();
    }


    // ��ȭ ���� �Լ� - ��ư�� ����
    public void EndConversation()
    {
        isTalking = false;
        StartCoroutine(EndConversationCoroutine());
    }

    private IEnumerator EndConversationCoroutine()
    {
        uIManager.SetNullInputField();
        uIManager.SetConversationUI(false);
        uIManager.SetBlankAnswerText();

        // SwitchCameraWithFade �ڷ�ƾ�� �Ϸ�� ������ ���
        yield return StartCoroutine(FadeUtility.Instance.SwitchCameraWithFade(uIManager.screen, cameraManager, player, npcRole));

        // �ڷ�ƾ�� ���� �Ŀ� ������ �ڵ�
        player.GetComponent<Player>().UnactivateIsTalking();
        RemoveNPCRole();
    }
}
