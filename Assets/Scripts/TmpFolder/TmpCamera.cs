using UnityEngine;

public class TmpCamera : MonoBehaviour
{
    public float mouseSensitivity = 100f; // 마우스 감도
    public Transform playerBody; // 플레이어 몸체 (카메라와 함께 회전할 객체)
    private float xRotation = 0f; // 카메라의 X축 회전 값

    public Camera targetCamera; // Inspector에서 설정할 카메라
    public Transform startObject; // Ray 시작 위치로 사용할 오브젝트

    void LateUpdate()
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

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 화면 중심의 좌표 계산
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);

            // 화면 중심을 향하는 월드 방향 벡터 계산
            Ray screenRay = targetCamera.ScreenPointToRay(screenCenter);
            Vector3 direction = screenRay.direction;

            // 시작 오브젝트 위치에서 시작하여 화면 중심 방향으로 향하는 Ray 생성
            Ray ray = new Ray(startObject.position, direction);

            RaycastHit hit;
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 1f); // 레이 디버그 표시

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Debug.Log(hit.collider.gameObject.name + "이(가) 클릭되었습니다!");
            }
            else
            {
                Debug.Log("레이캐스트가 아무것도 맞추지 못했습니다.");
            }
        }
    }

}
