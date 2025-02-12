using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdvancedPeopleSystem;

public class Alan : MonoBehaviour
{
    private CharacterCustomization cc;

    void Start()
    {
        cc = GetComponent<CharacterCustomization>();

        // �� ���� �� ���� �ʱ�ȭ
        if (cc != null)
        {
            cc.InitColors();
            Debug.Log(name + "�� ������ �ʱ�ȭ�Ǿ����ϴ�, start");
        }
        else Debug.Log(name + " cc ����");
    }
}
