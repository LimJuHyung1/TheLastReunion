using Newtonsoft.Json.Linq;
using OpenAI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;


public class ConversationManager : MonoBehaviour
{
    public class OnResponse : UnityEvent<string> { }

    [Header("Dialogue Sounds")]
    public AudioClip[] typeSounds;

    [Header("Player")]
    public GameObject player;

    [Header("Managers")]
    public CameraManager cameraManager;
    public GameManager gameManager;
    public SpawnManager spawnManager;

    private bool isTalking = false;
    private bool isAbleToGoNext = false;

    private Coroutine displayCoroutine;
    private Queue<string> sentencesQueue = new Queue<string>();

    private NPCRole npcRole;

    // NPC �̸� ����
    private static readonly Dictionary<NPCRole.Character, string> npcNameMap = new Dictionary<NPCRole.Character, string>
    {
        { NPCRole.Character.Nason, "���̽�" },
        { NPCRole.Character.Mina, "�̳�" },
        { NPCRole.Character.Jenny, "����" }
    };

    // NPC�� ��� �Ҹ� ����
    private static readonly Dictionary<NPCRole.Character, AudioClip> npcAudioMap = new Dictionary<NPCRole.Character, AudioClip>();



    void Awake()
    {
        // NPC�� ��� �Ҹ� ����
        npcAudioMap[NPCRole.Character.Nason] = typeSounds[0];
        npcAudioMap[NPCRole.Character.Mina] = typeSounds[1];
        npcAudioMap[NPCRole.Character.Jenny] = typeSounds[2];
    }

    // Start is called before the first frame update
    void Start()
    {
        // ���� ���� �ʱ�ȭ
        SoundManager.Instance.SetNullAudioMixerGroup();
    }




    /// <summary>
    /// Player ��ũ��Ʈ���� ���� ��ȭ�ϴ� NPC�� �����ϴ� �޼���
    /// </summary>    
    public void GetNPCRole(NPCRole npc)
    {
        npcRole = npc;
    }

    /// <summary>
    /// InputField���� ����Ǵ� �̺�Ʈ ������ ���
    /// </summary>
    public void AddListenersResponse()
    {
        if (npcRole != null)
        {
            gameManager.uIManager.OnEndEditAskField(npcRole.GetResponse);
            gameManager.uIManager.OnEndEditAskField(gameManager.uIManager.SetNullInputField);
        }
    }

    /// <summary>
    /// �������� NPC ���� (��ȭ�� ����� �� ȣ���)
    /// </summary>
    public void RemoveNPCRole()
    {
        npcRole = null;
    }

    //-------------------------------------------------------------//

    /// <summary>
    /// NPC �̸��� UI�� ǥ��
    /// </summary>
    public void ShowName()
    {
        // TryGetValue - Ű�� �����ϸ� true, �׷��� ������ false ��ȯ
        // (out Ű���带 ���� key ���� �����Ѵٸ� value ���� ��ȯ��)
        if (npcRole != null && npcNameMap.TryGetValue(npcRole.currentCharacter, out string npcName))
        {
            gameManager.uIManager.ChangeNPCName(npcName);
        }
        else
        {
            Debug.LogError("ShowName - NPC�� ���ų� ���ε��� �ʾҽ��ϴ�!");
        }
    }

    /// <summary>
    /// NPC�� �亯�� �޾� ȭ�鿡 ��� (���� ������ ������ ��� ť�� ����)
    /// </summary>
    public void ShowAnswer(string answer)
    {
        if (npcRole == null)
        {
            Debug.LogError("NPC�� �����ϴ�!");
            return;
        }

        int interrogation_pressure = 0;
        string responseText = "";

        try
        {
            JObject json = JObject.Parse(answer); // JSON �Ľ�
            interrogation_pressure = int.Parse(json["interrogation_pressure"].ToString());
            responseText = json["response"]?.ToString();

            if (string.IsNullOrWhiteSpace(responseText))
            {
                Debug.LogWarning("response �׸��� ����ֽ��ϴ�.");
                return;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"JSON �Ľ� �� ���� �߻�: {ex.Message}");
            return;
        }

        gameManager.logManager.AddLog(npcRole, gameManager.uIManager.GetQuestion(), responseText);
        gameManager.uIManager.SetInteractableAskField(false);
        gameManager.uIManager.SetActiveEndConversationButton(false);
        SetAudio();

        sentencesQueue.Clear();

        // '.', '?', '!' �������� ���� �����ϵ� "..."�� ���� ó��        
        string[] sentences = Regex.Split(responseText, @"(?<!(\.{2,}))(?<=[.!?])\s+");

        foreach (string part in sentences)
        {
            if (!string.IsNullOrWhiteSpace(part))
            {
                string trimmedSentence = part.Trim();
                sentencesQueue.Enqueue(trimmedSentence);
            }
        }

        if (displayCoroutine == null)
        {
            displayCoroutine = StartCoroutine(DisplaySentences(interrogation_pressure));
        }
    }

    /// <summary>
    /// NPC�� �亯�� �ѹ��� �� ȭ�鿡 ���
    /// </summary>
    private IEnumerator DisplaySentences(int interrogation_pressure)
    {
        if (!isTalking) yield break;

        var uiManager = gameManager.uIManager; // ������ ���� ������ ����

        while (sentencesQueue.Count > 0)
        {
            string sentence = sentencesQueue.Dequeue();
            isAbleToGoNext = false;
            uiManager.IsReadyToSkip = true;

            ChatMessage message = new ChatMessage { Content = sentence };
            npcRole.PlayEmotion(message);

            yield return StartCoroutine(uiManager.ShowLine(uiManager.GetNPCAnswer(), sentence, interrogation_pressure));

            yield return new WaitUntil(() => isAbleToGoNext);

            if (sentencesQueue.Count > 0)
            {
                yield return new WaitUntil(() => Input.GetMouseButtonDown(0)
                || Input.GetKeyDown(KeyCode.Space)
                || Input.GetKeyDown(KeyCode.Return));

                uiManager.ChangeIsSkipping(true);
            }
        }

        gameManager.uIManager.IsReadyToSkip = false;

        // ��ȭ�� �������Ƿ� "��ȭ ����" ��ư�� Ȱ��ȭ
        gameManager.uIManager.SetActiveEndConversationButton(true);

        // �÷��̾ ���� �Է� �ʵ带 �ٽ� ����� �� �ֵ��� ����
        gameManager.uIManager.SetInteractableAskField(true);

        // ��� ��ŵ ���� ���θ� �ٽ� ����
        gameManager.uIManager.ChangeIsSkipping(true);

        // �÷��̾ ������ �Է��� �� �ֵ��� �Է� �ʵ忡 ��Ŀ��
        gameManager.uIManager.FocusOnAskField();

        // ���� ���� ���� �ڷ�ƾ�� �ʱ�ȭ�Ͽ� ���� ������ �غ�
        displayCoroutine = null;
    }

    /// <summary>
    /// ���� �������� �Ѿ �� �ֵ��� ���� ����
    /// </summary>
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
        if (npcRole != null && npcAudioMap.TryGetValue(npcRole.currentCharacter, out AudioClip clip))
        {
            SoundManager.Instance.ChangeTextAudioClip(clip);
        }
        else
        {
            SoundManager.Instance.ChangeTextAudioClip(typeSounds[0]); // �⺻��
        }
    }

    //-------------------------------------------------------------//

    /// <summary>
    /// ��ȭ�� �����ϴ� �޼��� (���콺 Ŭ�� �� ����)
    /// </summary>
    public void StartConversation()
    {
        StartCoroutine(StartConversationCoroutine());
    }

    private IEnumerator StartConversationCoroutine()
    {
        yield return FadeUtility.Instance.SwitchCameraWithFade(gameManager.uIManager.screen, cameraManager, player, npcRole);

        gameManager.uIManager.SetConversationUI(true);
        isTalking = true;
        gameManager.uIManager.SetSpeakingUI();
        SetAudio();
    }


    /// <summary>
    /// ��ȭ�� �����ϴ� �޼��� (NPC�� ��ȭ ���� �� ����)
    /// </summary>
    public void EndConversation()
    {
        isTalking = false;
        StartCoroutine(EndConversationCoroutine());
    }

    private IEnumerator EndConversationCoroutine()
    {
        gameManager.uIManager.SetNullInputField();
        gameManager.uIManager.SetConversationUI(false);
        gameManager.uIManager.RemoveOnEndEditListener();
        gameManager.uIManager.SetBlankAnswerText();

        yield return FadeUtility.Instance.SwitchCameraWithFade(
            gameManager.uIManager.screen, cameraManager, player, npcRole, spawnManager);

        player.GetComponent<Player>().UnactivateIsTalking();
        RemoveNPCRole();
    }

    /// <summary>
    /// ��ȭ ������ ���� ��ȯ
    /// </summary>
    public bool GetIsTalking()
    {
        return isTalking;
    }
}