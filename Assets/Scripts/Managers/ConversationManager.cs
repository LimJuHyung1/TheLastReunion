using OpenAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class ConversationManager : MonoBehaviour
{
    public class OnResponse : UnityEvent<string> { }

    public AudioClip[] typeSounds;
    public GameObject player;

    public CameraManager cameraManager;
    public LogManager logManager;
    public SpawnManager spawnManager;
    public UIManager uIManager;

    private bool isTalking = false;
    private bool isAbleToGoNext = false;

    private Coroutine displayCoroutine;
    private Queue<string> sentencesQueue = new Queue<string>();

    [SerializeField] NPCRole npcRole;

    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.SetNullAudioMixerGroup();
    }

    /// <summary>
    /// Player 스크립트에서 접촉 npc 반환 받음
    /// </summary>
    /// <param NPC Object="col"></param>    
    public void GetNPCRole(NPCRole npc)
    {
        npcRole = npc;
    }

    public void AddListenersResponse()
    {
        if (npcRole != null)
        {
            uIManager.OnEndEditAskField(npcRole.GetResponse);
            uIManager.OnEndEditAskField(uIManager.SetNullInputField);
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
                    uIManager.ChangeNPCName("네이슨");
                    break;
                case "Mina":
                    uIManager.ChangeNPCName("미나");
                    break;
                case "Jenny":
                    uIManager.ChangeNPCName("제니");
                    break;

                default:
                    npcRole.currentCharacter.ToString(npcRole.currentCharacter.ToString());
                    break;
            }
        }
        else
            Debug.LogError("ShowName - npc가 없습니다!");
    }

    public void ShowAnswer(string answer)
    {
        if (npcRole != null)
        {
            logManager.AddLog(npcRole, uIManager.GetQuestion(), answer);
            uIManager.SetInteractableAskField(false);
            uIManager.SetActiveEndConversationButton(false);
            SetAudio();

            sentencesQueue.Clear();

            // 정규식을 이용해 '.', '?', '!'로 분할하면서 "..." 예외 처리
            string[] sentences = System.Text.RegularExpressions.Regex.Split(answer, @"(?<=[^\.]\.|[!?])");

            foreach (string part in sentences)
            {
                if (!string.IsNullOrWhiteSpace(part)) // 공백이 아닌 유효한 문자열만 처리
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
            Debug.LogError("npc가 없습니다!");
        }
    }

    /// <summary>
    /// NPC의 답변을 한문장 씩 화면에 출력
    /// </summary>
    /// <returns></returns>
    private IEnumerator DisplaySentences()
    {
        // NPC가 대화 중일 경우 실행
        if (isTalking)
        {
            do
            {
                // 대사 큐에서 다음 문장을 가져옴
                string sentence = sentencesQueue.Dequeue();
                isAbleToGoNext = false; // 다음 문장으로 넘어갈 수 없도록 설정
                uIManager.IsReadyToSkip = true;

                // NPC 감정 표현 실행 (예: 애니메이션, 표정 변화 등)
                ChatMessage message = new ChatMessage { Content = sentence };
                npcRole.PlayEmotion(message);

                // UI에서 텍스트를 한 글자씩 출력하는 코루틴 실행
                yield return StartCoroutine(uIManager.ShowLine(uIManager.GetNPCAnswer(), sentence));

                // 다음 문장으로 넘어가기 전에 사용자의 입력을 기다림
                yield return new WaitUntil(() => isAbleToGoNext);

                // 대사 큐에 문장이 남아 있고, 다음 문장으로 넘어갈 수 있는 상태라면 실행
                if (sentencesQueue.Count > 0 && isAbleToGoNext)
                {
                    // 마우스 클릭 / 스페이스 바 / 엔터 키 입력을 기다림
                    yield return new WaitUntil(() => Input.GetMouseButtonDown(0)
                    || Input.GetKeyDown(KeyCode.Space)
                    || Input.GetKeyDown(KeyCode.Return));

                    // 대사 스킵 여부를 변경 (사용자가 다음 문장을 빠르게 넘길 수 있도록 설정)
                    uIManager.ChangeIsSkipping(true);
                }
            }
            while (sentencesQueue.Count > 0); // 모든 문장을 출력할 때까지 반복
        }

        uIManager.IsReadyToSkip = false;

        // 대화가 끝났으므로 "대화 종료" 버튼을 활성화
        uIManager.SetActiveEndConversationButton(true);

        // 플레이어가 질문 입력 필드를 다시 사용할 수 있도록 설정
        uIManager.SetInteractableAskField(true);

        // 대사 스킵 가능 여부를 다시 설정
        uIManager.ChangeIsSkipping(true);

        // 플레이어가 질문을 입력할 수 있도록 입력 필드에 포커스
        uIManager.FocusOnAskField();

        // 현재 실행 중인 코루틴을 초기화하여 다음 실행을 준비
        displayCoroutine = null;
    }

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
        if (npcRole != null)
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
        // Intro 시작?
        else
        {
            SoundManager.Instance.ChangeTextAudioClip(typeSounds[0]);
        }
    }

    //-------------------------------------------------------------//

    // 대화 시작 함수
    public void StartConversation()
    {
        StartCoroutine(StartConversationCoroutine());
    }

    private IEnumerator StartConversationCoroutine()
    {
        // SwitchCameraWithFade 코루틴이 끝날 때까지 대기
        yield return StartCoroutine(FadeUtility.Instance.SwitchCameraWithFade(uIManager.screen, cameraManager, player, npcRole));

        // 코루틴이 끝난 후에 대화 UI 활성화
        uIManager.SetConversationUI(true);
        isTalking = true;

        // 나머지 작업 수행
        uIManager.SetSpeakingUI();  // 로딩 이미지 비활성화
        SetAudio();
    }


    // 대화 종료 함수 - 버튼에 적용
    public void EndConversation()
    {
        isTalking = false;
        StartCoroutine(EndConversationCoroutine());
    }

    private IEnumerator EndConversationCoroutine()
    {
        uIManager.SetNullInputField();
        uIManager.SetConversationUI(false);
        uIManager.RemoveOnEndEditListener();
        uIManager.SetBlankAnswerText();

        // SwitchCameraWithFade 코루틴이 완료될 때까지 대기
        yield return StartCoroutine(FadeUtility.Instance.SwitchCameraWithFade
            (uIManager.screen, cameraManager, player, npcRole, spawnManager));

        // 코루틴이 끝난 후에 실행할 코드
        player.GetComponent<Player>().UnactivateIsTalking();
        RemoveNPCRole();
    }

    public bool GetIsTalking()
    {
        return isTalking;
    }
}