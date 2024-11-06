using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public Transform spawnPositions;
    public Dictionary<Transform, bool> spawnPositionUsage = new Dictionary<Transform, bool>();    // npc 재배치 위치
    public NPCRole[] NPCs;

    private System.Random random = new System.Random();
    private List<Transform> spawnPositionList = new List<Transform>(); // 미사용 위치 목록

    void Start()
    {
        SetSpawnPositions();
        SetInitialNPCPosition();
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
            // 랜덤 인덱스 선택 및 위치 설정
            int randomIndex = random.Next(spawnPositionList.Count);
            Transform chosenPosition = spawnPositionList[randomIndex];

            if (IsSpawnedPosition(chosenPosition))
            {
                i--;
                continue;
            }
            else
            {
                NPCs[i].transform.position = chosenPosition.position;
                spawnPositionUsage[chosenPosition] = true;
            }
        }
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
