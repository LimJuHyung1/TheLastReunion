using AdvancedPeopleSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndNPC : MonoBehaviour
{
    private float emotionTime = 5;
    private CharacterCustomization cc;
    private List<string> emotionsList = new List<string>();


    void Start()
    {
        cc = GetComponent<CharacterCustomization>();

        foreach (var e in cc.Settings.characterAnimationPresets)
        {
            emotionsList.Add(e.name);
        }

        // 씬 시작 시 색상 초기화
        if (cc != null)
        {
            cc.InitColors();
        }
    }

    private void OnEnable()
    {
        // 객체 활성화 시 색상 초기화
        if (cc != null)
        {
            cc.InitColors();
        }
    }

    /// <summary>
    /// 플레이어 방향으로 rotate
    /// </summary>
    public void TurnTowardPlayer(Transform playerTrans)
    {
        // 현재 오브젝트의 위치
        Vector3 targetPosition = playerTrans.position;

        // 현재 오브젝트의 y 위치는 변경하지 않음
        targetPosition.y = transform.position.y;

        // 타겟 위치를 바라보도록 회전
        transform.LookAt(targetPosition);
    }

    public void PlayEmotion(string statement)
    {                
        emotionTime = Mathf.Clamp(statement.Length * 0.2f, 2f, 5f); // 최소 2초, 최대 5초
        cc.PlayBlendshapeAnimation(emotionsList[5], emotionTime);
    }
}
