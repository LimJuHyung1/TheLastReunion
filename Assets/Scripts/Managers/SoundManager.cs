using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }   // �̱��� ���� ����

    [Header("Sounds")]
    public AudioClip typingClip;            // Ÿ���� ȿ����
    public AudioClip[] endBGM;              // 0 - ���� �� ��  // 1 - ���� �� ��
    public AudioClip[] footStepSounds;      // �߼Ҹ� ȿ����
    public AudioClip[] buttonSounds;        // ��ư Ŭ�� ȿ���� (0: ������, 1: �޴�, 2: ȭ��ǥ, 3: ������)

    [Header("Buttons")]
    public Button[] closeButtons;
    public Button[] menuButtons;
    public Button[] arrowButtons;
    public Button[] profileButtons;

    [Header("Footstep SoundManager")]
    public StepCycle stepCycleManager;       // �߼Ҹ� �ֱ⸦ �����ϴ� Ŭ����

    public LayerMask groundLayer;            // ������ �ν��� ���̾�

    private AudioSource[] audioSources;      // 0 - �߼Ҹ�  // 1 - Ÿ���� �Ҹ�   // 2 - BGM  // 3 - ��ư Ŭ�� �Ҹ�
    private CharacterController characterController;
    private GameObject player;

    private float fadeOutDuration = 1f; // ���� ���� �ð�
    private float fadeInDuration = 1f;  // ���� ���� �ð�
    private float targetVolume = 1f;    // ���� ��ǥ ���� ��

    private Dictionary<string, AudioClip> footstepMap;  // �߼Ҹ� ���� ����

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

        SetButtonsSound();   // ��ư ���� ����

        // �߼Ҹ� ���� �ʱ�ȭ
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
    /// ����� �ͼ��� null�� �����Ͽ� �⺻ ����� ����ϵ��� ����
    /// </summary>
    public void SetNullAudioMixerGroup()
    {
        audioSources[0].outputAudioMixerGroup = null;
    }

    /// <summary>
    /// �߼Ҹ��� ����ϴ� �޼���
    /// </summary>
    void PlayFootstep(AudioClip audioClip)
    {
        audioSources[0].clip = audioClip;
        audioSources[0].Play();
    }

    /// <summary>
    /// �÷��̾��� ���� �ٴ� ���̾ ���� �߼Ҹ��� ���
    /// </summary>
    /// <param ��������_���̾�="layer">�÷��̾ �� �ִ� �ٴ��� ���̾�</param>
    public void PlayFootStepSound(string layer)
    {
        player = GameObject.Find("Player");
        characterController = player.GetComponent<CharacterController>();

        if (!footstepMap.TryGetValue(layer, out AudioClip clip))
        {
            clip = footStepSounds[1]; // �⺻��
        }

        // �÷��̾� �ӵ��� ���� �߼Ҹ� �ֱ⸦ ����
        stepCycleManager.ProgressStepCycle(characterController.velocity.magnitude, PlayFootstep, clip);
    }

    //-----------------------------------------------//
    // Ÿ���� �Ҹ� ���� �޼���

    /// <summary>
    /// Ÿ���� ȿ������ ���
    /// </summary>
    public void PlayTextSound()
    {
        if (audioSources[1] != null)
            audioSources[1].Play();
    }

    /// <summary>
    /// Ÿ���� ȿ������ ����
    /// </summary>
    public void StopTextSound()
    {
        if (audioSources[1] != null)
            audioSources[1].Stop();
    }

    /// <summary>
    /// Ÿ���� ȿ������ ����
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
    /// ��ư Ŭ�� ���� ����
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
    /// ��ư Ŭ�� �� ���带 ����ϵ��� ������ �߰�
    /// </summary>
    void AddButtonListeners(Button[] buttons, int soundIndex)
    {
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => PlayButtonSound(soundIndex));
        }
    }

    /// <summary>
    /// ��ư Ŭ�� �� ���� ���
    /// </summary>
    void PlayButtonSound(int soundIndex)
    {
        audioSources[3].clip = buttonSounds[soundIndex];
        audioSources[3].Play();
    }

    //-----------------------------------------------//

    /// <summary>
    /// ���� ���� �� BGM ��ȯ
    /// </summary>
    public AudioClip GetSelectCriminalBGM()
    {
        return endBGM[0];
    }

    /// <summary>
    /// ���� BGM ��ȯ
    /// </summary>
    public AudioClip GetEndingBGM()
    {
        return endBGM[1];
    }

    /// <summary>
    /// BGM�� ������ ���� �� �����ϴ� �ڷ�ƾ
    /// </summary>
    public IEnumerator FadeOutAndChangeClip(AudioClip audio)
    {
        // ������ õõ�� 0���� ����
        float startVolume = audioSources[2].volume;
        float t = 0f;

        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            audioSources[2].volume = Mathf.Lerp(startVolume, 0, t / fadeOutDuration);
            yield return null;
        }

        audioSources[2].volume = 0; // ������ 0���� ���� ����
        audioSources[2].clip = audio;
        audioSources[2].Play();

        t = 0f;
        // ������ �ٽ� ��ǥ �������� ����
        while (t < fadeInDuration)
        {
            t += Time.deltaTime;
            audioSources[2].volume = Mathf.Lerp(0, targetVolume, t / fadeInDuration);
            yield return null;
        }

        audioSources[2].volume = targetVolume; // ��ǥ ���� ����
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