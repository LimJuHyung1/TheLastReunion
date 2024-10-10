using System.Collections.Generic;
using UnityEngine;
using OpenAI;


public class EvidenceManager : MonoBehaviour
{
    [SerializeField] private List<Evidence> evidences = new List<Evidence>();
    [SerializeField] private Queue<Evidence> evidenceQueue = new Queue<Evidence>();

    private const int totalEvidenceLength = 9;      // 나중에 9로 하기(증거 개수)
    private int currentEvidenceLength = 0;

    public NPCRole[] npcRole = new NPCRole[3];

    void Start()
    {
        InitializeEvidenceInformation();
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

    private void InitializeEvidence(Evidence evidence)
    {
        string evidenceName = evidence.GetEvidenceName();
        evidence.Initialize(GetEvidenceName(evidenceName), GetEvidenceDescription(evidenceName));
        evidences.Add(evidence);
    }

    private void DequeueAllEvidence()
    {
        // 큐에 남아 있는 모든 증거 초기화
        while (evidenceQueue.Count > 0)
        {
            InitializeEvidence(evidenceQueue.Dequeue());
        }
    }   

    //----------------------------------------------------------//

    // npc들에게 증거에 대한 정보 전달
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
                            Debug.LogError("3명 npc 외의 다른 이름 참조!");
                            return;
                    }

                    string completeEvidenceMessage =
                        $"{i + 1}번째 증거에 대한 정보와, 해당 정보에 대한 캐릭터의 태도 및 의견을 알려줄께.\n" +
                        $"증거 이름 : {evidenceName}.\n" +
                        $"증거 내용 : {evidenceInformation}.\n" +
                        $"증거가 발견된 장소 : {evidenceFoundAt}.\n" +
                        $"증거와 관련된 주요 인물 : {evidenceRealtionship}.\n" +
                        $"해당 증거의 중요도 : {evidenceImportance}.\n" +
                        $"추가적으로 필요한 증거나 단서 : {evidenceNotes}.\n" +
                        $"해당 증거에 대한 {npcName}의 태도 및 의견 : {extraInformation}.\n";

                    evidenceMessage.Content = completeEvidenceMessage;

                    npcRole[i].AddMessage(npcRole[i].currentCharacter, evidenceMessage);
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
        return evidence?.description; // null이면 null 반환
    }
}