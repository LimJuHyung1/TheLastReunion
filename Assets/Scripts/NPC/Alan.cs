using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdvancedPeopleSystem;

public class Alan : MonoBehaviour
{
    private CharacterCustomization cc;

    void Start()
    {
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
}
