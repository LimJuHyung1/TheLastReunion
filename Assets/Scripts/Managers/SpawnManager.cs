using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public EvidenceManager evidenceManager;
    public Transform spawnPositions;
    public Dictionary<Transform, bool> spawnPositionUsage = new Dictionary<Transform, bool>();    // npc ���ġ ��ġ    [SerializeField] private NPCRole[] NPCs;

    private System.Random random = new System.Random();
    private List<Transform> spawnPositionList = new List<Transform>(); // �̻�� ��ġ ���
    [SerializeField] private NPCRole[] NPCs = new NPCRole[3];
    [SerializeField] private string[] NPCNames = { "NPC4", "NPC3", "NPC2" };

    void Start()
    {
        SetSpawnPositions();
        SetInitialNPCPosition();
        evidenceManager.ReceiveNPCRole(SendNPCsToEvidenceManager());
    }

    void SetSpawnPositions()
    {
        for (int i = 0; i < spawnPositions.childCount; i++)
        {
            Transform position = spawnPositions.GetChild(i).GetComponent<Transform>();
            spawnPositionUsage.Add(position, false);
            spawnPositionList.Add(position);
        }
    }    

    void SetInitialNPCPosition()
    {
        for (int i = 0; i < NPCs.Length; i++)
        {            
            // ���� �ε��� ���� �� ��ġ ����
            int randomIndex = random.Next(spawnPositionList.Count);
            Transform chosenPosition = spawnPositionList[randomIndex];

            if (IsSpawnedPosition(chosenPosition))
            {
                i--;
                continue;
            }
            else
            {
                GameObject npc = Instantiate(Resources.Load<GameObject>(NPCNames[i]));
                int layer = LayerMask.NameToLayer("NPC");
                npc.layer = layer;
                NPCs[i] = npc.GetComponent<NPCRole>();

                npc.transform.position = chosenPosition.position;
                spawnPositionUsage[chosenPosition] = true;                
            }
        }        
    }

    public NPCRole[] SendNPCsToEvidenceManager()
    {
        return NPCs;
    }



    public void RelocationNPC(NPCRole npc)
    {       
        while (true)
        {
            int randomIndex = random.Next(spawnPositionUsage.Count);
            Transform trans = spawnPositionList[randomIndex];

            if (IsSpawnedPosition(trans))
            {
                continue;
            }
            else
            {
                npc.transform.position = trans.position;
                spawnPositionUsage[trans] = true;
                break;
            }
        }        
    }

    bool IsSpawnedPosition(Transform trans)
    {
        return spawnPositionUsage[trans];
    }
}
