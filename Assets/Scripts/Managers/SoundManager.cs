using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioClip[] footStepSounds;
    public AudioClip[] dispatchSounds;
    public StepCycle stepCycleManager;

    public LayerMask groundLayer; // ������ �ν��� ���̾�

    private short dispatchIndex = 0;
    private AudioSource[] audioSources; // 0 - �߼Ҹ�  // 1 - Ÿ���� �Ҹ�
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
            Destroy(gameObject); // �ߺ��� ������Ʈ�� ���� ��� �ı�
        }
    }

    void Start()
    {
        audioSources = GetComponents<AudioSource>();

        // StepCycle Ŭ���� �ʱ�ȭ
        stepCycleManager = new StepCycle(2f); // �ʱ� �߼Ҹ� ����

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

    public void PlayDispatchSound()
    {
        audioSources[0].clip = dispatchSounds[dispatchIndex];
        audioSources[0].Play();
        dispatchIndex++;
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
