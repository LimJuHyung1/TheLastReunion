using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

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