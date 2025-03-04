using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Evidence Manager")]
    public EvidenceManager evidenceManager;

    [Header("NPCs Spawn Positions")]
    public Transform spawnPositions;    // NPC가 스폰될 수 있는 위치들을 포함하는 부모 오브젝트

    public Dictionary<Transform, bool> spawnPositionUsage = new Dictionary<Transform, bool>();    // npc 재배치 위치

    private System.Random random = new System.Random();  // 랜덤 인덱스 생성을 위한 Random 객체
    private List<Transform> spawnPositionList = new List<Transform>(); // 사용 가능한 스폰 위치 목록

    private NPCRole[] NPCs = new NPCRole[3];                    // 게임 내 NPC 개체 (3명)
    private string[] NPCNames = { "NPC4", "NPC3", "NPC2" };     // NPC 프리팹의 이름 (Resources 폴더에서 로드)

    void Start()
    {
        SetSpawnPositions();                    // 스폰 위치 설정
        SetInitialNPCPosition();                // NPC 초기 배치
        evidenceManager.ReceiveNPCRole(NPCs);   // NPC 정보를 EvidenceManager에 전달
    }

    /// <summary>
    /// 스폰 위치를 설정하는 함수
    /// - `spawnPositions`의 자식 객체들을 가져와 리스트에 저장
    /// - 사용 가능 여부를 `spawnPositionUsage`에 false로 저장
    /// </summary>
    void SetSpawnPositions()
    {
        spawnPositionList = new List<Transform>();
        spawnPositionUsage = new Dictionary<Transform, bool>();

        foreach (Transform position in spawnPositions)
        {
            spawnPositionUsage[position] = false;   // 초기에는 모든 위치가 사용되지 않음
            spawnPositionList.Add(position);
        }
    }

    /// <summary>
    /// NPC를 초기 위치에 배치하는 함수
    /// - `NPCs` 배열에 있는 NPC를 순차적으로 배치
    /// - 사용된 위치는 `spawnPositionList`에서 제거하여 중복 배치 방지
    /// </summary>
    void SetInitialNPCPosition()
    {
        for (int i = 0; i < NPCs.Length; i++)
        {
            if (spawnPositionList.Count == 0)
            {
                Debug.LogWarning("사용 가능한 스폰 위치가 없습니다.");
                return;
            }

            // 랜덤한 스폰 위치 선택
            int randomIndex = random.Next(spawnPositionList.Count);
            Transform chosenPosition = spawnPositionList[randomIndex];

            // 선택된 위치에서 NPC 생성 및 설정
            GameObject npc = Instantiate(Resources.Load<GameObject>(NPCNames[i]));
            npc.layer = LayerMask.NameToLayer("NPC");
            NPCs[i] = npc.GetComponent<NPCRole>();

            // 선택된 위치에 NPC 배치
            npc.transform.position = chosenPosition.position;
            spawnPositionUsage[chosenPosition] = true;

            // 사용된 위치 제거
            spawnPositionList.RemoveAt(randomIndex);
        }
    }

    /// <summary>
    /// 특정 NPC를 새로운 위치로 이동 (재배치)
    /// </summary>
    /// <param 특정_NPC="npc">재배치할 NPC</param>
    public void RelocationNPC(NPCRole npc)
    {
        if (spawnPositionList.Count == 0)
        {
            Debug.LogWarning("사용 가능한 스폰 위치가 없습니다.");
            return;
        }

        // 랜덤한 새로운 스폰 위치 선택
        int randomIndex = random.Next(spawnPositionList.Count);
        Transform newPosition = spawnPositionList[randomIndex];

        // NPC의 위치를 새로운 위치로 변경
        npc.transform.position = newPosition.position;
        spawnPositionUsage[newPosition] = true;     // 해당 위치를 사용 중으로 표시

        // 사용된 위치를 목록에서 제거하여 중복 배치 방지
        spawnPositionList.RemoveAt(randomIndex);
    }
}
