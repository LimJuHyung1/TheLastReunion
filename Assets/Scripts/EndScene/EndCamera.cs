using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EndCamera : MonoBehaviour
{    
    private float rotationSpeed = 5.0f; // 회전 속도
    private float zoomSpeed = 5.0f; // 확대 속도
    public float targetFOV = 30.0f; // 타겟을 바라볼 때의 줌 레벨
    private float originalFOV; // 원래 카메라 줌 레벨
    private Quaternion originalRotation; // 원래 카메라 회전 값

    private Camera cam; // 카메라 컴포넌트

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        originalFOV = cam.fieldOfView; // 초기 줌 레벨 저장
        originalRotation = transform.rotation; // 초기 회전 값 저장
    }

    public void FocusNPC(Transform target)
    {
        StartCoroutine(FocusOnTargetRoutine(target));
    }

    private IEnumerator FocusOnTargetRoutine(Transform target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);

        // 타겟을 바라보며 확대
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f || Mathf.Abs(cam.fieldOfView - targetFOV) > 0.1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
            yield return null;
        }

        // 최종적으로 정확히 목표 상태로 설정
        transform.rotation = targetRotation;
        cam.fieldOfView = targetFOV;
    }

    /// <summary>
    /// Player 스크립트에서 호출하여 타겟을 확대하며 바라봅니다.
    /// </summary>
    public void FocusAndReturnToOriginal()
    {
        StartCoroutine(ReturnToOriginalRoutine());
    }

    private IEnumerator ReturnToOriginalRoutine()
    {
        // 원래 회전과 FOV로 복구
        while (Quaternion.Angle(transform.rotation, originalRotation) > 0.1f || Mathf.Abs(cam.fieldOfView - originalFOV) > 0.1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, originalRotation, Time.deltaTime * rotationSpeed);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, originalFOV, Time.deltaTime * zoomSpeed);
            yield return null;
        }

        // 최종적으로 정확히 원래 상태로 설정
        transform.rotation = originalRotation;
        cam.fieldOfView = originalFOV;
    }
}
