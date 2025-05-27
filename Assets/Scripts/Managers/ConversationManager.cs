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

    // NPC 이름 매핑
    private static readonly Dictionary<NPCRole.Character, string> npcNameMap = new Dictionary<NPCRole.Character, string>
    {
        { NPCRole.Character.Nason, "네이슨" },
        { NPCRole.Character.Mina, "미나" },
        { NPCRole.Character.Jenny, "제니" }
    };

    // NPC별 대사 소리 설정
    private static readonly Dictionary<NPCRole.Character, AudioClip> npcAudioMap = new Dictionary<NPCRole.Character, AudioClip>();



    void Awake()
    {
        // NPC별 대사 소리 설정
        npcAudioMap[NPCRole.Character.Nason] = typeSounds[0];
        npcAudioMap[NPCRole.Character.Mina] = typeSounds[1];
        npcAudioMap[NPCRole.Character.Jenny] = typeSounds[2];
    }

    // Start is called before the first frame update
    void Start()
    {
        // 사운드 설정 초기화
        SoundManager.Instance.SetNullAudioMixerGroup();
    }




    /// <summary>
    /// Player 스크립트에서 현재 대화하는 NPC를 설정하는 메서드
    /// </summary>    
    public void GetNPCRole(NPCRole npc)
    {
        npcRole = npc;
    }

    /// <summary>
    /// InputField에서 실행되는 이벤트 리스너 등록
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
    /// 참조중인 NPC 제거 (대화가 종료될 때 호출됨)
    /// </summary>
    public void RemoveNPCRole()
    {
        npcRole = null;
    }

    //-------------------------------------------------------------//

    /// <summary>
    /// NPC 이름을 UI에 표시
    /// </summary>
    public void ShowName()
    {
        // TryGetValue - 키가 존재하면 true, 그렇지 않으면 false 반환
        // (out 키워드를 통해 key 값이 존재한다면 value 값이 반환됨)
        if (npcRole != null && npcNameMap.TryGetValue(npcRole.currentCharacter, out string npcName))
        {
            gameManager.uIManager.ChangeNPCName(npcName);
        }
        else
        {
            Debug.LogError("ShowName - NPC가 없거나 매핑되지 않았습니다!");
        }
    }

    /// <summary>
    /// NPC의 답변을 받아 화면에 출력 (문장 단위로 나누어 대사 큐에 저장)
    /// </summary>
    public void ShowAnswer(string answer)
    {
        if (npcRole == null)
        {
            Debug.LogError("NPC가 없습니다!");
            return;
        }

        int interrogation_pressure = 0;
        string responseText = "";

        try
        {
            JObject json = JObject.Parse(answer); // JSON 파싱
            interrogation_pressure = int.Parse(json["interrogation_pressure"].ToString());
            responseText = json["response"]?.ToString();

            if (string.IsNullOrWhiteSpace(responseText))
            {
                Debug.LogWarning("response 항목이 비어있습니다.");
                return;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"JSON 파싱 중 오류 발생: {ex.Message}");
            return;
        }

        gameManager.logManager.AddLog(npcRole, gameManager.uIManager.GetQuestion(), responseText);
        gameManager.uIManager.SetInteractableAskField(false);
        gameManager.uIManager.SetActiveEndConversationButton(false);
        SetAudio();

        sentencesQueue.Clear();

        // '.', '?', '!' 기준으로 문장 분할하되 "..."은 예외 처리        
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
    /// NPC의 답변을 한문장 씩 화면에 출력
    /// </summary>
    private IEnumerator DisplaySentences(int interrogation_pressure)
    {
        if (!isTalking) yield break;

        var uiManager = gameManager.uIManager; // 참조를 지역 변수로 저장

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

        // 대화가 끝났으므로 "대화 종료" 버튼을 활성화
        gameManager.uIManager.SetActiveEndConversationButton(true);

        // 플레이어가 질문 입력 필드를 다시 사용할 수 있도록 설정
        gameManager.uIManager.SetInteractableAskField(true);

        // 대사 스킵 가능 여부를 다시 설정
        gameManager.uIManager.ChangeIsSkipping(true);

        // 플레이어가 질문을 입력할 수 있도록 입력 필드에 포커스
        gameManager.uIManager.FocusOnAskField();

        // 현재 실행 중인 코루틴을 초기화하여 다음 실행을 준비
        displayCoroutine = null;
    }

    /// <summary>
    /// 다음 문장으로 넘어갈 수 있도록 상태 변경
    /// </summary>
    public void IsAbleToGoNextTrue()
    {
        isAbleToGoNext = true;
    }

    //-------------------------------------------------------------//    

    /// <summary>
    /// NPC에 따라 dialogue 대사 소리 변경
    /// </summary>
    void SetAudio()
    {
        if (npcRole != null && npcAudioMap.TryGetValue(npcRole.currentCharacter, out AudioClip clip))
        {
            SoundManager.Instance.ChangeTextAudioClip(clip);
        }
        else
        {
            SoundManager.Instance.ChangeTextAudioClip(typeSounds[0]); // 기본값
        }
    }

    //-------------------------------------------------------------//

    /// <summary>
    /// 대화를 시작하는 메서드 (마우스 클릭 시 실행)
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
    /// 대화를 종료하는 메서드 (NPC와 대화 종료 시 실행)
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
    /// 대화 중인지 여부 반환
    /// </summary>
    public bool GetIsTalking()
    {
        return isTalking;
    }
}