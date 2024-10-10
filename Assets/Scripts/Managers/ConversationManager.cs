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
    /// Player 스크립트에서 접촉 npc 반환 받음
    /// </summary>
    /// <param NPC Object="col"></param>    
    public void GetNPCRole(NPCRole npc)
    {
        npcRole = npc;        
    }

    /// <summary>
    /// 임시로 버튼을 통해 npc 답변 받아냄
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
                    uIManager.ChangeNPCName("네이슨");
                    break;
                case "Mina":
                    uIManager.ChangeNPCName("미나");
                    break;
                case "Jenny":
                    uIManager.ChangeNPCName("제니");
                    break;
                case "Police":
                    uIManager.ChangeNPCName("경찰");
                    break;

                default:
                    npcRole.currentCharacter.ToString(npcRole.currentCharacter.ToString());
                    break;
            }
        }        
        else
            Debug.LogError("ShowName - npc가 없습니다!");
    }

    /// <summary>
    /// NPC 스크립트에서 실행됨
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

            // 정규식을 이용해 문장을 '.', '?', '!' 문자로 분할하면서 해당 문자 유지
            string[] sentences = System.Text.RegularExpressions.Regex.Split(answer, @"(?<=[.!?])");

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
        do
        {
            string sentence = sentencesQueue.Dequeue();
            isAbleToGoNext = false; // 다음 문장으로 넘어갈 수 없도록 설정

            ChatMessage message = new ChatMessage { Content = sentence };
            npcRole.PlayEmotion(message);

            yield return StartCoroutine(uIManager.ShowLine(uIManager.GetNPCAnswer(), sentence)); // 텍스트 표시 코루틴 실행

            // 마우스 클릭을 기다리기 전에 다음 문장으로 넘어갈 수 있도록 대기
            yield return new WaitUntil(() => isAbleToGoNext);

            if (sentencesQueue.Count > 0 && isAbleToGoNext)
            {
                yield return new WaitUntil(() => Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)); // 마우스 클릭 또는 엔터키 대기
                uIManager.ChangeIsSkipping(true);
            }
        }
        while (sentencesQueue.Count > 0);

        uIManager.SetInteractableAskField(true);
        uIManager.ChangeIsSkipping(true);
        displayCoroutine = null; // 코루틴이 끝나면 변수 초기화
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
        uIManager.SetBlankAnswerText();

        // SwitchCameraWithFade 코루틴이 완료될 때까지 대기
        yield return StartCoroutine(FadeUtility.Instance.SwitchCameraWithFade(uIManager.screen, cameraManager, player, npcRole));

        // 코루틴이 끝난 후에 실행할 코드
        player.GetComponent<Player>().UnactivateIsTalking();
        RemoveNPCRole();
    }
}
