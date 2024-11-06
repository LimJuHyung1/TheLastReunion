using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPlayer : MonoBehaviour
{
    public Camera cam;

    /// <summary>
    /// NPC �������� rotate
    /// </summary>
    public void TurnTowardNPC(Transform npcTrans)
    {
        // ���� ������Ʈ�� ��ġ
        Vector3 targetPosition = npcTrans.position;

        // ���� ������Ʈ�� y ��ġ�� �������� ����
        targetPosition.y = transform.position.y;

        // Ÿ�� ��ġ�� �ٶ󺸵��� ȸ��
        transform.LookAt(targetPosition);
        cam.GetComponent<CameraScript>().FocusNPC(this.transform);
    }
}
