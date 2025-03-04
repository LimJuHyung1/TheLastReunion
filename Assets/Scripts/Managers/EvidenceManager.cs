using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenAI;
using static Evidence;


public class EvidenceManager : MonoBehaviour
{
    [Header("Evidence UI")]
    public ScrollRect evidenceScrollRect;   // ���� ��ư�� ǥ���ϴ� ��ũ�� UI
    public Image evidencePage;              // ���� �Ұ� ������ UI

    [Header("Managers")]
    public SpawnManager spawnManager;
        
    GameObject evidenceButton; // Resources ������ ����
    GameObject evidenceIntroductionPage; // Resources ������ ����

    [SerializeField] // �߰��� ���� ��ư ��ġ ����Ʈ
    private List<RectTransform> findedEvidenceRectTransformList = new List<RectTransform>();
    [SerializeField] // ���� �Ұ� ������ ����Ʈ
    private List<Image> evidenceIntroductionList = new List<Image>();
    [SerializeField] // ���� ����Ʈ
    private List<Evidence> evidences = new List<Evidence>();
    [SerializeField] // ���� �ʱ�ȭ ��� ť
    private Queue<Evidence> evidenceQueue = new Queue<Evidence>();

    private NPCRole[] npcRole = new NPCRole[3];

    private const int totalEvidenceLength = 11;         // ���� ����
    private int currentEvidenceLength = 0;              // ���� �߰��� ���� ����
    private float space = 40f;                          // ��ư �� ����

    void Start()
    {
        // Resources �������� ������ �ҷ�����
        evidenceButton = Resources.Load<GameObject>("EvidenceButton");
        evidenceIntroductionPage = Resources.Load<GameObject>("EvidenceIntroductionPage");
    }

    //----------------------------------------------------------//

    /// <summary>
    /// NPC ������ �޾� ����
    /// </summary>
    public void ReceiveNPCRole(NPCRole[] npcs)
    {
        npcRole = npcs;
    }

    /// <summary>
    /// ���� �ʱ�ȭ
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
    /// ť�� �ִ� ��� ���Ÿ� �ʱ�ȭ�ϴ� �޼���
    /// </summary>
    private void DequeueAllEvidence()
    {
        // ť�� ���� �ִ� ��� ���� �ʱ�ȭ
        while (evidenceQueue.Count > 0)
        {
            InitializeEvidence(evidenceQueue.Dequeue());
        }
    }

    /// <summary>
    /// ���� ��ü�� �ʱ�ȭ�ϴ� �޼���
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
    /// ���Ÿ� ã���� �� ȣ��Ǵ� �޼���
    /// </summary>
    public void FindEvidence(Evidence evidence)
    {
        SendEvidenceInfo(evidence);
        UpdateEvidencePage(evidence);

        int targetLayer = LayerMask.NameToLayer("House");
        SetLayerRecursively(evidence.gameObject, targetLayer);
    }

    /// <summary>
    /// ���� �������� ������Ʈ�ϴ� �޼���
    /// </summary>
    private void UpdateEvidencePage(Evidence evidence)
    {
        // ���� ��ư ���� �� ����
        EvidenceButton evidenceButtonInstance = Instantiate(evidenceButton, evidenceScrollRect.content).GetComponent<EvidenceButton>();
        findedEvidenceRectTransformList.Add(evidenceButtonInstance.GetComponent<RectTransform>());

        // ���� �Ұ� ������ ���� �� ����
        GameObject evidencePageInstance = Instantiate(evidenceIntroductionPage, evidencePage.transform);
        evidencePageInstance.SetActive(false);

        string evidenceName = evidence.GetName();

        // Text ������Ʈ�� ȿ�������� �����ϴ� ���� �Լ� ȣ��
        SetRawImage(evidencePageInstance.transform.GetChild(0), GetEvidenceRenderTexture(evidenceName));
        SetTextInChild(evidencePageInstance.transform.GetChild(1), GetEvidenceName(evidenceName));
        SetTextInChild(evidencePageInstance.transform.GetChild(2), GetEvidenceInformation(evidenceName));
        SetTextInChild(evidencePageInstance.transform.GetChild(3), "�߰� ��Ʈ : " + GetEvidenceNotes(evidenceName));        
        evidenceButtonInstance.SetAnchor();
        evidenceButtonInstance.SetText(evidence);
        evidenceButtonInstance.GetComponent<Button>().onClick.AddListener(SetActiveFalseAllIntroductions);
        evidenceButtonInstance.SetEvidenceIntroduction(evidencePageInstance.GetComponent<Image>());
        evidenceIntroductionList.Add(evidenceButtonInstance.GetEvidenceIntroductionPage());

        // ��ư ��ġ ������Ʈ
        UpdateEvidenceButtonPositions(findedEvidenceRectTransformList, evidenceScrollRect);
    }

    /// <summary>
    /// ���� UI�� �ؽ�Ʈ ���� ���� �޼���
    /// </summary>
    private void SetTextInChild(Transform childTransform, string text)
    {
        childTransform.GetComponent<Text>().text = text;
    }

    /// <summary>
    /// ���� UI�� �̹��� ���� ���� �޼���
    /// </summary>
    private void SetRawImage(Transform childTransform, RenderTexture evidenceRenderTexture)
    {
        if (evidenceRenderTexture != null)
        {
            childTransform.GetComponent<RawImage>().texture = evidenceRenderTexture;
        }
        else Debug.Log("texture ����");
    }

    /// <summary>
    /// ��� ���� �Ұ� ������ ��Ȱ��ȭ
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
    /// ���� ��ư���� ��ġ�� ������Ʈ�ϴ� �޼���
    /// </summary>
    void UpdateEvidenceButtonPositions(List<RectTransform> foundEvidenceList, ScrollRect scrollRect)
    {
        float y = 30f;
        for (int i = 0; i < foundEvidenceList.Count; i++)
        {
            foundEvidenceList[i].anchoredPosition = new Vector2(0f, -y);
            y += foundEvidenceList[i].sizeDelta.y + space;
        }

        // ScrollRect content ũ�� ������Ʈ
        scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, y);
    }


    /// <summary>
    /// ��ü �� �ڽ� ��ü���� ���̾ �����ϴ� ��� �Լ�
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
    /// NPC���� ���� ������ �����ϴ� �޼���
    /// </summary>
    public void SendEvidenceInfo(Evidence evidence)
    {
        if (JsonManager.evidenceInfoList != null)
        {
            EvidenceInfo evidenceInfo = GetEvidenceByName(evidence.GetName());

            if (evidenceInfo == null)
            {
                Debug.LogWarning("���� ������ ã�� �� �����ϴ�.");
                return;
            }

            foreach (var npc in npcRole)
            {
                ChatMessage evidenceMessage = new ChatMessage();
                evidenceMessage.Role = "system";

                string extraInformation = GetEvidenceExtraInformation(evidenceInfo.name, npc);

                string completeEvidenceMessage =
                    $"���� �÷��̾ �߰��� ���ſ� ���� ������ �˷��ٲ�.\n" +
                    $"���� �̸� : {evidenceInfo.name}.\n" +
                    $"���� ���� : {evidenceInfo.information}.\n" +
                    $"���Ű� �߰ߵ� ��� : {evidenceInfo.foundAt}.\n" +
                    $"�߰������� �ʿ��� ���ų� �ܼ� : {evidenceInfo.notes}.\n" +
                    $"���Ű� {npc.currentCharacter}�� ���õ� ��� : {extraInformation}.\n";

                evidenceMessage.Content = completeEvidenceMessage;
                npc.AddMessage(evidenceMessage);
            }
        }
    }


    //----------------------------------------------------------//

    // Ư�� ������ EvidenceInfo�� ��ȯ�ϴ� �޼���
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

        Debug.LogWarning($"'{evidenceName}' ���Ÿ� ã�� �� �����ϴ�.");
        return null;
    }

    // Ư�� ���� �̸� ��ȯ
    public string GetEvidenceName(string evidenceName)
    {
        EvidenceInfo evidence = GetEvidenceByName(evidenceName);
        return evidence?.name; // null�̸� null ��ȯ
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