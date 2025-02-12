using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class IntroPoliceCar : MonoBehaviour
{
    AudioSource audio;

    public int loopCount; // �ݺ� Ƚ�� ����
    [SerializeField] short currentLoop = 0;

    public GameObject[] wheels;
    
    float currentTime = 0f;
    float wheelRotationSpeed = 120f; // ���� ȸ�� �ӵ� ����
    float targetVolume = 0.6f; // ��ǥ ����
    float fadeInDuration = 4f; // ���̵� �� ���� �ð� (�� ����)
    float fadeOutDuration = 4f; // ���̵� �ƿ� ���� �ð� (�� ����)
    float moveSpeed = 2f; // �������� ������ �̵��ϴ� �ӵ�

    bool isEndSetQuality = false;

    void Start()
    {
        audio = GetComponent<AudioSource>();

        // AudioSource ����
        audio.volume = 0f; // �ʱ� ������ 0���� ����
        audio.loop = false; // Loop�� ��Ȱ��ȭ�Ͽ� ���� ����
        audio.Stop();
    }

    void Update()
    {
        if (isEndSetQuality)
        {
            // AudioSource�� ���߸� �ݺ� Ƚ���� üũ�Ͽ� ���
            if (!audio.isPlaying && currentLoop < loopCount)
            {
                // Ƚ���� �� ä�� ���
                if (currentLoop == loopCount)
                {
                    StopCoroutine(StartSiren());
                    StartCoroutine(EndSiren());
                }
                // ���� ���� �� �Ҹ� ���
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
            // �� �������� �̵� (z�� ����)
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

        // ���� �ð����� fadeInDuration ���� ����
        while (currentTime < fadeInDuration)
        {
            currentTime += Time.deltaTime;
            audio.volume = Mathf.Lerp(0f, targetVolume, currentTime / fadeInDuration);
            yield return null; // �� ������ ���
        }

        audio.volume = targetVolume; // ��ǥ ������ �����ϵ��� ����
    }

    private IEnumerator EndSiren()
    {
        currentTime = 0f;
        float startVolume = audio.volume;

        // ���� �ð����� fadeOutDuration ���� ����
        while (currentTime < fadeOutDuration)
        {
            currentTime += Time.deltaTime;
            audio.volume = Mathf.Lerp(startVolume, 0f, currentTime / fadeOutDuration);
            yield return null; // �� ������ ���
        }

        audio.volume = 0f; // ������ 0���� �����Ͽ� ���̵� �ƿ� �Ϸ�
        audio.Stop(); // ����� ����
        StopCoroutine(EndSiren());
    }

    public void ReadyToSiren()
    {
        isEndSetQuality = true;
    }
}

