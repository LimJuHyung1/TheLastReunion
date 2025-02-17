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

    GameObject evidenceButton; // Resources ������ ����
    GameObject evidenceIntroductionPage; // Resources ������ ����
    [SerializeField] private List<RectTransform> findedEvidenceRectTransformList = new List<RectTransform>();
    [SerializeField] private List<Image> evidenceIntroductionList = new List<Image>();
    [SerializeField] private List<Evidence> evidences = new List<Evidence>();
    [SerializeField] private Queue<Evidence> evidenceQueue = new Queue<Evidence>();

    private NPCRole[] npcRole = new NPCRole[3];

    private const int totalEvidenceLength = 11;      // ���� ����
    private int currentEvidenceLength = 0;
    private float space = 40f;

    private int resWidth;   // ����� ����� ���� ����
    private int resHeight;  // ����� ����� ���� ����

    private float scrollRectBlank;  // evidenceScrollRect ���� ����
    private float scrollRectWidth;  // evidenceScrollRect ���� ����
    private float scrollRectHeight;  // evidenceScrollRect ���� ����


    void Start()
    {        
        evidenceButton = Resources.Load<GameObject>("EvidenceButton");
        evidenceIntroductionPage = Resources.Load<GameObject>("EvidenceIntroductionPage");

        SetEvidenceScrollRect();
    }

    //----------------------------------------------------------//

    void SetEvidenceScrollRect()
    {
        Resolution currentRes = Screen.currentResolution;
        resWidth = currentRes.width;
        resHeight = currentRes.height;

        // scrollRect ���� ���� ����
        Vector2 newPosition = evidenceScrollRect.GetComponent<RectTransform>().anchoredPosition;
        scrollRectBlank = resWidth * 0.1f;   // ���� ��ũ�Ѱ� ����� ���� ���� ���� ����
        newPosition.x = scrollRectBlank;
        evidenceScrollRect.GetComponent<RectTransform>().anchoredPosition = newPosition;

        // scrollRect ���� ���� ����
        Vector2 newDeltaSize = evidenceScrollRect.GetComponent<RectTransform>().sizeDelta;
        scrollRectWidth = resWidth * 0.2f;  // ���� ��ũ�� ���� ����(����� ���� ������ 25%)
        scrollRectHeight = resHeight * 0.6f;    // ���� ��ũ�� ���� ����(����� ���� ������ 80%)
        newDeltaSize.x = scrollRectWidth;
        newDeltaSize.y = scrollRectHeight;

        evidenceScrollRect.GetComponent<RectTransform>().sizeDelta = newDeltaSize;


    }

    //----------------------------------------------------------//

    public void ReceiveNPCRole(NPCRole[] npcs)
    {
        npcRole = npcs;
    }

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
            GetEvidenceFoundAt(evidenceName),
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
        // ���� ��ư ���� �� ����
        EvidenceButton evidenceButtonInstance = Instantiate(evidenceButton, evidenceScrollRect.content).GetComponent<EvidenceButton>();
        SetEviedenceButton(evidenceButtonInstance.GetComponent<RectTransform>());

        findedEvidenceRectTransformList.Add(evidenceButtonInstance.GetComponent<RectTransform>());

        // ���� �Ұ� ������ ���� �� ����
        GameObject evidencePageInstance = Instantiate(evidenceIntroductionPage, evidencePage.transform);
        SetEvidencePage(evidencePageInstance.GetComponent<RectTransform>());

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

    void SetEviedenceButton(RectTransform param)
    {
        // ���� ��ư ���� �� ����        
        Vector2 buttonSizeDelta = param.sizeDelta;
        buttonSizeDelta.x = scrollRectWidth * 0.9f;
        param.GetComponent<RectTransform>().sizeDelta = buttonSizeDelta;
    }

    void SetEvidencePage(RectTransform param)
    {
        // ���� ����
        Vector2 newPosition = param.anchoredPosition;
        newPosition.x = -scrollRectBlank;
        param.anchoredPosition = newPosition;

        // ����, ���� ���� ����
        Vector2 newDeltaSize = param.sizeDelta;
        newDeltaSize.x = resWidth * 0.35f;
        newDeltaSize.y = scrollRectHeight;
        param.sizeDelta = newDeltaSize;

        // SetEvidencePageChildern(param);
    }

    void SetEvidencePageChildern(RectTransform parent)
    {
        float parentWidth = parent.sizeDelta.x;
        float parentHeight = parent.sizeDelta.y;

        Debug.Log($"�ʱ� �θ� ũ�� - Width: {parentWidth}, Height: {parentHeight}");

        // Anchor�� �θ��� ������ �߾����� ����
        parent.anchorMin = new Vector2(1, 0.5f);
        parent.anchorMax = new Vector2(1, 0.5f);
        parent.pivot = new Vector2(1, 0.5f); // �������� ������ �߾����� ����

        Vector2 newPosition;
        Vector2 newDeltaSize;

        Graphic[] graphics = parent.GetComponentsInChildren<Graphic>();
        if (graphics.Length == 0)
        {
            Debug.LogError("�ڽ� UI ���(Graphic)�� �����ϴ�!");
            return;
        }

        RectTransform tmpRectTransform = graphics[0].GetComponent<RectTransform>();
        if (tmpRectTransform == null)
        {
            Debug.LogError($"{graphics[0].gameObject.name} �� RectTransform�� �����ϴ�!");
            return;
        }

        //-------------------- ���� ���� --------------------//        

        // ���� ���� (������ �߾��� �������� ����)
        newPosition = tmpRectTransform.anchoredPosition;
        newPosition.x = -parentWidth * 0.1f; // ������ �߾��� �������� �������� �̵�
        newPosition.y = parentHeight * 0.1f;
        tmpRectTransform.anchoredPosition = newPosition;

        // ����, ���� ���� ����
        newDeltaSize = tmpRectTransform.sizeDelta;
        newDeltaSize.x = parentWidth * 0.8f;
        newDeltaSize.y = parentHeight * 0.2f;
        tmpRectTransform.sizeDelta = newDeltaSize;

        Debug.Log($"���� �θ� ũ�� - Width: {parent.sizeDelta.x}, Height: {parent.sizeDelta.y}");
    }

    // �ؽ�Ʈ ������ �ݺ������� ó���ϴ� ���� �Լ�
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
        else Debug.Log("texture ����");
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


    // ��������� ���̾ �����ϴ� �Լ�
    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    //----------------------------------------------------------//

    // npc�鿡�� ���ſ� ���� ���� ����
    public void SendEvidenceInfo(Evidence evidence)
    {
        if (JsonManager.evidenceInfoList != null)
        {
            string evidenceName;
            string evidenceInformation;
            string evidenceFoundAt;
            string evidenceNotes;

            string extraInformation;

            for (int i = 0; i < npcRole.Length; i++)
            {
                ChatMessage evidenceMessage = new ChatMessage();
                evidenceMessage.Role = "system";

                evidenceName = GetEvidenceName(evidence.GetName());
                evidenceInformation = GetEvidenceInformation(evidence.GetName());
                evidenceFoundAt = GetEvidenceFoundAt(evidence.GetName());
                evidenceNotes = GetEvidenceNotes(evidence.GetName());
                extraInformation = GetEvidenceExtraInformation(evidence.GetName(), npcRole[i]);
                string completeEvidenceMessage =
                "���� �÷��̾ �߰��� ���ſ� ���� ������ �˷��ٲ�.\n" +
                $"���� �̸� : {evidenceName}.\n" +
                $"���� ���� : {evidenceInformation}.\n" +
                $"���Ű� �߰ߵ� ��� : {evidenceFoundAt}.\n" +
                $"�߰������� �ʿ��� ���ų� �ܼ� : {evidenceNotes}.\n" +
                $"���Ű� {npcRole[i].currentCharacter.ToString()}�� ���õ� ��� : {extraInformation}.\n";

                evidenceMessage.Content = completeEvidenceMessage;
                npcRole[i].AddMessage(evidenceMessage);
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