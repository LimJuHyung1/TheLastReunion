using System.Collections.Generic;
using UnityEngine;
using OpenAI;


public class EvidenceManager : MonoBehaviour
{
    // <����, �ش� ���Ÿ� ������� ����>
    // private Dictionary<Evidence, bool> evidenceDict = new Dictionary<Evidence, bool>();
    [SerializeField] private List<Evidence> evidences = new List<Evidence>();
    [SerializeField] private Queue<Evidence> evidenceQueue = new Queue<Evidence>();

    private const int totalEvidenceLength = 4;
    private int currentEvidenceLength = 0;

    public NPCRole[] npcRole = new NPCRole[3];
    public JsonManager jsonManager;


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

    private void InitializeEvidence(Evidence evidence)
    {
        string evidenceName = evidence.GetEvidenceName();
        // Debug.Log("evidence �̸� ���� : " + evidenceName);
        evidence.Initialize(GetEvidenceName(evidenceName), GetEvidenceDescription(evidenceName));
        // Debug.Log("���� �̸� : " + GetEvidenceName(evidenceName) + "\n���� ���� : " + GetEvidenceDescription(evidenceName));
        evidences.Add(evidence);
    }

    private void DequeueAllEvidence()
    {
        // ť�� ���� �ִ� ��� ���� �ʱ�ȭ
        while (evidenceQueue.Count > 0)
        {
            InitializeEvidence(evidenceQueue.Dequeue());
        }
    }

    // ���Ÿ� ã���� ��� �̺�Ʈ �߻���Ű��
    public void FindEvidence(Evidence evidence)
    {
        for(int i = 0; i < npcRole.Length; i++)
        {
            // npcRole[i].DiscoverEvidence(evidence);
            SendInformation(evidence, npcRole[i]);
        }
    }
    
    /// <summary>
    /// npc���� ���� ���� ����
    /// </summary>
    /// <param EvidenceManager���� �����ϴ� ���� ������Ʈ="evidence"></param>
    /// <param �� npc="npc"></param>
    void SendInformation(Evidence evidence, NPCRole npc)
    {
        ChatMessage evidenceMessage = new ChatMessage();
        evidenceMessage.Role = "system";
        
        string information = GetEvidenceInformation(evidence.name);
        string extraInformation = GetNPCInformation(evidence.name, npc.currentCharacter.ToString());

        evidenceMessage.Content = information + extraInformation;

        npc.AddMessage(evidenceMessage);
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

    // Ư�� ���� ���� ��ȯ
    public string GetEvidenceDescription(string evidenceName)
    {
        EvidenceInfo evidence = GetEvidenceByName(evidenceName);
        return evidence?.description; // null�̸� null ��ȯ
    }

    // Ư�� ���� ���� ��ȯ
    public string GetEvidenceInformation(string evidenceName)
    {
        EvidenceInfo evidence = GetEvidenceByName(evidenceName);
        return evidence?.information; // null�̸� null ��ȯ
    }

    // npc�� ���� ������ ���� ����
    public string GetNPCInformation(string evidenceName, string npcName)
    {
        EvidenceInfo evidence = GetEvidenceByName(evidenceName);

        if(npcName == "Nason")
            return evidence?.nasonExtraInformation; // null�̸� null ��ȯ
        else if(npcName == "Jenny")
            return evidence?.jennyExtraInformation;
        else if(npcName == "Jenny")
            return evidence?.minaExtraInformation;

        else return null;
    }
}