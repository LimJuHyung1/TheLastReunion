using UnityEngine;
using System.Collections;

/// <summary>
/// 특정 NPC를 바라보도록 카메라를 회전시키고 줌인을 조정하는 클래스.
/// NPC를 초점으로 맞추거나 원래 위치로 복귀하는 기능을 제공.
/// </summary>
public class EndCamera : MonoBehaviour
{
    private float rotationSpeed = 5.0f; // 카메라 회전 속도
    private float zoomSpeed = 5.0f; // 카메라 줌 속도
    private float targetFOV = 30.0f; // 줌인 시 목표 FOV (Field of View)
    private float originalFOV; // 원래 카메라 FOV 값

    private Quaternion originalRotation; // 원래 카메라 회전 값
    private Camera cam; // 카메라 컴포넌트 참조

    private Coroutine activeCoroutine; // 현재 실행 중인 코루틴을 추적하여 중복 실행 방지

    /// <summary>
    /// 카메라의 초기 상태를 저장
    /// </summary>
    void Start()
    {
        cam = GetComponent<Camera>();
        ResetOriginalState();
    }

    /// <summary>
    /// 현재 카메라 상태를 원래 상태로 설정 (초기화 가능)
    /// </summary>
    public void ResetOriginalState()
    {
        originalFOV = cam.fieldOfView;
        originalRotation = transform.rotation;
    }

    /// <summary>
    /// 새로운 카메라 이동 코루틴을 실행하기 전에 기존 코루틴을 정리하는 메서드
    /// - 기존에 실행 중인 카메라 이동 코루틴이 있다면 중지하고 새로운 코루틴을 실행
    /// </summary>
    private void StartSmoothTransition(IEnumerator newRoutine)
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }
        activeCoroutine = StartCoroutine(newRoutine);
    }

    /// <summary>
    /// 특정 NPC를 바라보도록 카메라를 회전하고 줌인
    /// </summary>
    /// <param name="target">초점을 맞출 NPC의 Transform</param>
    public void FocusNPC(Transform target)
    {
        StartSmoothTransition(FocusOnTargetRoutine(target));
    }

    /// <summary>
    /// NPC를 바라보도록 카메라를 부드럽게 회전 및 줌인하는 코루틴
    /// </summary>
    /// <param name="target">초점을 맞출 NPC의 Transform</param>
    private IEnumerator FocusOnTargetRoutine(Transform target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);

        // 현재 카메라 회전과 줌이 목표 상태에 도달할 때까지 반복
        while (!Mathf.Approximately(Quaternion.Angle(transform.rotation, targetRotation), 0f) ||
               !Mathf.Approximately(cam.fieldOfView, targetFOV))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
            yield return null;
        }

        transform.rotation = targetRotation;
        cam.fieldOfView = targetFOV;
        activeCoroutine = null;
    }

    /// <summary>
    /// 카메라를 원래 위치 및 줌 상태로 복귀
    /// </summary>
    public void FocusAndReturnToOriginal()
    {
        StartSmoothTransition(ReturnToOriginalRoutine());
    }

    /// <summary>
    /// 원래 위치 및 줌 상태로 부드럽게 복귀하는 코루틴
    /// </summary>
    private IEnumerator ReturnToOriginalRoutine()
    {
        yield return SmoothTransition(originalRotation, originalFOV);
    }

    /// <summary>
    /// 부드러운 카메라 회전 및 줌을 처리하는 공통 메서드
    /// </summary>
    private IEnumerator SmoothTransition(Quaternion targetRotation, float targetFOV)
    {
        // 목표 회전 및 FOV에 도달할 때까지 반복
        while (!Mathf.Approximately(Quaternion.Angle(transform.rotation, targetRotation), 0f) ||
               !Mathf.Approximately(cam.fieldOfView, targetFOV))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
            yield return null;
        }

        // 최종 값 설정
        transform.rotation = targetRotation;
        cam.fieldOfView = targetFOV;
        activeCoroutine = null;     // 실행 중인 코루틴 해제
    }
}
