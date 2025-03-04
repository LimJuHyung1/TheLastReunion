using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Evidence Manager")]
    public EvidenceManager evidenceManager;

    [Header("NPCs Spawn Positions")]
    public Transform spawnPositions;    // NPC�� ������ �� �ִ� ��ġ���� �����ϴ� �θ� ������Ʈ

    public Dictionary<Transform, bool> spawnPositionUsage = new Dictionary<Transform, bool>();    // npc ���ġ ��ġ

    private System.Random random = new System.Random();  // ���� �ε��� ������ ���� Random ��ü
    private List<Transform> spawnPositionList = new List<Transform>(); // ��� ������ ���� ��ġ ���

    private NPCRole[] NPCs = new NPCRole[3];                    // ���� �� NPC ��ü (3��)
    private string[] NPCNames = { "NPC4", "NPC3", "NPC2" };     // NPC �������� �̸� (Resources �������� �ε�)

    void Start()
    {
        SetSpawnPositions();                    // ���� ��ġ ����
        SetInitialNPCPosition();                // NPC �ʱ� ��ġ
        evidenceManager.ReceiveNPCRole(NPCs);   // NPC ������ EvidenceManager�� ����
    }

    /// <summary>
    /// ���� ��ġ�� �����ϴ� �Լ�
    /// - `spawnPositions`�� �ڽ� ��ü���� ������ ����Ʈ�� ����
    /// - ��� ���� ���θ� `spawnPositionUsage`�� false�� ����
    /// </summary>
    void SetSpawnPositions()
    {
        spawnPositionList = new List<Transform>();
        spawnPositionUsage = new Dictionary<Transform, bool>();

        foreach (Transform position in spawnPositions)
        {
            spawnPositionUsage[position] = false;   // �ʱ⿡�� ��� ��ġ�� ������ ����
            spawnPositionList.Add(position);
        }
    }

    /// <summary>
    /// NPC�� �ʱ� ��ġ�� ��ġ�ϴ� �Լ�
    /// - `NPCs` �迭�� �ִ� NPC�� ���������� ��ġ
    /// - ���� ��ġ�� `spawnPositionList`���� �����Ͽ� �ߺ� ��ġ ����
    /// </summary>
    void SetInitialNPCPosition()
    {
        for (int i = 0; i < NPCs.Length; i++)
        {
            if (spawnPositionList.Count == 0)
            {
                Debug.LogWarning("��� ������ ���� ��ġ�� �����ϴ�.");
                return;
            }

            // ������ ���� ��ġ ����
            int randomIndex = random.Next(spawnPositionList.Count);
            Transform chosenPosition = spawnPositionList[randomIndex];

            // ���õ� ��ġ���� NPC ���� �� ����
            GameObject npc = Instantiate(Resources.Load<GameObject>(NPCNames[i]));
            npc.layer = LayerMask.NameToLayer("NPC");
            NPCs[i] = npc.GetComponent<NPCRole>();

            // ���õ� ��ġ�� NPC ��ġ
            npc.transform.position = chosenPosition.position;
            spawnPositionUsage[chosenPosition] = true;

            // ���� ��ġ ����
            spawnPositionList.RemoveAt(randomIndex);
        }
    }

    /// <summary>
    /// Ư�� NPC�� ���ο� ��ġ�� �̵� (���ġ)
    /// </summary>
    /// <param Ư��_NPC="npc">���ġ�� NPC</param>
    public void RelocationNPC(NPCRole npc)
    {
        if (spawnPositionList.Count == 0)
        {
            Debug.LogWarning("��� ������ ���� ��ġ�� �����ϴ�.");
            return;
        }

        // ������ ���ο� ���� ��ġ ����
        int randomIndex = random.Next(spawnPositionList.Count);
        Transform newPosition = spawnPositionList[randomIndex];

        // NPC�� ��ġ�� ���ο� ��ġ�� ����
        npc.transform.position = newPosition.position;
        spawnPositionUsage[newPosition] = true;     // �ش� ��ġ�� ��� ������ ǥ��

        // ���� ��ġ�� ��Ͽ��� �����Ͽ� �ߺ� ��ġ ����
        spawnPositionList.RemoveAt(randomIndex);
    }
}
