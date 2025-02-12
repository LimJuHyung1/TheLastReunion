using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class IntroPoliceCar : MonoBehaviour
{
    AudioSource audio;

    public int loopCount; // 반복 횟수 설정
    [SerializeField] short currentLoop = 0;

    public GameObject[] wheels;
    
    float currentTime = 0f;
    float wheelRotationSpeed = 120f; // 바퀴 회전 속도 설정
    float targetVolume = 0.6f; // 목표 볼륨
    float fadeInDuration = 4f; // 페이드 인 지속 시간 (초 단위)
    float fadeOutDuration = 4f; // 페이드 아웃 지속 시간 (초 단위)
    float moveSpeed = 2f; // 경찰차가 앞으로 이동하는 속도

    bool isEndSetQuality = false;

    void Start()
    {
        audio = GetComponent<AudioSource>();

        // AudioSource 설정
        audio.volume = 0f; // 초기 볼륨을 0으로 설정
        audio.loop = false; // Loop를 비활성화하여 직접 제어
        audio.Stop();
    }

    void Update()
    {
        if (isEndSetQuality)
        {
            // AudioSource가 멈추면 반복 횟수를 체크하여 재생
            if (!audio.isPlaying && currentLoop < loopCount)
            {
                // 횟수를 다 채운 경우
                if (currentLoop == loopCount)
                {
                    StopCoroutine(StartSiren());
                    StartCoroutine(EndSiren());
                }
                // 게임 시작 시 소리 출력
                else
                {
                    audio.Play();
                }
                currentLoop++;
            }
        }
    }

    void FixedUpdate()
    {
        if (isEndSetQuality)
        {
            // 앞 방향으로 이동 (z축 기준)
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

            foreach (var wheel in wheels)
            {
                wheel.transform.Rotate(wheelRotationSpeed * Time.deltaTime, 0, 0);
            }
        }
    }

    public void StartPoliceCar()
    {        
        audio.Play();
        StartCoroutine(StartSiren());
    }

    private IEnumerator StartSiren()
    {
        currentTime = 0f;

        // 현재 시간부터 fadeInDuration 동안 실행
        while (currentTime < fadeInDuration)
        {
            currentTime += Time.deltaTime;
            audio.volume = Mathf.Lerp(0f, targetVolume, currentTime / fadeInDuration);
            yield return null; // 한 프레임 대기
        }

        audio.volume = targetVolume; // 목표 볼륨에 도달하도록 보정
    }

    private IEnumerator EndSiren()
    {
        currentTime = 0f;
        float startVolume = audio.volume;

        // 현재 시간부터 fadeOutDuration 동안 실행
        while (currentTime < fadeOutDuration)
        {
            currentTime += Time.deltaTime;
            audio.volume = Mathf.Lerp(startVolume, 0f, currentTime / fadeOutDuration);
            yield return null; // 한 프레임 대기
        }

        audio.volume = 0f; // 볼륨을 0으로 설정하여 페이드 아웃 완료
        audio.Stop(); // 오디오 중지
        StopCoroutine(EndSiren());
    }

    public void ReadyToSiren()
    {
        isEndSetQuality = true;
    }
}

