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
        cam = GetComponent<Camera>(); // 현재 오브젝트의 Camera 컴포넌트 가져오기
        originalFOV = cam.fieldOfView; // 초기 FOV 저장
        originalRotation = transform.rotation; // 초기 회전 값 저장
    }

    /// <summary>
    /// 특정 NPC를 바라보도록 카메라를 회전하고 줌인
    /// </summary>
    /// <param name="target">초점을 맞출 NPC의 Transform</param>
    public void FocusNPC(Transform target)
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine); // 기존 실행 중인 코루틴이 있다면 중지
        }
        activeCoroutine = StartCoroutine(FocusOnTargetRoutine(target)); // 새로운 코루틴 실행
    }

    /// <summary>
    /// NPC를 바라보도록 카메라를 부드럽게 회전 및 줌인하는 코루틴
    /// </summary>
    /// <param name="target">초점을 맞출 NPC의 Transform</param>
    private IEnumerator FocusOnTargetRoutine(Transform target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position); // NPC 방향으로 회전 값 설정

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f || Mathf.Abs(cam.fieldOfView - targetFOV) > 0.1f)
        {
            // 회전을 부드럽게 변경
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            // 줌인 효과를 부드럽게 적용
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
            yield return null; // 다음 프레임까지 대기
        }

        // 최종적으로 목표 위치로 설정
        transform.rotation = targetRotation;
        cam.fieldOfView = targetFOV;

        activeCoroutine = null; // 코루틴 종료 후 초기화
    }

    /// <summary>
    /// 카메라를 원래 위치 및 줌 상태로 복귀
    /// </summary>
    public void FocusAndReturnToOriginal()
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine); // 실행 중인 코루틴 중지
        }
        activeCoroutine = StartCoroutine(ReturnToOriginalRoutine()); // 원래 상태로 되돌리는 코루틴 실행
    }

    /// <summary>
    /// 원래 카메라 위치 및 줌 상태로 부드럽게 복귀하는 코루틴
    /// </summary>
    private IEnumerator ReturnToOriginalRoutine()
    {
        while (Quaternion.Angle(transform.rotation, originalRotation) > 0.1f || Mathf.Abs(cam.fieldOfView - originalFOV) > 0.1f)
        {
            // 회전을 원래 상태로 부드럽게 변경
            transform.rotation = Quaternion.Lerp(transform.rotation, originalRotation, Time.deltaTime * rotationSpeed);
            // 줌을 원래 상태로 부드럽게 변경
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, originalFOV, Time.deltaTime * zoomSpeed);
            yield return null; // 다음 프레임까지 대기
        }

        // 최종적으로 원래 위치로 설정
        transform.rotation = originalRotation;
        cam.fieldOfView = originalFOV;

        activeCoroutine = null; // 코루틴 종료 후 초기화
    }
}
