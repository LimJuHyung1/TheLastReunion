using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdvancedPeopleSystem;

public class Alan : MonoBehaviour
{
    private CharacterCustomization cc;

    void Start()
    {
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
}
