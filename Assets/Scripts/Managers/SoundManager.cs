using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioClip[] endBGM;              // 0 - 범인 고를 때  // 1 - 범인 고른 후
    public AudioClip[] footStepSounds;
    public AudioClip[] buttonSounds;        // 0 - 나가기  // 1 - 메뉴   // 2 - 화살표  // 3 - 프로필
    public Button[] closeButtons;
    public Button[] menuButtons;
    public Button[] arrowButtons;
    public Button[] profileButtons;
    public StepCycle stepCycleManager;

    public LayerMask groundLayer; // 땅으로 인식할 레이어

    private AudioSource[] audioSources; // 0 - 발소리  // 1 - 타이핑 소리   // 2 - BGM  // 3 - 버튼 클릭 소리
    private CharacterController characterController;
    private GameObject player;

    private float fadeOutDuration = 1f; // 볼륨 감소 시간
    private float fadeInDuration = 1f;  // 볼륨 증가 시간
    private float targetVolume = 1f;    // 최종 목표 볼륨 값

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // 중복된 오브젝트가 있을 경우 파괴
        }
    }

    void Start()
    {
        audioSources = GetComponents<AudioSource>();

        // StepCycle 클래스 초기화
        stepCycleManager = new StepCycle(2f); // 초기 발소리 간격

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
    /// Player 스크립트에서 호출
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
    // 타이핑 소리 관련 메서드

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
        // 버튼 그룹과 사운드 인덱스를 매핑
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
        // 1. 볼륨을 천천히 줄여 0으로 만들기
        float startVolume = audioSources[2].volume;
        for (float t = 0; t < fadeOutDuration; t += Time.deltaTime)
        {
            audioSources[2].volume = Mathf.Lerp(startVolume, 0, t / fadeOutDuration);
            yield return null;
        }
        audioSources[2].volume = 0;

        // 2. 오디오 클립 변경
        audioSources[2].clip = audio;
        audioSources[2].Play();

        // 3. 볼륨을 서서히 증가시켜 목표 볼륨으로 맞추기
        for (float t = 0; t < fadeInDuration; t += Time.deltaTime)
        {
            audioSources[2].volume = Mathf.Lerp(0, targetVolume, t / fadeInDuration);
            yield return null;
        }
        audioSources[2].volume = targetVolume;
    }
}

// 발소리 간격 및 사이클을 관리하는 StepCycle 클래스
public class StepCycle
{
    public float stepInterval; // 발소리 간격 (초 단위)
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

    // 발소리 사이클을 진행하고, 필요할 때 발소리를 재생
    public void ProgressStepCycle<T>(float speed, System.Action<T> playFootstep, T argument)
    {
        stepCycle += (speed + 1f) * Time.deltaTime;

        if (stepCycle > nextStep)
        {
            playFootstep(argument); // 발소리 재생
            nextStep = stepCycle + stepInterval;
        }
    }
}