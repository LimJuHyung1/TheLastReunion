using UnityEngine;

public class IntroCamera : MonoBehaviour
{
    public Transform policeCar;  // 추적할 경찰차의 Transform
    Vector3 offset = new Vector3(1, 1, -1); // 카메라를 경찰차의 대각선 뒤쪽에 배치

    void LateUpdate()
    {
        Vector3 desiredPosition = policeCar.position + policeCar.TransformDirection(offset);
        transform.position = desiredPosition;

        transform.LookAt(policeCar);
    }
}
