using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isOpened = false;
    
    public Vector3 guideMoveVec;
    public Vector3 guideRot;
    public float openAngle;

    private AudioSource audio;
    private float openSpeed = 40f;

    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    //---------------------------------------------------------------//

    public void Open()
    {
        isOpened = true;
        audio.Play();

        StartCoroutine(OpenCoroutine());
    }

    // �� ����
    public IEnumerator OpenCoroutine()
    {
        float targetAngle = transform.eulerAngles.y + openAngle;
        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.1f)
        {
            float newYRotation = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, openSpeed * Time.deltaTime);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, newYRotation, transform.eulerAngles.z);

            yield return null;
        }

        // ��Ȯ�� 120���� ����
        Vector3 finalRotation = transform.eulerAngles;
        finalRotation.y = targetAngle;
        transform.eulerAngles = finalRotation;
    }   

    // DoorManagers���� Awake�� ����
    public void SetAudio()
    {
        audio = GetComponent<AudioSource>();
    }
}
