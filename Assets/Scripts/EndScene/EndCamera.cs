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
        cam = GetComponent<Camera>(); // ���� ������Ʈ�� Camera ������Ʈ ��������
        originalFOV = cam.fieldOfView; // �ʱ� FOV ����
        originalRotation = transform.rotation; // �ʱ� ȸ�� �� ����
    }

    /// <summary>
    /// Ư�� NPC�� �ٶ󺸵��� ī�޶� ȸ���ϰ� ����
    /// </summary>
    /// <param name="target">������ ���� NPC�� Transform</param>
    public void FocusNPC(Transform target)
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine); // ���� ���� ���� �ڷ�ƾ�� �ִٸ� ����
        }
        activeCoroutine = StartCoroutine(FocusOnTargetRoutine(target)); // ���ο� �ڷ�ƾ ����
    }

    /// <summary>
    /// NPC�� �ٶ󺸵��� ī�޶� �ε巴�� ȸ�� �� �����ϴ� �ڷ�ƾ
    /// </summary>
    /// <param name="target">������ ���� NPC�� Transform</param>
    private IEnumerator FocusOnTargetRoutine(Transform target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position); // NPC �������� ȸ�� �� ����

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f || Mathf.Abs(cam.fieldOfView - targetFOV) > 0.1f)
        {
            // ȸ���� �ε巴�� ����
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            // ���� ȿ���� �ε巴�� ����
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
            yield return null; // ���� �����ӱ��� ���
        }

        // ���������� ��ǥ ��ġ�� ����
        transform.rotation = targetRotation;
        cam.fieldOfView = targetFOV;

        activeCoroutine = null; // �ڷ�ƾ ���� �� �ʱ�ȭ
    }

    /// <summary>
    /// ī�޶� ���� ��ġ �� �� ���·� ����
    /// </summary>
    public void FocusAndReturnToOriginal()
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine); // ���� ���� �ڷ�ƾ ����
        }
        activeCoroutine = StartCoroutine(ReturnToOriginalRoutine()); // ���� ���·� �ǵ����� �ڷ�ƾ ����
    }

    /// <summary>
    /// ���� ī�޶� ��ġ �� �� ���·� �ε巴�� �����ϴ� �ڷ�ƾ
    /// </summary>
    private IEnumerator ReturnToOriginalRoutine()
    {
        while (Quaternion.Angle(transform.rotation, originalRotation) > 0.1f || Mathf.Abs(cam.fieldOfView - originalFOV) > 0.1f)
        {
            // ȸ���� ���� ���·� �ε巴�� ����
            transform.rotation = Quaternion.Lerp(transform.rotation, originalRotation, Time.deltaTime * rotationSpeed);
            // ���� ���� ���·� �ε巴�� ����
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, originalFOV, Time.deltaTime * zoomSpeed);
            yield return null; // ���� �����ӱ��� ���
        }

        // ���������� ���� ��ġ�� ����
        transform.rotation = originalRotation;
        cam.fieldOfView = originalFOV;

        activeCoroutine = null; // �ڷ�ƾ ���� �� �ʱ�ȭ
    }
}
