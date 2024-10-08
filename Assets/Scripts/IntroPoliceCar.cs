using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroPoliceCar : MonoBehaviour
{
    AudioSource audio;
    public int loopCount; // �ݺ� Ƚ�� ����

    short currentLoop = 0;
    float currentTime = 0f;
    float targetVolume = 0.6f; // ��ǥ ����
    float fadeInDuration = 4f; // ���̵� �� ���� �ð� (�� ����)
    float fadeOutDuration = 4f; // ���̵� �ƿ� ���� �ð� (�� ����)
    float moveSpeed = 2f; // �������� ������ �̵��ϴ� �ӵ�

    void Start()
    {
        audio = GetComponent<AudioSource>();

        // AudioSource ����
        audio.volume = 0f; // �ʱ� ������ 0���� ����
        audio.loop = false; // Loop�� ��Ȱ��ȭ�Ͽ� ���� ����
        audio.Play();
        StartCoroutine(StartSiren());
    }

    void Update()
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

    void FixedUpdate()
    {
        // �� �������� �̵� (z�� ����)
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
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
}
