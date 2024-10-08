using System.Collections.Generic;
using UnityEngine;
using OpenAI;


public class EvidenceManager : MonoBehaviour
{
    // <증거, 해당 증거를 얻었는지 여부>
    // private Dictionary<Evidence, bool> evidenceDict = new Dictionary<Evidence, bool>();
    [SerializeField] private List<Evidence> evidences = new List<Evidence>();
    [SerializeField] private Queue<Evidence> evidenceQueue = new Queue<Evidence>();

    private const int totalEvidenceLength = 4;
    private int currentEvidenceLength = 0;

    public NPCRole[] npcRole = new NPCRole[3];
    public JsonManager jsonManager;


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
        // Debug.Log("evidence 이름 시험 : " + evidenceName);
        evidence.Initialize(GetEvidenceName(evidenceName), GetEvidenceDescription(evidenceName));
        // Debug.Log("증거 이름 : " + GetEvidenceName(evidenceName) + "\n증거 설명 : " + GetEvidenceDescription(evidenceName));
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

    // 증거를 찾았을 경우 이벤트 발생시키기
    public void FindEvidence(Evidence evidence)
    {
        for(int i = 0; i < npcRole.Length; i++)
        {
            // npcRole[i].DiscoverEvidence(evidence);
            SendInformation(evidence, npcRole[i]);
        }
    }
    
    /// <summary>
    /// npc에게 증거 정보 전달
    /// </summary>
    /// <param EvidenceManager에서 관리하는 증거 오브젝트="evidence"></param>
    /// <param 각 npc="npc"></param>
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

    // 특정 증거 설명 반환
    public string GetEvidenceDescription(string evidenceName)
    {
        EvidenceInfo evidence = GetEvidenceByName(evidenceName);
        return evidence?.description; // null이면 null 반환
    }

    // 특정 증거 정보 반환
    public string GetEvidenceInformation(string evidenceName)
    {
        EvidenceInfo evidence = GetEvidenceByName(evidenceName);
        return evidence?.information; // null이면 null 반환
    }

    // npc에 따라 적절한 정보 전달
    public string GetNPCInformation(string evidenceName, string npcName)
    {
        EvidenceInfo evidence = GetEvidenceByName(evidenceName);

        if(npcName == "Nason")
            return evidence?.nasonExtraInformation; // null이면 null 반환
        else if(npcName == "Jenny")
            return evidence?.jennyExtraInformation;
        else if(npcName == "Jenny")
            return evidence?.minaExtraInformation;

        else return null;
    }
}