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

        // 씬 시작 시 색상 초기화
        if (cc != null)
        {
            cc.InitColors();
            Debug.Log(name + "의 색상이 초기화되었습니다, start");
        }
        else Debug.Log(name + " cc 없음");
    }
}
