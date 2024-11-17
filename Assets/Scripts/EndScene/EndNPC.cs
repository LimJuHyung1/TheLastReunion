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

        // �� ���� �� ���� �ʱ�ȭ
        if (cc != null)
        {
            cc.InitColors();
        }
    }

    private void OnEnable()
    {
        // ��ü Ȱ��ȭ �� ���� �ʱ�ȭ
        if (cc != null)
        {
            cc.InitColors();
        }
    }

    /// <summary>
    /// �÷��̾� �������� rotate
    /// </summary>
    public void TurnTowardPlayer(Transform playerTrans)
    {
        // ���� ������Ʈ�� ��ġ
        Vector3 targetPosition = playerTrans.position;

        // ���� ������Ʈ�� y ��ġ�� �������� ����
        targetPosition.y = transform.position.y;

        // Ÿ�� ��ġ�� �ٶ󺸵��� ȸ��
        transform.LookAt(targetPosition);
    }

    public void PlayEmotion(string statement)
    {                
        emotionTime = Mathf.Clamp(statement.Length * 0.2f, 2f, 5f); // �ּ� 2��, �ִ� 5��
        cc.PlayBlendshapeAnimation(emotionsList[5], emotionTime);
    }
}
