using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenAI;
using static Evidence;


public class EvidenceManager : MonoBehaviour
{    
    public ScrollRect evidenceScrollRect;
    public SpawnManager spawnManager;
    public Image evidencePage;

    GameObject evidenceButton; // Resources 폴더에 존재
    GameObject evidenceIntroductionPage; // Resources 폴더에 존재
    [SerializeField] private List<RectTransform> findedEvidenceRectTransformList = new List<RectTransform>();
    [SerializeField] private List<Image> evidenceIntroductionList = new List<Image>();
    [SerializeField] private List<Evidence> evidences = new List<Evidence>();
    [SerializeField] private Queue<Evidence> evidenceQueue = new Queue<Evidence>();

    private NPCRole[] npcRole = new NPCRole[3];

    private const int totalEvidenceLength = 11;      // 증거 개수
    private int currentEvidenceLength = 0;
    private float space = 40f;

    void Start()
    {        
        evidenceButton = Resources.Load<GameObject>("EvidenceButton");
        evidenceIntroductionPage = Resources.Load<GameObject>("EvidenceIntroductionPage");
    }

    //----------------------------------------------------------//

    public void ReceiveNPCRole(NPCRole[] npcs)
    {
        npcRole = npcs;
    }

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
            GetEvidenceFoundAt(evidenceName),
            GetEvidenceRelationship(evidenceName),            
            GetEvidenceNotes(evidenceName));
        evidences.Add(evidence);
    }



    public void FindEvidence(Evidence evidence)
    {
        SendEvidenceInfo(evidence);
        UpdateEvidencePage(evidence);

        int targetLayer = LayerMask.NameToLayer("House");
        SetLayerRecursively(evidence.gameObject, targetLayer);
    }

    private void UpdateEvidencePage(Evidence evidence)
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
        SetTextInChild(evidencePageInstance.transform.GetChild(2), "연관인물 : " + GetEvidenceRelationship(evidenceName));
        SetTextInChild(evidencePageInstance.transform.GetChild(3), "추가 힌트 : " + GetEvidenceNotes(evidenceName));
        SetRawImage(evidencePageInstance.transform.GetChild(4), GetEvidenceRenderTexture(evidenceName));
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

    private void SetRawImage(Transform childTransform, RenderTexture evidenceRenderTexture)
    {
        if (evidenceRenderTexture != null)
        {
            childTransform.GetComponent<RawImage>().texture = evidenceRenderTexture;
        }
        else Debug.Log("texture 없음");
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


    // 재귀적으로 레이어를 설정하는 함수
    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    //----------------------------------------------------------//

    // npc들에게 증거에 대한 정보 전달
    public void SendEvidenceInfo(Evidence evidence)
    {
        if (JsonManager.evidenceInfoList != null)
        {
            string evidenceName;
            string evidenceInformation;
            string evidenceFoundAt;
            string evidenceRealtionship;
            string evidenceNotes;

            string extraInformation;

            for (int i = 0; i < npcRole.Length; i++)
            {
                ChatMessage evidenceMessage = new ChatMessage();
                evidenceMessage.Role = "system";

                evidenceName = GetEvidenceName(evidence.GetName());
                evidenceInformation = GetEvidenceInformation(evidence.GetName());
                evidenceFoundAt = GetEvidenceFoundAt(evidence.GetName());
                evidenceRealtionship = GetEvidenceRelationship(evidence.GetName());
                evidenceNotes = GetEvidenceNotes(evidence.GetName());
                extraInformation = GetEvidenceExtraInformation(evidence.GetName(), npcRole[i]);
                string completeEvidenceMessage =
                "지금 플레이어가 발견한 증거에 대한 정보를 알려줄께.\n" +
                $"증거 이름 : {evidenceName}.\n" +
                $"증거 내용 : {evidenceInformation}.\n" +
                $"증거가 발견된 장소 : {evidenceFoundAt}.\n" +
                $"증거와 관련된 주요 인물 : {evidenceRealtionship}.\n" +
                $"추가적으로 필요한 증거나 단서 : {evidenceNotes}.\n" +
                $"증거가 {npcRole[i].currentCharacter.ToString()}와 관련된 사실 : {extraInformation}.\n";

                evidenceMessage.Content = completeEvidenceMessage;
                npcRole[i].AddMessage(evidenceMessage);
            }
        }
    }

    //----------------------------------------------------------//

    // 특정 증거의 EvidenceInfo를 반환하는 메서드
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

    public string GetEvidenceFoundAt(string evidenceName)
    {
        EvidenceInfo evidence = GetEvidenceByName(evidenceName);
        return evidence?.foundAt;
    }

    public string GetEvidenceRelationship(string evidenceName)
    {
        EvidenceInfo evidence = GetEvidenceByName(evidenceName);
        return evidence?.relationship;
    }

    public string GetEvidenceNotes(string evidenceName)
    {
        EvidenceInfo evidence = GetEvidenceByName(evidenceName);
        return evidence?.notes;
    }


    public string GetEvidenceExtraInformation(string evidenceName, NPCRole npc)
    {
        EvidenceInfo evidence = GetEvidenceByName(evidenceName);

        switch (npc.currentCharacter.ToString())
        {
            case "Nason":
                return evidence?.nasonExtraInformation;
            case "Jenny":
                return evidence?.jennyExtraInformation;
            case "Mina":
                return evidence?.minaExtraInformation;

            default:
                return null;
        }
    }

    public RenderTexture GetEvidenceRenderTexture(string evidenceName)
    {
        EvidenceInfo evidence = GetEvidenceByName(evidenceName);        
        RenderTexture renderTexture = Resources.Load<RenderTexture>($"{evidence.renderTexturePath}");

        return renderTexture;
    }
}