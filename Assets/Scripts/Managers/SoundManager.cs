using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }   // 싱글톤 패턴 적용

    [Header("Sounds")]
    public AudioClip typingClip;            // 타이핑 효과음
    public AudioClip[] endBGM;              // 0 - 범인 고를 때  // 1 - 범인 고른 후
    public AudioClip[] footStepSounds;      // 발소리 효과음
    public AudioClip[] buttonSounds;        // 버튼 클릭 효과음 (0: 나가기, 1: 메뉴, 2: 화살표, 3: 프로필)

    [Header("Buttons")]
    public Button[] closeButtons;
    public Button[] menuButtons;
    public Button[] arrowButtons;
    public Button[] profileButtons;

    [Header("Footstep SoundManager")]
    public StepCycle stepCycleManager;       // 발소리 주기를 관리하는 클래스

    public LayerMask groundLayer;            // 땅으로 인식할 레이어

    private AudioSource[] audioSources;      // 0 - 발소리  // 1 - 타이핑 소리   // 2 - BGM  // 3 - 버튼 클릭 소리
    private CharacterController characterController;
    private GameObject player;

    private float fadeOutDuration = 1f; // 볼륨 감소 시간
    private float fadeInDuration = 1f;  // 볼륨 증가 시간
    private float targetVolume = 1f;    // 최종 목표 볼륨 값

    private Dictionary<string, AudioClip> footstepMap;  // 발소리 사운드 매핑

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

        SetButtonsSound();   // 버튼 사운드 설정

        // 발소리 맵핑 초기화
        footstepMap = new Dictionary<string, AudioClip>
        {
            { "Ground", footStepSounds[0] },
            { "Tile", footStepSounds[1] },
            { "House", footStepSounds[2] }
        };

        stepCycleManager.SetFootStepSound(GetComponent<AudioSource>(), footStepSounds[0]);

        SetNullAudioMixerGroup();
    }

    /// <summary>
    /// 오디오 믹서를 null로 설정하여 기본 출력을 사용하도록 변경
    /// </summary>
    public void SetNullAudioMixerGroup()
    {
        audioSources[0].outputAudioMixerGroup = null;
    }

    /// <summary>
    /// 발소리를 재생하는 메서드
    /// </summary>
    void PlayFootstep(AudioClip audioClip)
    {
        audioSources[0].clip = audioClip;
        audioSources[0].Play();
    }

    /// <summary>
    /// 플레이어의 현재 바닥 레이어에 따라 발소리를 재생
    /// </summary>
    /// <param 접촉중인_레이어="layer">플레이어가 서 있는 바닥의 레이어</param>
    public void PlayFootStepSound(string layer)
    {
        player = GameObject.Find("Player");
        characterController = player.GetComponent<CharacterController>();

        if (!footstepMap.TryGetValue(layer, out AudioClip clip))
        {
            clip = footStepSounds[1]; // 기본값
        }

        // 플레이어 속도에 맞춰 발소리 주기를 적용
        stepCycleManager.ProgressStepCycle(characterController.velocity.magnitude, PlayFootstep, clip);
    }

    //-----------------------------------------------//
    // 타이핑 소리 관련 메서드

    /// <summary>
    /// 타이핑 효과음을 재생
    /// </summary>
    public void PlayTextSound()
    {
        if (audioSources[1] != null)
            audioSources[1].Play();
    }

    /// <summary>
    /// 타이핑 효과음을 정지
    /// </summary>
    public void StopTextSound()
    {
        if (audioSources[1] != null)
            audioSources[1].Stop();
    }

    /// <summary>
    /// 타이핑 효과음을 변경
    /// </summary>
    public void SetTypingClip()
    {
        if (audioSources[1] != null)
            audioSources[1].clip = typingClip;
    }

    public void ChangeTextAudioClip(AudioClip typeSound)
    {
        if (audioSources[1] != null)
            audioSources[1].clip = typeSound;
    }

    //-----------------------------------------------//

    /// <summary>
    /// 버튼 클릭 사운드 설정
    /// </summary>
    void SetButtonsSound()
    {
        Dictionary<Button[], int> buttonMapping = new Dictionary<Button[], int>
        {
            { closeButtons, 0 },
            { menuButtons, 1 },
            { arrowButtons, 2 },
            { profileButtons, 3 }
        };

        foreach (var entry in buttonMapping)
        {
            AddButtonListeners(entry.Key, entry.Value);
        }
    }

    /// <summary>
    /// 버튼 클릭 시 사운드를 재생하도록 리스너 추가
    /// </summary>
    void AddButtonListeners(Button[] buttons, int soundIndex)
    {
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => PlayButtonSound(soundIndex));
        }
    }

    /// <summary>
    /// 버튼 클릭 시 사운드 재생
    /// </summary>
    void PlayButtonSound(int soundIndex)
    {
        audioSources[3].clip = buttonSounds[soundIndex];
        audioSources[3].Play();
    }

    //-----------------------------------------------//

    /// <summary>
    /// 범인 선택 씬 BGM 반환
    /// </summary>
    public AudioClip GetSelectCriminalBGM()
    {
        return endBGM[0];
    }

    /// <summary>
    /// 엔딩 BGM 반환
    /// </summary>
    public AudioClip GetEndingBGM()
    {
        return endBGM[1];
    }

    /// <summary>
    /// BGM을 서서히 줄인 후 변경하는 코루틴
    /// </summary>
    public IEnumerator FadeOutAndChangeClip(AudioClip audio)
    {
        // 볼륨을 천천히 0으로 감소
        float startVolume = audioSources[2].volume;
        float t = 0f;

        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            audioSources[2].volume = Mathf.Lerp(startVolume, 0, t / fadeOutDuration);
            yield return null;
        }

        audioSources[2].volume = 0; // 볼륨을 0으로 강제 설정
        audioSources[2].clip = audio;
        audioSources[2].Play();

        t = 0f;
        // 볼륨을 다시 목표 볼륨까지 증가
        while (t < fadeInDuration)
        {
            t += Time.deltaTime;
            audioSources[2].volume = Mathf.Lerp(0, targetVolume, t / fadeInDuration);
            yield return null;
        }

        audioSources[2].volume = targetVolume; // 목표 볼륨 설정
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