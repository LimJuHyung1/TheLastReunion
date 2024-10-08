using UnityEngine;

public class TmpCamera : MonoBehaviour
{
    public float mouseSensitivity = 100f; // ���콺 ����
    public Transform playerBody; // �÷��̾� ��ü (ī�޶�� �Բ� ȸ���� ��ü)
    private float xRotation = 0f; // ī�޶��� X�� ȸ�� ��

    void Update()
    {
        // ���콺 X�� �� Y�� �Է� �� ��������
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Y�� ȸ�� �� ������Ʈ (X�� ȸ��)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // ���� ȸ���� ����

        // ī�޶��� ���� ȸ�� ����
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // �÷��̾��� ��ü�� �¿� ȸ����Ű�� (Y�� ȸ��)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
