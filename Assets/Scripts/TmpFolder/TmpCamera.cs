using UnityEngine;

public class TmpCamera : MonoBehaviour
{
    public float mouseSensitivity = 100f; // ���콺 ����
    public Transform playerBody; // �÷��̾� ��ü (ī�޶�� �Բ� ȸ���� ��ü)
    private float xRotation = 0f; // ī�޶��� X�� ȸ�� ��

    public Camera targetCamera; // Inspector���� ������ ī�޶�
    public Transform startObject; // Ray ���� ��ġ�� ����� ������Ʈ

    void LateUpdate()
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

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // ȭ�� �߽��� ��ǥ ���
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);

            // ȭ�� �߽��� ���ϴ� ���� ���� ���� ���
            Ray screenRay = targetCamera.ScreenPointToRay(screenCenter);
            Vector3 direction = screenRay.direction;

            // ���� ������Ʈ ��ġ���� �����Ͽ� ȭ�� �߽� �������� ���ϴ� Ray ����
            Ray ray = new Ray(startObject.position, direction);

            RaycastHit hit;
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 1f); // ���� ����� ǥ��

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Debug.Log(hit.collider.gameObject.name + "��(��) Ŭ���Ǿ����ϴ�!");
            }
            else
            {
                Debug.Log("����ĳ��Ʈ�� �ƹ��͵� ������ ���߽��ϴ�.");
            }
        }
    }

}
