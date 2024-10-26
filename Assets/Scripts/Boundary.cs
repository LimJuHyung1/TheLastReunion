using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundary : MonoBehaviour
{
    public UIManager uIManager;
    private Coroutine fadeCoroutine; // 현재 실행 중인 코루틴을 저장할 변수

    void OnTriggerEnter(Collider other)
    {
        if (IsPlayerLayer(other.gameObject.layer))
        {
            uIManager.ActivateBoundaryUI();

            // 이미 실행 중인 코루틴이 있으면 멈춘다
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            // 새로운 페이드 인/아웃 코루틴을 시작하고 참조를 저장
            fadeCoroutine = StartCoroutine(FadeUtility.Instance.FadeInOut(uIManager.GetBoundaryUI(), 1.5f));
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (IsPlayerLayer(other.gameObject.layer))
        {
            // 실행 중인 코루틴이 있으면 멈춘다
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
                fadeCoroutine = null;  // 코루틴 참조 초기화
            }

            uIManager.UnactivateBoundaryUI();
        }
    }

    bool IsPlayerLayer(int layer)
    {
        return layer == LayerMask.NameToLayer("Player");
    }
}
