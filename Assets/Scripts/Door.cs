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

    // 오디오 소스 초기화
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
        if (isOpened) return; // 이미 열린 경우 중복 실행 방지
        isOpened = true;
        audio.Play();
        StartCoroutine(OpenCoroutine());
    }

    public bool IsOpened() => isOpened;

    public float SetOpenAngle(float angle) => this.openAngle = angle;

    // 문 열기 코루틴
    private IEnumerator OpenCoroutine()
    {
        float targetAngle = transform.eulerAngles.y + openAngle;
        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.1f)
        {
            float newYRotation = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, openSpeed * Time.deltaTime);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, newYRotation, transform.eulerAngles.z);
            yield return null;
        }

        // 정확한 각도로 설정
        Vector3 finalRotation = transform.eulerAngles;
        finalRotation.y = targetAngle;
        transform.eulerAngles = finalRotation;
    }

    // AudioClip 설정 메서드
    public void SetAudioClip(AudioClip clip)
    {

        audio.clip = clip;
    }
}
