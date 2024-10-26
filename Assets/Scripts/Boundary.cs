using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundary : MonoBehaviour
{
    public UIManager uIManager;
    private Coroutine fadeCoroutine; // ���� ���� ���� �ڷ�ƾ�� ������ ����

    void OnTriggerEnter(Collider other)
    {
        if (IsPlayerLayer(other.gameObject.layer))
        {
            uIManager.ActivateBoundaryUI();

            // �̹� ���� ���� �ڷ�ƾ�� ������ �����
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            // ���ο� ���̵� ��/�ƿ� �ڷ�ƾ�� �����ϰ� ������ ����
            fadeCoroutine = StartCoroutine(FadeUtility.Instance.FadeInOut(uIManager.GetBoundaryUI(), 1.5f));
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (IsPlayerLayer(other.gameObject.layer))
        {
            // ���� ���� �ڷ�ƾ�� ������ �����
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
                fadeCoroutine = null;  // �ڷ�ƾ ���� �ʱ�ȭ
            }

            uIManager.UnactivateBoundaryUI();
        }
    }

    bool IsPlayerLayer(int layer)
    {
        return layer == LayerMask.NameToLayer("Player");
    }
}
