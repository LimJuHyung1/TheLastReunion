using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioClip[] footStepSounds;
    public AudioClip[] dispatchSounds;
    public StepCycle stepCycleManager;

    public LayerMask groundLayer; // 땅으로 인식할 레이어

    private short dispatchIndex = 0;
    private AudioSource[] audioSources; // 0 - 발소리  // 1 - 타이핑 소리
    private CharacterController characterController;
    private GameObject player;
    private RaycastHit hit;

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

    public void PlayDispatchSound()
    {
        audioSources[0].clip = dispatchSounds[dispatchIndex];
        audioSources[0].Play();
        dispatchIndex++;
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
