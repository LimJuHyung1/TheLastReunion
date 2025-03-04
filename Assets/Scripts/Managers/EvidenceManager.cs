using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenAI;
using static Evidence;


public class EvidenceManager : MonoBehaviour
{
    [Header("Evidence UI")]
    public ScrollRect evidenceScrollRect;   // 증거 버튼을 표시하는 스크롤 UI
    public Image evidencePage;              // 증거 소개 페이지 UI

    [Header("Managers")]
    public SpawnManager spawnManager;
        
    GameObject evidenceButton; // Resources 폴더에 존재
    GameObject evidenceIntroductionPage; // Resources 폴더에 존재

    [SerializeField] // 발견한 증거 버튼 위치 리스트
    private List<RectTransform> findedEvidenceRectTransformList = new List<RectTransform>();
    [SerializeField] // 증거 소개 페이지 리스트
    private List<Image> evidenceIntroductionList = new List<Image>();
    [SerializeField] // 증거 리스트
    private List<Evidence> evidences = new List<Evidence>();
    [SerializeField] // 증거 초기화 대기 큐
    private Queue<Evidence> evidenceQueue = new Queue<Evidence>();

    private NPCRole[] npcRole = new NPCRole[3];

    private const int totalEvidenceLength = 11;         // 증거 개수
    private int currentEvidenceLength = 0;              // 현재 발견한 증거 개수
    private float space = 40f;                          // 버튼 간 간격

    void Start()
    {
        // Resources 폴더에서 프리팹 불러오기
        evidenceButton = Resources.Load<GameObject>("EvidenceButton");
        evidenceIntroductionPage = Resources.Load<GameObject>("EvidenceIntroductionPage");
    }

    //----------------------------------------------------------//

    /// <summary>
    /// NPC 정보를 받아 저장
    /// </summary>
    public void ReceiveNPCRole(NPCRole[] npcs)
    {
        npcRole = npcs;
    }

    /// <summary>
    /// 증거 초기화
    /// </summary>
    public void SetEvidence(Evidence evidence)
    {
        evidenceQueue.Enqueue(evidence);
        currentEvidenceLength++;

        if (currentEvidenceLength == totalEvidenceLength)
        {
            DequeueAllEvidence();
        }
    }

    /// <summary>
    /// 큐에 있는 모든 증거를 초기화하는 메서드
    /// </summary>
    private void DequeueAllEvidence()
    {
        // 큐에 남아 있는 모든 증거 초기화
        while (evidenceQueue.Count > 0)
        {
            InitializeEvidence(evidenceQueue.Dequeue());
        }
    }

    /// <summary>
    /// 증거 객체를 초기화하는 메서드
    /// </summary>
    private void InitializeEvidence(Evidence evidence)
    {
        EvidenceInfo evidenceInfo = GetEvidenceByName(evidence.GetName());

        if (evidenceInfo != null)
        {
            evidence.Initialize(
                evidenceInfo.name,
                evidenceInfo.description,
                evidenceInfo.information,
                evidenceInfo.foundAt,
                evidenceInfo.notes);

            evidences.Add(evidence);
        }
    }

    /// <summary>
    /// 증거를 찾았을 때 호출되는 메서드
    /// </summary>
    public void FindEvidence(Evidence evidence)
    {
        SendEvidenceInfo(evidence);
        UpdateEvidencePage(evidence);

        int targetLayer = LayerMask.NameToLayer("House");
        SetLayerRecursively(evidence.gameObject, targetLayer);
    }

    /// <summary>
    /// 증거 페이지를 업데이트하는 메서드
    /// </summary>
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
        SetRawImage(evidencePageInstance.transform.GetChild(0), GetEvidenceRenderTexture(evidenceName));
        SetTextInChild(evidencePageInstance.transform.GetChild(1), GetEvidenceName(evidenceName));
        SetTextInChild(evidencePageInstance.transform.GetChild(2), GetEvidenceInformation(evidenceName));
        SetTextInChild(evidencePageInstance.transform.GetChild(3), "추가 힌트 : " + GetEvidenceNotes(evidenceName));        
        evidenceButtonInstance.SetAnchor();
        evidenceButtonInstance.SetText(evidence);
        evidenceButtonInstance.GetComponent<Button>().onClick.AddListener(SetActiveFalseAllIntroductions);
        evidenceButtonInstance.SetEvidenceIntroduction(evidencePageInstance.GetComponent<Image>());
        evidenceIntroductionList.Add(evidenceButtonInstance.GetEvidenceIntroductionPage());

        // 버튼 위치 업데이트
        UpdateEvidenceButtonPositions(findedEvidenceRectTransformList, evidenceScrollRect);
    }

    /// <summary>
    /// 증거 UI의 텍스트 설정 헬퍼 메서드
    /// </summary>
    private void SetTextInChild(Transform childTransform, string text)
    {
        childTransform.GetComponent<Text>().text = text;
    }

    /// <summary>
    /// 증거 UI의 이미지 설정 헬퍼 메서드
    /// </summary>
    private void SetRawImage(Transform childTransform, RenderTexture evidenceRenderTexture)
    {
        if (evidenceRenderTexture != null)
        {
            childTransform.GetComponent<RawImage>().texture = evidenceRenderTexture;
        }
        else Debug.Log("texture 없음");
    }

    /// <summary>
    /// 모든 증거 소개 페이지 비활성화
    /// </summary>
    void SetActiveFalseAllIntroductions()
    {
        foreach (var page in evidenceIntroductionList)
        {
            if (page.gameObject.activeSelf)
                page.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 증거 버튼들의 위치를 업데이트하는 메서드
    /// </summary>
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


    /// <summary>
    /// 객체 및 자식 객체들의 레이어를 변경하는 재귀 함수
    /// </summary>
    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    //----------------------------------------------------------//

    /// <summary>
    /// NPC에게 증거 정보를 전달하는 메서드
    /// </summary>
    public void SendEvidenceInfo(Evidence evidence)
    {
        if (JsonManager.evidenceInfoList != null)
        {
            EvidenceInfo evidenceInfo = GetEvidenceByName(evidence.GetName());

            if (evidenceInfo == null)
            {
                Debug.LogWarning("증거 정보를 찾을 수 없습니다.");
                return;
            }

            foreach (var npc in npcRole)
            {
                ChatMessage evidenceMessage = new ChatMessage();
                evidenceMessage.Role = "system";

                string extraInformation = GetEvidenceExtraInformation(evidenceInfo.name, npc);

                string completeEvidenceMessage =
                    $"지금 플레이어가 발견한 증거에 대한 정보를 알려줄께.\n" +
                    $"증거 이름 : {evidenceInfo.name}.\n" +
                    $"증거 내용 : {evidenceInfo.information}.\n" +
                    $"증거가 발견된 장소 : {evidenceInfo.foundAt}.\n" +
                    $"추가적으로 필요한 증거나 단서 : {evidenceInfo.notes}.\n" +
                    $"증거가 {npc.currentCharacter}와 관련된 사실 : {extraInformation}.\n";

                evidenceMessage.Content = completeEvidenceMessage;
                npc.AddMessage(evidenceMessage);
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