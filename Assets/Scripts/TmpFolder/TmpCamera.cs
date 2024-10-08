using UnityEngine;

public class TmpCamera : MonoBehaviour
{
    public float mouseSensitivity = 100f; // 마우스 감도
    public Transform playerBody; // 플레이어 몸체 (카메라와 함께 회전할 객체)
    private float xRotation = 0f; // 카메라의 X축 회전 값

    void Update()
    {
        // 마우스 X축 및 Y축 입력 값 가져오기
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Y축 회전 값 업데이트 (X축 회전)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // 상하 회전을 제한

        // 카메라의 로컬 회전 적용
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 플레이어의 몸체를 좌우 회전시키기 (Y축 회전)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
