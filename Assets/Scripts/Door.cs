using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Vector3 guideMoveVec;
    public Vector3 guideRot;
    private float openAngle;
    private bool isOpened = false;
    private float openSpeed = 80f;

    private AudioSource audio;

    void Awake()
    {
        SetAudioSource();
    }

    // ����� �ҽ� �ʱ�ȭ
    private void SetAudioSource()
    {
        audio = GetComponent<AudioSource>();
        if (audio == null)
        {
            audio = gameObject.AddComponent<AudioSource>();
        }
    }

    public void Open()
    {
        if (isOpened) return; // �̹� ���� ��� �ߺ� ���� ����
        isOpened = true;
        audio.Play();
        StartCoroutine(OpenCoroutine());
    }

    public bool IsOpened() => isOpened;

    public float SetOpenAngle(float angle) => this.openAngle = angle;

    // �� ���� �ڷ�ƾ
    private IEnumerator OpenCoroutine()
    {
        float targetAngle = transform.eulerAngles.y + openAngle;
        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.1f)
        {
            float newYRotation = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, openSpeed * Time.deltaTime);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, newYRotation, transform.eulerAngles.z);
            yield return null;
        }

        // ��Ȯ�� ������ ����
        Vector3 finalRotation = transform.eulerAngles;
        finalRotation.y = targetAngle;
        transform.eulerAngles = finalRotation;
    }

    // AudioClip ���� �޼���
    public void SetAudioClip(AudioClip clip)
    {

        audio.clip = clip;
    }
}
