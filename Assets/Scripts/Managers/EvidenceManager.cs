using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenAI;


public class EvidenceManager : MonoBehaviour
{
    public NPCRole[] npcRole = new NPCRole[3];
    public ScrollRect evidenceScrollRect;
    public Image evidencePage;

    [SerializeField] GameObject evidenceButton; // Resources 폴더에 존재
    [SerializeField] GameObject evidenceIntroductionPage; // Resources 폴더에 존재
    [SerializeField] private List<RectTransform> findedEvidenceRectTransformList = new List<RectTransform>();
    [SerializeField] private List<Image> evidenceIntroductionList = new List<Image>();
    [SerializeField] private List<Evidence> evidences = new List<Evidence>();
    [SerializeField] private Queue<Evidence> evidenceQueue = new Queue<Evidence>();

    private const int totalEvidenceLength = 9;      // 나중에 9로 하기(증거 개수)
    private int currentEvidenceLength = 0;
    private float space = 40f;

    void Start()
    {
        SendEvidenceInfo();
        evidenceButton = Resources.Load<GameObject>("EvidenceButton");
        evidenceIntroductionPage = Resources.Load<GameObject>("EvidenceIntroductionPage");
    }

    //----------------------------------------------------------//

    // 증거 초기화
    public void SetEvidence(Evidence evidence)
    {
        evidenceQueue.Enqueue(evidence);
        currentEvidenceLength++;

        if (currentEvidenceLength == totalEvidenceLength)
        {
            DequeueAllEvidence();
        }
    }

    private void DequeueAllEvidence()
    {
        // 큐에 남아 있는 모든 증거 초기화
        while (evidenceQueue.Count > 0)
        {
            InitializeEvidence(evidenceQueue.Dequeue());
        }
    }

    private void InitializeEvidence(Evidence evidence)
    {
        string evidenceName = evidence.GetName();
        evidence.Initialize
            (GetEvidenceName(evidenceName),
            GetEvidenceDescription(evidenceName),
            GetEvidenceInformation(evidenceName),
            GetEvidenceRelationship(evidenceName),
            GetEvidenceInformation(evidenceName),
            GetEvidenceNotes(evidenceName));
        evidences.Add(evidence);
    }



    public void FindEvidence(Evidence evidence)
    {
        // 증거 버튼 생성 및 설정
        EvidenceButton evidenceButtonInstance = Instantiate(evidenceButton, evidenceScrollRect.content).GetComponent<EvidenceButton>();
        findedEvidenceRectTransformList.Add(evidenceButtonInstance.GetComponent<RectTransform>());

        // 증거 소개 페이지 생성 및 설정
        GameObject evidencePageInstance = Instantiate(evidenceIntroductionPage, evidencePage.transform);
        evidencePageInstance.SetActive(false);
        
        string evidenceName = evidence.GetName();

        // Text 컴포넌트를 효율적으로 설정하는 헬퍼 함수 호출
        SetTextInChild(evidencePageInstance.transform.GetChild(0), GetEvidenceName(evidenceName));
        SetTextInChild(evidencePageInstance.transform.GetChild(1), GetEvidenceInformation(evidenceName));
        SetTextInChild(evidencePageInstance.transform.GetChild(2), "중요도 : " + GetEvidenceImportance(evidenceName));
        SetTextInChild(evidencePageInstance.transform.GetChild(3), "연관인물 : " + GetEvidenceRelationship(evidenceName));
        SetTextInChild(evidencePageInstance.transform.GetChild(4), "추가 힌트 : " + GetEvidenceNotes(evidenceName));

        evidenceButtonInstance.SetAnchor();
        evidenceButtonInstance.SetText(evidence);
        evidenceButtonInstance.GetComponent<Button>().onClick.AddListener(SetActiveFalseAllIntroductions);
        evidenceButtonInstance.SetEvidenceIntroduction(evidencePageInstance.GetComponent<Image>());
        evidenceIntroductionList.Add(evidenceButtonInstance.GetEvidenceIntroductionPage());

        // 버튼 위치 업데이트
        UpdateEvidenceButtonPositions(findedEvidenceRectTransformList, evidenceScrollRect);
    }

    // 텍스트 설정을 반복적으로 처리하는 헬퍼 함수
    private void SetTextInChild(Transform childTransform, string text)
    {
        childTransform.GetComponent<Text>().text = text;
    }

    void SetActiveFalseAllIntroductions()
    {
        foreach (var page in evidenceIntroductionList)
        {
            if (page.gameObject.activeSelf)
                page.gameObject.SetActive(false);
        }
    }

    void UpdateEvidenceButtonPositions(List<RectTransform> foundEvidenceList, ScrollRect scrollRect)
    {
        float y = 30f;
        for (int i = 0; i < foundEvidenceList.Count; i++)
        {
            foundEvidenceList[i].anchoredPosition = new Vector2(0f, -y);
            y += foundEvidenceList[i].sizeDelta.y + space;
        }

        // ScrollRect content 크기 업데이트
        scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, y);
    }

    //----------------------------------------------------------//

    // npc들에게 증거에 대한 정보 전달
    private void SendEvidenceInfo()
    {
        if (JsonManager.evidenceInfoList != null)
        {
            string npcName;

            string evidenceName;
            string evidenceInformation;
            string evidenceFoundAt;
            string evidenceRealtionship;
            string evidenceImportance;
            string evidenceNotes;

            string extraInformation;

            for (int i = 0; i < npcRole.Length; i++)
            {
                npcName = npcRole[i].currentCharacter.ToString();
                int tmpIndex = 0;
                foreach (EvidenceInfo ei in JsonManager.evidenceInfoList.evidenceInfoList)
                {
                    ChatMessage evidenceMessage = new ChatMessage();
                    evidenceMessage.Role = "system";

                    evidenceName = ei.name;
                    evidenceInformation = ei.information;
                    evidenceFoundAt = ei.foundAt;
                    evidenceRealtionship = ei.relationship;
                    evidenceImportance = ei.importance;
                    evidenceNotes = ei.notes;

                    switch (npcName)
                    {
                        case "Nason":
                            extraInformation = ei.nasonExtraInformation;
                            break;
                        case "Jenny":
                            extraInformation = ei.jennyExtraInformation;
                            break;
                        case "Mina":
                            extraInformation = ei.minaExtraInformation;
                            break;

                        default:
                            Debug.LogError("3명 npc 외의 다른 이름 참조!");
                            return;
                    }

                    string completeEvidenceMessage =
                        $"{tmpIndex + 1}번째 증거에 대한 정보와, 해당 정보에 대한 캐릭터의 태도 및 의견을 알려줄께.\n" +
                        $"증거 이름 : {evidenceName}.\n" +
                        $"증거 내용 : {evidenceInformation}.\n" +
                        $"증거가 발견된 장소 : {evidenceFoundAt}.\n" +
                        $"증거와 관련된 주요 인물 : {evidenceRealtionship}.\n" +
                        $"해당 증거의 중요도 : {evidenceImportance}.\n" +
                        $"추가적으로 필요한 증거나 단서 : {evidenceNotes}.\n" +
                        $"해당 증거에 대한 {npcName}의 태도 및 의견 : {extraInformation}.\n";

                    evidenceMessage.Content = completeEvidenceMessage;

                    tmpIndex++;
                    npcRole[i].AddMessage(evidenceMessage);
                }
            }
        }
    }

    //----------------------------------------------------------//

    // 특정 증거의 ThingInfo를 반환하는 메서드
    private EvidenceInfo GetEvidenceByName(string evidenceName)
    {
        if (JsonManager.evidenceInfoList != null)
        {
            foreach (EvidenceInfo ei in JsonManager.evidenceInfoList.evidenceInfoList)
            {
                if (ei.name == evidenceName)
                {
                    return ei;
                }
            }
        }

        Debug.LogWarning($"'{evidenceName}' 증거를 찾을 수 없습니다.");
        return null;
    }

    // 특정 증거 이름 반환
    public string GetEvidenceName(string evidenceName)
    {
        EvidenceInfo evidence = GetEvidenceByName(evidenceName);
        return evidence?.name; // null이면 null 반환
    }

    public string GetEvidenceDescription(string evidenceName)
    {
        EvidenceInfo evidence = GetEvidenceByName(evidenceName);
        return evidence?.description;
    }

    public string GetEvidenceInformation(string evidenceName)
    {
        EvidenceInfo evidence = GetEvidenceByName(evidenceName);
        return evidence?.information;
    }

    public string GetEvidenceRelationship(string evidenceName)
    {
        EvidenceInfo evidence = GetEvidenceByName(evidenceName);
        return evidence?.relationship;
    }

    public string GetEvidenceImportance(string evidenceName)
    {
        EvidenceInfo evidence = GetEvidenceByName(evidenceName);
        return evidence?.importance;
    }

    public string GetEvidenceNotes(string evidenceName)
    {
        EvidenceInfo evidence = GetEvidenceByName(evidenceName);
        return evidence?.notes;
    }
}