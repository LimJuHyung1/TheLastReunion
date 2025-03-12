using UnityEngine;
using System.Collections;

/// <summary>
/// Ư�� NPC�� �ٶ󺸵��� ī�޶� ȸ����Ű�� ������ �����ϴ� Ŭ����.
/// NPC�� �������� ���߰ų� ���� ��ġ�� �����ϴ� ����� ����.
/// </summary>
public class EndCamera : MonoBehaviour
{
    private float rotationSpeed = 5.0f; // ī�޶� ȸ�� �ӵ�
    private float zoomSpeed = 5.0f; // ī�޶� �� �ӵ�
    private float targetFOV = 30.0f; // ���� �� ��ǥ FOV (Field of View)
    private float originalFOV; // ���� ī�޶� FOV ��

    private Quaternion originalRotation; // ���� ī�޶� ȸ�� ��
    private Camera cam; // ī�޶� ������Ʈ ����

    private Coroutine activeCoroutine; // ���� ���� ���� �ڷ�ƾ�� �����Ͽ� �ߺ� ���� ����

    /// <summary>
    /// ī�޶��� �ʱ� ���¸� ����
    /// </summary>
    void Start()
    {
        cam = GetComponent<Camera>();
        ResetOriginalState();
    }

    /// <summary>
    /// ���� ī�޶� ���¸� ���� ���·� ���� (�ʱ�ȭ ����)
    /// </summary>
    public void ResetOriginalState()
    {
        originalFOV = cam.fieldOfView;
        originalRotation = transform.rotation;
    }

    /// <summary>
    /// ���ο� ī�޶� �̵� �ڷ�ƾ�� �����ϱ� ���� ���� �ڷ�ƾ�� �����ϴ� �޼���
    /// - ������ ���� ���� ī�޶� �̵� �ڷ�ƾ�� �ִٸ� �����ϰ� ���ο� �ڷ�ƾ�� ����
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
    /// Ư�� NPC�� �ٶ󺸵��� ī�޶� ȸ���ϰ� ����
    /// </summary>
    /// <param name="target">������ ���� NPC�� Transform</param>
    public void FocusNPC(Transform target)
    {
        StartSmoothTransition(FocusOnTargetRoutine(target));
    }

    /// <summary>
    /// NPC�� �ٶ󺸵��� ī�޶� �ε巴�� ȸ�� �� �����ϴ� �ڷ�ƾ
    /// </summary>
    /// <param name="target">������ ���� NPC�� Transform</param>
    private IEnumerator FocusOnTargetRoutine(Transform target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);

        // ���� ī�޶� ȸ���� ���� ��ǥ ���¿� ������ ������ �ݺ�
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
    /// ī�޶� ���� ��ġ �� �� ���·� ����
    /// </summary>
    public void FocusAndReturnToOriginal()
    {
        StartSmoothTransition(ReturnToOriginalRoutine());
    }

    /// <summary>
    /// ���� ��ġ �� �� ���·� �ε巴�� �����ϴ� �ڷ�ƾ
    /// </summary>
    private IEnumerator ReturnToOriginalRoutine()
    {
        yield return SmoothTransition(originalRotation, originalFOV);
    }

    /// <summary>
    /// �ε巯�� ī�޶� ȸ�� �� ���� ó���ϴ� ���� �޼���
    /// </summary>
    private IEnumerator SmoothTransition(Quaternion targetRotation, float targetFOV)
    {
        // ��ǥ ȸ�� �� FOV�� ������ ������ �ݺ�
        while (!Mathf.Approximately(Quaternion.Angle(transform.rotation, targetRotation), 0f) ||
               !Mathf.Approximately(cam.fieldOfView, targetFOV))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
            yield return null;
        }

        // ���� �� ����
        transform.rotation = targetRotation;
        cam.fieldOfView = targetFOV;
        activeCoroutine = null;     // ���� ���� �ڷ�ƾ ����
    }
}
