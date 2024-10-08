//using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TMP : MonoBehaviour
{


    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // ȭ�� �߾ӿ��� ����ĳ��Ʈ�� ���� Ŭ���� ������Ʈ Ȯ��
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
            Ray ray = Camera.main.ScreenPointToRay(screenCenter);
            RaycastHit hit;

            // ����ĳ��Ʈ ������ ���� �׸���
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 1f);  // ����ĳ��Ʈ�� �ð������� Ȯ��

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                // Ŭ���� ������Ʈ�� �ִٸ�
                Debug.Log(hit.collider.gameObject.name + "��(��) Ŭ���Ǿ����ϴ�!");
            }
            else
            {
                Debug.Log("����ĳ��Ʈ�� �ƹ��͵� ������ ���߽��ϴ�.");
            }
        }
    }
}
