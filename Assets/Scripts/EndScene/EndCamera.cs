using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EndCamera : MonoBehaviour
{    
    private float rotationSpeed = 5.0f; // ȸ�� �ӵ�
    private float zoomSpeed = 5.0f; // Ȯ�� �ӵ�
    public float targetFOV = 30.0f; // Ÿ���� �ٶ� ���� �� ����
    private float originalFOV; // ���� ī�޶� �� ����
    private Quaternion originalRotation; // ���� ī�޶� ȸ�� ��

    private Camera cam; // ī�޶� ������Ʈ

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        originalFOV = cam.fieldOfView; // �ʱ� �� ���� ����
        originalRotation = transform.rotation; // �ʱ� ȸ�� �� ����
    }

    public void FocusNPC(Transform target)
    {
        StartCoroutine(FocusOnTargetRoutine(target));
    }

    private IEnumerator FocusOnTargetRoutine(Transform target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);

        // Ÿ���� �ٶ󺸸� Ȯ��
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f || Mathf.Abs(cam.fieldOfView - targetFOV) > 0.1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
            yield return null;
        }

        // ���������� ��Ȯ�� ��ǥ ���·� ����
        transform.rotation = targetRotation;
        cam.fieldOfView = targetFOV;
    }

    /// <summary>
    /// Player ��ũ��Ʈ���� ȣ���Ͽ� Ÿ���� Ȯ���ϸ� �ٶ󺾴ϴ�.
    /// </summary>
    public void FocusAndReturnToOriginal()
    {
        StartCoroutine(ReturnToOriginalRoutine());
    }

    private IEnumerator ReturnToOriginalRoutine()
    {
        // ���� ȸ���� FOV�� ����
        while (Quaternion.Angle(transform.rotation, originalRotation) > 0.1f || Mathf.Abs(cam.fieldOfView - originalFOV) > 0.1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, originalRotation, Time.deltaTime * rotationSpeed);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, originalFOV, Time.deltaTime * zoomSpeed);
            yield return null;
        }

        // ���������� ��Ȯ�� ���� ���·� ����
        transform.rotation = originalRotation;
        cam.fieldOfView = originalFOV;
    }
}
