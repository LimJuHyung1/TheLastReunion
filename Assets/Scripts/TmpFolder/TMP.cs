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
            // 화면 중앙에서 레이캐스트를 쏴서 클릭된 오브젝트 확인
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
            Ray ray = Camera.main.ScreenPointToRay(screenCenter);
            RaycastHit hit;

            // 레이캐스트 디버깅용 라인 그리기
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 1f);  // 레이캐스트를 시각적으로 확인

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                // 클릭된 오브젝트가 있다면
                Debug.Log(hit.collider.gameObject.name + "이(가) 클릭되었습니다!");
            }
            else
            {
                Debug.Log("레이캐스트가 아무것도 맞추지 못했습니다.");
            }
        }
    }
}
