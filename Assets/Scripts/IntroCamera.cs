using UnityEngine;

public class IntroCamera : MonoBehaviour
{
    public Transform policeCar;  // ������ �������� Transform
    Vector3 offset = new Vector3(1, 1, -1); // ī�޶� �������� �밢�� ���ʿ� ��ġ

    void LateUpdate()
    {
        Vector3 desiredPosition = policeCar.position + policeCar.TransformDirection(offset);
        transform.position = desiredPosition;

        transform.LookAt(policeCar);
    }
}
