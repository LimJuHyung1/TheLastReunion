using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioClip[] endBGM;              // 0 - ���� �� ��  // 1 - ���� �� ��
    public AudioClip[] footStepSounds;
    public AudioClip[] buttonSounds;        // 0 - ������  // 1 - �޴�   // 2 - ȭ��ǥ  // 3 - ������
    public Button[] closeButtons;
    public Button[] menuButtons;
    public Button[] arrowButtons;
    public Button[] profileButtons;
    public StepCycle stepCycleManager;

    public LayerMask groundLayer; // ������ �ν��� ���̾�

    private AudioSource[] audioSources; // 0 - �߼Ҹ�  // 1 - Ÿ���� �Ҹ�   // 2 - BGM  // 3 - ��ư Ŭ�� �Ҹ�
    private CharacterController characterController;
    private GameObject player;

    private float fadeOutDuration = 1f; // ���� ���� �ð�
    private float fadeInDuration = 1f;  // ���� ���� �ð�
    private float targetVolume = 1f;    // ���� ��ǥ ���� ��

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // �ߺ��� ������Ʈ�� ���� ��� �ı�
        }
    }

    void Start()
    {
        audioSources = GetComponents<AudioSource>();

        // StepCycle Ŭ���� �ʱ�ȭ
        stepCycleManager = new StepCycle(2f); // �ʱ� �߼Ҹ� ����

        audioSources[0].loop = false;

        // audioSources[2].Play();
        SetButtonsSound();

        stepCycleManager.SetFootStepSound(GetComponent<AudioSource>(), footStepSounds[0]);
        SetNullAudioMixerGroup();
    }

    public void SetNullAudioMixerGroup()
    {
        audioSources[0].outputAudioMixerGroup = null;
    }

    void PlayFootstep(AudioClip audioClip)
    {
        if (audioSources[0].clip != null)
        {
            audioSources[0].clip = audioClip;
            audioSources[0].Play();
        }
    }

    /// <summary>
    /// Player ��ũ��Ʈ���� ȣ��
    /// </summary>
    /// <param name="layer"></param>
    public void PlayFootStepSound(string layer)
    {
        player = GameObject.Find("Player");
        characterController = player.GetComponent<CharacterController>();

        switch (layer)
        {
            case "Ground":
                stepCycleManager.ProgressStepCycle(characterController.velocity.magnitude, PlayFootstep, footStepSounds[0]);
                break;
            case "Tile":
                stepCycleManager.ProgressStepCycle(characterController.velocity.magnitude, PlayFootstep, footStepSounds[1]);
                break;
            case "House":
                stepCycleManager.ProgressStepCycle(characterController.velocity.magnitude, PlayFootstep, footStepSounds[2]);
                break;

            default:
                stepCycleManager.ProgressStepCycle(characterController.velocity.magnitude, PlayFootstep, footStepSounds[1]);
                break;
        }
    }

    //-----------------------------------------------//
    // Ÿ���� �Ҹ� ���� �޼���

    public void PlayTextSound()
    {
        if (audioSources[1] != null)
            audioSources[1].Play();
    }

    public void StopTextSound()
    {
        if (audioSources[1] != null)
            audioSources[1].Stop();
    }

    public void ChangeTextAudioClip(AudioClip typeSound)
    {
        if (audioSources[1] != null)
            audioSources[1].clip = typeSound;
    }

    //-----------------------------------------------//

    void SetButtonsSound()
    {
        // ��ư �׷�� ���� �ε����� ����
        AddButtonListeners(closeButtons, 0);
        AddButtonListeners(menuButtons, 1);
        AddButtonListeners(arrowButtons, 2);
        AddButtonListeners(profileButtons, 3);
    }

    void AddButtonListeners(Button[] buttons, int soundIndex)
    {
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => PlayButtonSound(soundIndex));
        }
    }

    void PlayButtonSound(int soundIndex)
    {
        audioSources[3].clip = buttonSounds[soundIndex];
        audioSources[3].Play();
    }

    //-----------------------------------------------//

    public AudioClip GetSelectCriminalBGM()
    {
        return endBGM[0];
    }

    public AudioClip GetEndingBGM()
    {
        return endBGM[1];
    }

    public IEnumerator FadeOutAndChangeClip(AudioClip audio)
    {
        // 1. ������ õõ�� �ٿ� 0���� �����
        float startVolume = audioSources[2].volume;
        for (float t = 0; t < fadeOutDuration; t += Time.deltaTime)
        {
            audioSources[2].volume = Mathf.Lerp(startVolume, 0, t / fadeOutDuration);
            yield return null;
        }
        audioSources[2].volume = 0;

        // 2. ����� Ŭ�� ����
        audioSources[2].clip = audio;
        audioSources[2].Play();

        // 3. ������ ������ �������� ��ǥ �������� ���߱�
        for (float t = 0; t < fadeInDuration; t += Time.deltaTime)
        {
            audioSources[2].volume = Mathf.Lerp(0, targetVolume, t / fadeInDuration);
            yield return null;
        }
        audioSources[2].volume = targetVolume;
    }
}

// �߼Ҹ� ���� �� ����Ŭ�� �����ϴ� StepCycle Ŭ����
public class StepCycle
{
    public float stepInterval; // �߼Ҹ� ���� (�� ����)
    private float stepCycle;
    private float nextStep;

    public StepCycle(float stepInterval)
    {
        this.stepInterval = stepInterval;
        this.stepCycle = 0f;
        this.nextStep = 0f;
    }

    public void SetFootStepSound(AudioSource audio, AudioClip footSound)
    {
        audio.clip = footSound;
    }

    // �߼Ҹ� ����Ŭ�� �����ϰ�, �ʿ��� �� �߼Ҹ��� ���
    public void ProgressStepCycle<T>(float speed, System.Action<T> playFootstep, T argument)
    {
        stepCycle += (speed + 1f) * Time.deltaTime;

        if (stepCycle > nextStep)
        {
            playFootstep(argument); // �߼Ҹ� ���
            nextStep = stepCycle + stepInterval;
        }
    }
}