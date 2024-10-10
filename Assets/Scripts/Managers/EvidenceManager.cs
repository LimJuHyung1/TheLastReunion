using System.Collections.Generic;
using UnityEngine;
using OpenAI;


public class EvidenceManager : MonoBehaviour
{
    [SerializeField] private List<Evidence> evidences = new List<Evidence>();
    [SerializeField] private Queue<Evidence> evidenceQueue = new Queue<Evidence>();

    private const int totalEvidenceLength = 9;      // ���߿� 9�� �ϱ�(���� ����)
    private int currentEvidenceLength = 0;

    public NPCRole[] npcRole = new NPCRole[3];

    void Start()
    {
        InitializeEvidenceInformation();
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

    private void InitializeEvidence(Evidence evidence)
    {
        string evidenceName = evidence.GetEvidenceName();
        evidence.Initialize(GetEvidenceName(evidenceName), GetEvidenceDescription(evidenceName));
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

    //----------------------------------------------------------//

    // npc�鿡�� ���ſ� ���� ���� ����
    private void InitializeEvidenceInformation()
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
                        $"{i + 1}��° ���ſ� ���� ������, �ش� ������ ���� ĳ������ �µ� �� �ǰ��� �˷��ٲ�.\n" +
                        $"���� �̸� : {evidenceName}.\n" +
                        $"���� ���� : {evidenceInformation}.\n" +
                        $"���Ű� �߰ߵ� ��� : {evidenceFoundAt}.\n" +
                        $"���ſ� ���õ� �ֿ� �ι� : {evidenceRealtionship}.\n" +
                        $"�ش� ������ �߿䵵 : {evidenceImportance}.\n" +
                        $"�߰������� �ʿ��� ���ų� �ܼ� : {evidenceNotes}.\n" +
                        $"�ش� ���ſ� ���� {npcName}�� �µ� �� �ǰ� : {extraInformation}.\n";

                    evidenceMessage.Content = completeEvidenceMessage;

                    npcRole[i].AddMessage(npcRole[i].currentCharacter, evidenceMessage);
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
        return evidence?.description; // null�̸� null ��ȯ
    }
}