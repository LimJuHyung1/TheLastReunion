using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenAI;


public class EvidenceManager : MonoBehaviour
{
    public NPCRole[] npcRole = new NPCRole[3];
    public ScrollRect evidenceScrollRect;
    public Image evidencePage;

    [SerializeField] GameObject evidenceButton; // Resources ������ ����
    [SerializeField] GameObject evidenceIntroductionPage; // Resources ������ ����
    [SerializeField] private List<RectTransform> findedEvidenceRectTransformList = new List<RectTransform>();
    [SerializeField] private List<Image> evidenceIntroductionList = new List<Image>();
    [SerializeField] private List<Evidence> evidences = new List<Evidence>();
    [SerializeField] private Queue<Evidence> evidenceQueue = new Queue<Evidence>();

    private const int totalEvidenceLength = 9;      // ���߿� 9�� �ϱ�(���� ����)
    private int currentEvidenceLength = 0;
    private float space = 40f;

    void Start()
    {
        SendEvidenceInfo();
        evidenceButton = Resources.Load<GameObject>("EvidenceButton");
        evidenceIntroductionPage = Resources.Load<GameObject>("EvidenceIntroductionPage");
    }

    //----------------------------------------------------------//

    // ���� �ʱ�ȭ
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
        // ť�� ���� �ִ� ��� ���� �ʱ�ȭ
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
        // ���� ��ư ���� �� ����
        EvidenceButton evidenceButtonInstance = Instantiate(evidenceButton, evidenceScrollRect.content).GetComponent<EvidenceButton>();
        findedEvidenceRectTransformList.Add(evidenceButtonInstance.GetComponent<RectTransform>());

        // ���� �Ұ� ������ ���� �� ����
        GameObject evidencePageInstance = Instantiate(evidenceIntroductionPage, evidencePage.transform);
        evidencePageInstance.SetActive(false);
        
        string evidenceName = evidence.GetName();

        // Text ������Ʈ�� ȿ�������� �����ϴ� ���� �Լ� ȣ��
        SetTextInChild(evidencePageInstance.transform.GetChild(0), GetEvidenceName(evidenceName));
        SetTextInChild(evidencePageInstance.transform.GetChild(1), GetEvidenceInformation(evidenceName));
        SetTextInChild(evidencePageInstance.transform.GetChild(2), "�߿䵵 : " + GetEvidenceImportance(evidenceName));
        SetTextInChild(evidencePageInstance.transform.GetChild(3), "�����ι� : " + GetEvidenceRelationship(evidenceName));
        SetTextInChild(evidencePageInstance.transform.GetChild(4), "�߰� ��Ʈ : " + GetEvidenceNotes(evidenceName));

        evidenceButtonInstance.SetAnchor();
        evidenceButtonInstance.SetText(evidence);
        evidenceButtonInstance.GetComponent<Button>().onClick.AddListener(SetActiveFalseAllIntroductions);
        evidenceButtonInstance.SetEvidenceIntroduction(evidencePageInstance.GetComponent<Image>());
        evidenceIntroductionList.Add(evidenceButtonInstance.GetEvidenceIntroductionPage());

        // ��ư ��ġ ������Ʈ
        UpdateEvidenceButtonPositions(findedEvidenceRectTransformList, evidenceScrollRect);
    }

    // �ؽ�Ʈ ������ �ݺ������� ó���ϴ� ���� �Լ�
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

        // ScrollRect content ũ�� ������Ʈ
        scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, y);
    }

    //----------------------------------------------------------//

    // npc�鿡�� ���ſ� ���� ���� ����
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
                            Debug.LogError("3�� npc ���� �ٸ� �̸� ����!");
                            return;
                    }

                    string completeEvidenceMessage =
                        $"{tmpIndex + 1}��° ���ſ� ���� ������, �ش� ������ ���� ĳ������ �µ� �� �ǰ��� �˷��ٲ�.\n" +
                        $"���� �̸� : {evidenceName}.\n" +
                        $"���� ���� : {evidenceInformation}.\n" +
                        $"���Ű� �߰ߵ� ��� : {evidenceFoundAt}.\n" +
                        $"���ſ� ���õ� �ֿ� �ι� : {evidenceRealtionship}.\n" +
                        $"�ش� ������ �߿䵵 : {evidenceImportance}.\n" +
                        $"�߰������� �ʿ��� ���ų� �ܼ� : {evidenceNotes}.\n" +
                        $"�ش� ���ſ� ���� {npcName}�� �µ� �� �ǰ� : {extraInformation}.\n";

                    evidenceMessage.Content = completeEvidenceMessage;

                    tmpIndex++;
                    npcRole[i].AddMessage(evidenceMessage);
                }
            }
        }
    }

    //----------------------------------------------------------//

    // Ư�� ������ ThingInfo�� ��ȯ�ϴ� �޼���
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