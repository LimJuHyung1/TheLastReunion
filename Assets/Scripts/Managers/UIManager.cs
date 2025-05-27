using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;
// using static UnityEngine.Rendering.DebugUI;

public class UIManager : MonoBehaviour
{
    // 전역적으로 사용되는 변수들
    public static string tmpAnswer = "";
    public string tmpQuestion = "";
    public static bool isAttachedToEvidence = false; // 증거와의 상호작용 여부

    [Header("Managers")]
    public ConversationManager conversationManager;
    public EvidenceManager evidenceManager;

    [Header("UI")]
    public Button endConversationBtn;  // 대화 종료 버튼
    public GameObject pulsing;  // 특정 이벤트 시 깜빡이는 UI
    public InputField askField;  // 플레이어 질문 입력 필드
    public Image boundaryUI;
    public Image chatBox;
    public Image screen;
    public Image[] keys;  // 게임 내 상호작용 키
    public Text keyDescriptionText;
    public Text thingDescriptionText;
    public Text thingNameText;
    public Timer timer;  // 게임 타이머

    [SerializeField] Text NPCName;
    [SerializeField] Text NPCAnswer;
    [SerializeField] GameObject waitingMark;  // NPC가 발언 준비 중 표시
    [SerializeField] GameObject clickMark;  // 플레이어가 입력 가능할 때 표시

    UIManagerSup sup; // 서브 UI 관리 클래스

    [SerializeField] private bool waitToSkip = true;
    public bool isReadyToSkip = false;  // NPC가 문장 출력 시작 시 true로 변경됨
    public bool IsReadyToSkip
    {
        get { return isReadyToSkip; }
        set { isReadyToSkip = value; }
    }

    [SerializeField] private bool isSkipping = false;
    [SerializeField] private bool isShowingDescription = false; // 코루틴 실행 여부 확인
    public bool IsShowingDescription
    {
        get { return isShowingDescription; }
        set { isShowingDescription = value; }
    }

    void Start()
    {
        SetUI();    // UI 요소 비활성화 및 초기 설정
        sup = new UIManagerSup(keys, keyDescriptionText, evidenceManager);
    }

    /// <summary>
    /// 키 입력 감지 후 대화 스킵 기능 활성화
    /// </summary>
    void OnEnable()
    {
        StartCoroutine(WaitForSkipInput());
    }

    IEnumerator WaitForSkipInput()
    {
        while (true)
        {
            if (isReadyToSkip && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
            {
                isSkipping = true;
            }
            yield return null;
        }
    }


    /// <summary>
    /// 초기 UI 세팅
    /// </summary>
    void SetUI()
    {
        NPCName = chatBox.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>();
        NPCAnswer = chatBox.transform.GetChild(2).GetComponent<UnityEngine.UI.Text>();
        waitingMark = chatBox.transform.GetChild(3).gameObject;
        clickMark = chatBox.transform.GetChild(4).gameObject;

        askField.gameObject.SetActive(false);
        boundaryUI.gameObject.SetActive(false);
        chatBox.gameObject.SetActive(false);
        // cursor.gameObject.SetActive(false);
        endConversationBtn.gameObject.SetActive(false);
        pulsing.gameObject.SetActive(false);

        SetActiveTimer(false);

        // 게임 인트로 부터 실행시킬 때는 true로 설정하기 - UIManager도 같이
        screen.gameObject.SetActive(true);
    }

    //---------------------------------------------------------------//
    // 아래 함수들은 NPC 스크립트에서 호출됨

    // InputField 텍스트 길이 반환
    public int GetAskFieldTextLength()
    {
        return askField.text.Length;
    }

    // InputField 텍스트 반환
    public string GetAskFieldText()
    {
        return askField.text;
    }

    public InputField GetAskField()
    {
        return askField;
    }

    public string GetQuestion()
    {
        return tmpQuestion;
    }

    //---------------------------------------------------------------//
    //ConversationManager에서 호출됨

    /// <summary>
    /// 대화 UI를 활성화/비활성화
    /// </summary>
    public void SetConversationUI(bool b)
    {
        SetActiveUIElements(b, askField.gameObject, chatBox.gameObject, endConversationBtn.gameObject);
    }

    /// <summary>
    /// UI 요소들을 활성화/비활성화하는 헬퍼 메서드
    /// </summary>
    private void SetActiveUIElements(bool isActive, params GameObject[] elements)
    {
        foreach (GameObject element in elements)
        {
            element.SetActive(isActive);
        }
    }

    /// <summary>
    /// NPC 이름 변경
    /// </summary>
    public void ChangeNPCName(string name)
    {
        NPCName.text = name;
    }

    // NPC 답변 반환
    public Text GetNPCAnswer()
    {
        return NPCAnswer;
    }

    /// <summary>
    /// NPC가 말할 때 마커 UI 업데이트
    /// </summary>
    public void SetSpeakingUI()
    {
        waitingMark.gameObject.SetActive(false);
        clickMark.gameObject.SetActive(false);
    }

    /// <summary>
    /// NPC의 발언 상태에 따라 UI 설정
    /// </summary>
    /// <param name="isSpeaking">NPC가 발언 중인지 여부</param>
    public void SetNPCSpeakingUI(bool isSpeaking)
    {
        waitingMark.gameObject.SetActive(isSpeaking);
        clickMark.gameObject.SetActive(!isSpeaking);
    }

    /// <summary>
    /// input field의 질문 칸을 공백으로 설정
    /// </summary>
    public void SetNullInputField()
    {
        askField.text = "";
    }

    public void SetInteractableAskField(bool b)
    {
        askField.interactable = b;
    }

    public void SetActiveEndConversationButton(bool b)
    {
        endConversationBtn.gameObject.SetActive(b);
    }

    /// <summary>
    /// NPC 답변 텍스트를 공백으로 설정
    /// </summary>
    public void SetBlankAnswerText()
    {
        NPCAnswer.text = "";
    }

    /// <summary>
    /// NPC 대화 전 엔터 키 가능하게 세팅
    /// </summary>
    /// <param name="action"></param>
    public void OnEndEditAskField(UnityAction action)
    {
        askField.onEndEdit.AddListener((string text) =>
        {
            if (!string.IsNullOrWhiteSpace(text) && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
            {
                tmpQuestion = text.Trim();
                action();
            }
        });
    }

    public void RemoveOnEndEditListener()
    {
        askField.onEndEdit.RemoveAllListeners();
    }

    public void FocusOnAskField()
    {
        askField.Select();
        askField.ActivateInputField(); // InputField 활성화
    }

    //---------------------------------------------------------------//

    /// <summary>
    /// 대사를 한 글자씩 출력하는 코루틴
    /// </summary>
    public IEnumerator ShowLine(Text t, string answer, int interrogation_pressure, float second = 0.1f, bool isTyping = true)
    {
        t.text = ""; // 텍스트 초기화

        t.color = interrogation_pressure switch
        {
            <= 5 => Color.black,
            <= 8 => Color.yellow,
            _ => Color.red
        };


        Coroutine dialogSoundCoroutine = null;

        SetNPCSpeakingUI(true);
        dialogSoundCoroutine = StartCoroutine(PlayDialogSound());

        yield return new WaitUntil(() => waitToSkip);
        for (int i = 0; i < answer.Length; i++)
        {
            if (isSkipping) // 마우스 왼쪽 클릭 or 엔터키 감지
            {
                isSkipping = false;
                waitToSkip = false;
                t.text = answer; // 전체 텍스트 즉시 표시
                break; // 루프를 중단하고 전체 텍스트를 표시하도록 이동
            }

            t.text += answer[i]; // 한 글자씩 추가
            yield return new WaitForSeconds(second); // 0.02초 대기
        }

        tmpAnswer = t.text;
        isReadyToSkip = false;
        ChangeIsSkipping(false);

        // 코루틴이 실행되었을 경우에만 종료 처리
        if (dialogSoundCoroutine != null)
        {
            SetNPCSpeakingUI(false);
            StopCoroutine(dialogSoundCoroutine); // PlayDialogSound 코루틴 중지
            SoundManager.Instance.StopTextSound();
        }

        conversationManager.IsAbleToGoNextTrue();
    }

    /// <summary>
    /// Funiture, Evidence 설명 ui에 표시
    /// </summary>
    /// <param 증거 설명="description"></param>
    /// <returns></returns>
    public IEnumerator ShowDescription(string description)
    {
        if (isShowingDescription)
        {
            yield break; // 이미 실행 중이면 무시
        }
        
        isShowingDescription = true; // 코루틴 실행 중

        thingDescriptionText.text = "";

        foreach (char letter in description)
        {
            thingDescriptionText.text += letter; // 한 글자씩 추가
            yield return new WaitForSeconds(0.05f);
        }

        // 2초 후에 텍스트 지우기 시작
        StartCoroutine(ClearDescription(2f));
    }

    IEnumerator ClearDescription(float delay)
    {
        // 2초 대기
        yield return new WaitForSeconds(delay);

        isShowingDescription = false; // 코루틴 종료

        // 텍스트 내용 지우기
        thingNameText.text = "";
        thingDescriptionText.text = "";
    }


    public bool GetIsSkipping()
    {
        return waitToSkip;
    }

    public void ChangeIsSkipping(bool b)
    {
        waitToSkip = b;
    }

    IEnumerator PlayDialogSound()
    {
        while (true) // 무한 루프를 사용하여 반복 실행
        {
            SoundManager.Instance.PlayTextSound();
            yield return new WaitForSeconds(0.1f);
        }
    }

    //---------------------------------------------//
    //아래 함수들은 TutorialManager에서 호출

    public void SetActiveCursor(bool isOn)
    {
        // cursor.gameObject.SetActive(isOn);
    }

    public Image GetScreen()
    {
        return this.screen;
    }

    public void SetActiveTimer(bool whether)
    {
        timer.gameObject.SetActive(whether);
    }

    public void BeginCountdown()
    {
        timer.StartTimer();
    }

    //---------------------------------------------//
    // 게임 중 사용할 키 관련 함수

    public void ShowKeyAndDescriontion(GameObject other)
    {
        sup.ShowKeyAndDescriontion(other);
    }

    public void HideKeyAndDescriontion()
    {
        sup.HideKeyAndDescriontion();
    }

    //---------------------------------------------//

    public void ActivateBoundaryUI()
    {
        boundaryUI.gameObject.SetActive(true);
    }

    public void UnactivateBoundaryUI()
    {
        boundaryUI.gameObject.SetActive(false);
    }

    public Image GetBoundaryUI()
    {
        return boundaryUI;
    }




    public GameObject GetPulsing()
    {
        return pulsing;
    }

    public void SetActivePulsing(bool isOn)
    {
        pulsing.gameObject.SetActive(isOn);
    }

    // 프로퍼티 정의
    public bool IsAttachedToEvidenceProperty
    {
        get => isAttachedToEvidence;
        set
        {
            if (isAttachedToEvidence != value) // 값이 변경될 때만 실행
            {
                isAttachedToEvidence = value;
                IsAttachedToEvidenceChanged(); // 특정 함수 호출
            }
        }
    }

    private void IsAttachedToEvidenceChanged()
    {
        if (isAttachedToEvidence) SetActivePulsing(true);
        else SetActivePulsing(false);
    }
}

public class UIManagerSup
{
    // 0 - 증거 수집, 1 - npc 대화, 2 - 문 열기
    List<Image> keys = new List<Image>();
    Text description;

    EvidenceManager evidenceManager;

    string[] actionDescriptions = new string[5];
    string evidenceDescription;
    public UIManagerSup(Image[] keys, Text t, EvidenceManager evidenceManager)
    {
        foreach (Image key in keys)
        {
            this.keys.Add(key);
        }
        this.description = t;
        this.evidenceManager = evidenceManager;

        SetKeyDescriptions();
    }

    public void SetKeyDescriptions()
    {
        actionDescriptions[0] = "네이슨과 대화하다";
        actionDescriptions[1] = "제니와 대화하다";
        actionDescriptions[2] = "미나와 대화하다";
        actionDescriptions[3] = "문을 연다";
        actionDescriptions[4] = "증거를 수집하다";

        foreach (var key in keys)
        {
            key.gameObject.SetActive(false);
        }

        description.gameObject.SetActive(false);
    }

    public void ShowKeyAndDescriontion(GameObject other)
    {
        LayerMask layer = other.gameObject.layer;
        if (IsEvidenceLayer(layer))
        {
            keys[0].gameObject.SetActive(true);

            evidenceDescription = evidenceManager.GetEvidenceName(other.GetComponent<Evidence>().GetName());
            description.text = evidenceDescription;
            description.gameObject.SetActive(true);
        }
        else if (IsNPCLayer(layer))
        {
            keys[0].gameObject.SetActive(true);
            // NPC에 따라 알맞은 설명 보여주기
            string name = other.GetComponent<NPCRole>().currentCharacter.ToString();
            switch (name)
            {
                case "Nason":
                    description.text = actionDescriptions[0];
                    break;
                case "Jenny":
                    description.text = actionDescriptions[1];
                    break;
                case "Mina":
                    description.text = actionDescriptions[2];
                    break;
            }
            description.gameObject.SetActive(true);
        }
        else if (IsDoorLayer(layer))
        {
            if (!other.GetComponent<Door>().IsOpened())
            {
                keys[1].gameObject.SetActive(true);
                description.text = actionDescriptions[3];
                description.gameObject.SetActive(true);
            }
        }
    }

    public void HideKeyAndDescriontion()
    {
        foreach (var key in keys)
        {
            if (key.gameObject.activeSelf)
            {
                key.gameObject.SetActive(false);
                break;
            }
        }

        description.gameObject.SetActive(false);
    }

    bool IsEvidenceLayer(int layer)
    {
        return layer == LayerMask.NameToLayer("Evidence");
    }

    bool IsNPCLayer(int layer)
    {
        return layer == LayerMask.NameToLayer("NPC");
    }

    bool IsDoorLayer(int layer)
    {
        return layer == LayerMask.NameToLayer("Door");
    }
}