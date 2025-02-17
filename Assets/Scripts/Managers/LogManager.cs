using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogManager : MonoBehaviour
{
    public Image logPage;    

    Text statement;
    [SerializeField] Transform statementButtonsParent;
    [SerializeField] Transform npcLogScrollRectsParent;

    [SerializeField] Button[] statementButtons;
    [SerializeField] ScrollRect[] npcLogScrollRects;
    [SerializeField] Button exitLogButton;

    float space = 80f;

    [SerializeField] GameObject log; // Resources 폴더에 존재
    List<RectTransform> nasonLogs = new List<RectTransform>();
    List<RectTransform> jennyLogs = new List<RectTransform>();
    List<RectTransform> minaLogs = new List<RectTransform>();

    void Awake()
    {
        log = Resources.Load<GameObject>("Log");

        SetUI();
    }

    void SetUI()
    {
        statement = logPage.transform.GetChild(0).GetComponent<Text>();
        statementButtonsParent = logPage.transform.GetChild(1);
        npcLogScrollRectsParent = logPage.transform.GetChild(2);

        logPage.gameObject.SetActive(false);
        statement.gameObject.SetActive(false);

        statementButtons = new Button[statementButtonsParent.childCount - 1];
        npcLogScrollRects = new ScrollRect[npcLogScrollRectsParent.childCount - 1];

        for (int i = 0; i < statementButtonsParent.childCount - 1; i++)
        {            
            statementButtons[i] = statementButtonsParent.GetChild(i).GetComponent<Button>();
            statementButtons[i].gameObject.SetActive(true);
        }
        
        for (int i = 0; i < npcLogScrollRectsParent.childCount - 1; i++)
        {
            npcLogScrollRects[i] = npcLogScrollRectsParent.GetChild(i).GetComponent<ScrollRect>();
            npcLogScrollRects[i].gameObject.SetActive(false);            
        }

        exitLogButton = npcLogScrollRectsParent.GetChild(npcLogScrollRectsParent.childCount - 1).GetComponent<Button>();

        statementButtonsParent.gameObject.SetActive(false);
        npcLogScrollRectsParent.gameObject.SetActive(false);
    }

    // 로그 관련 함수

    // 로그 페이지 열기 - ESC에서 열 수 있도록 하기
    public void OpenLogPage()
    {
        logPage.gameObject.SetActive(true);
        statement.gameObject.SetActive(true);
        statementButtonsParent.gameObject.SetActive(true);
    }

    public void CloseLogPage()
    {
        logPage.gameObject.SetActive(false);
    }

    public void ClickNasonLogButton()
    {
        statement.gameObject.SetActive(false);
        statementButtonsParent.gameObject.SetActive(false);

        npcLogScrollRectsParent.gameObject.SetActive(true);
        npcLogScrollRects[0].gameObject.SetActive(true);
        npcLogScrollRects[0].content.anchoredPosition = new Vector2(0, -100);
        exitLogButton.gameObject.SetActive(true);
        SetAllChildrenActive(npcLogScrollRects[0].gameObject);
    }

    public void ClickJennyLogButton()
    {
        statement.gameObject.SetActive(false);
        statementButtonsParent.gameObject.SetActive(false);
        
        npcLogScrollRectsParent.gameObject.SetActive(true);
        npcLogScrollRects[1].gameObject.SetActive(true);
        npcLogScrollRects[1].content.anchoredPosition = new Vector2(0, -100);
        exitLogButton.gameObject.SetActive(true);
        SetAllChildrenActive(npcLogScrollRects[1].gameObject);
    }

    public void ClickMinaLogButton()
    {
        statement.gameObject.SetActive(false);
        statementButtonsParent.gameObject.SetActive(false);
        
        npcLogScrollRectsParent.gameObject.SetActive(true);
        npcLogScrollRects[2].gameObject.SetActive(true);
        npcLogScrollRects[2].content.anchoredPosition = new Vector2(0, -100);
        exitLogButton.gameObject.SetActive(true);
        SetAllChildrenActive(npcLogScrollRects[2].gameObject);
    }

    void SetAllChildrenActive(GameObject parentObject)
    {
        parentObject.gameObject.SetActive(true);

        // 자식 오브젝트들을 순회하며 재귀적으로 활성화
        foreach (Transform child in parentObject.transform)
        {
            SetAllChildrenActive(child.gameObject);
        }
    }
    
    public void CloseNPCLogScrollRect()
    {
        statement.gameObject.SetActive(true);
        statementButtonsParent.gameObject.SetActive(true);
        exitLogButton.gameObject.SetActive(false);

        foreach (var scrollRects in npcLogScrollRects)
        {
            scrollRects.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// NPC에 따라 질문 및 답변 내용 기록
    /// </summary>
    /// <param NPC="npc"></param>
    /// <param 질문="question"></param>
    /// <param ChatGPT 대답="answer"></param>
    public void AddLog(NPCRole npc, string question, string answer)
    {
        // 캐릭터에 따른 로그 리스트 및 ScrollRect 인덱스 설정
        List<RectTransform> targetLogs = null;
        int scrollRectIndex = -1;

        switch (npc.currentCharacter)
        {
            case NPCRole.Character.Nason:
                targetLogs = nasonLogs;
                scrollRectIndex = 0;
                break;

            case NPCRole.Character.Jenny:
                targetLogs = jennyLogs;
                scrollRectIndex = 1;
                break;

            case NPCRole.Character.Mina:
                targetLogs = minaLogs;
                scrollRectIndex = 2;
                break;
        }

        // 공통 작업: 로그 추가 및 위치 설정
        if (targetLogs != null && scrollRectIndex != -1)
        {
            var logRectTrasnform = Instantiate(log, npcLogScrollRects[scrollRectIndex].content).GetComponent<RectTransform>();            
            
            Log newLog = logRectTrasnform.GetComponent<Log>();

            newLog.SetComponent();
            newLog.SetAnchor();

            newLog.SetQuestion(question);
            newLog.SetAnswer(answer);

            targetLogs.Add(logRectTrasnform);
            UpdateLogPositions(targetLogs, npcLogScrollRects[scrollRectIndex]);
        }
    }

    void UpdateLogPositions(List<RectTransform> logs, ScrollRect scrollRect)
    {
        float y = 50f;        

        for (int i = 0; i < logs.Count; i++)
        {
            logs[i].anchoredPosition = new Vector2(0f, -y);
            y += logs[i].sizeDelta.y + space;
        }

        // ScrollRect content 크기 업데이트
        scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, y);
    }

}
