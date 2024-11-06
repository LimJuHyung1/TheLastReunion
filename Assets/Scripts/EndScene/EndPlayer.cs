using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPlayer : MonoBehaviour
{
    public Camera cam;

    /// <summary>
    /// NPC 방향으로 rotate
    /// </summary>
    public void TurnTowardNPC(Transform npcTrans)
    {
        // 현재 오브젝트의 위치
        Vector3 targetPosition = npcTrans.position;

        // 현재 오브젝트의 y 위치는 변경하지 않음
        targetPosition.y = transform.position.y;

        // 타겟 위치를 바라보도록 회전
        transform.LookAt(targetPosition);
        cam.GetComponent<CameraScript>().FocusNPC(this.transform);
    }
}
