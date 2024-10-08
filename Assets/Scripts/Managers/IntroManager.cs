using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    public Image screen;
    public Text[] introTexts;

    public FadeUtility fadeUtility;
    public GameObject rainPrefab;
    public SoundManager soundManager;

    short textIndex = 0;
    int currentSceneIndex;
    float verticalSpacing = 70f; // �ؽ�Ʈ ���� ���� ����

    string[] dispatch = {
            "���� 42, ��� ��ȣ 1375�� ���� �ٶ��ϴ�.",
            "1234 ���� ��Ʈ��Ʈ���� ���� ��� �߻�.",
            "�����ڴ� 30�� ����, �̸��� �ٷ�, ������� �Ҹ�.",
            "�����ڴ� �������� �������� �ʴ���� 3������ ����.",
            "��� ������ �����Ͽ� ������ ã�Ƴ��ʽÿ�!",
        };

    string officerResponse = "���� 42, Ȯ���߽��ϴ�. �⵿ ���Դϴ�.";

    string[] dispatch2 = {
            "Unit 42, please respond to call number 1375.",
            "Homicide reported at 1234 Elm Street.",
            "The victim is a male in his 30s, named Alan, cause of death unknown.",
            "Suspects are presumed to be three people invited to the residence.",
            "Investigate the crime scene and find the perpetrator!"
    };

    string officerResponse2 = "Unit 42, acknowledged. En route.";

    // Start is called before the first frame update
    void Start()
    {
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SetTextsPosition();

        StartSequence();
    }

    private void StartSequence()
    {
        StartCoroutine(WaitASecond(5f)); // ȭ���� ���̵�ƿ��ϰ� ��� ���
        StartCoroutine(ShowIntro());    // ��Ʈ�� �޽����� ��� ���

    }

    IEnumerator ShowIntro()
    {
        string initialText;
        initialText = "Dispatcher : ";

        foreach (string message in dispatch)
        {
            yield return new WaitForSeconds(1.5f);
            soundManager.PlayDispatchSound();
            
            yield return StartCoroutine(ShowIntroDispatch(introTexts[textIndex], message, initialText));
            textIndex++;
        }

        initialText = "Police Officer : ";
        yield return new WaitForSeconds(1.5f);
        soundManager.PlayDispatchSound();

        yield return StartCoroutine(ShowIntroDispatch(introTexts[textIndex], officerResponse, initialText));
        textIndex++;
        yield return new WaitForSeconds(1.5f);

        screen.transform.SetSiblingIndex(screen.transform.parent.childCount - 1);

        soundManager.stepCycleManager.SetFootStepSound
            (soundManager.GetComponent<AudioSource>(), soundManager.footStepSounds[0]);
        soundManager.SetNullAudioMixerGroup();

        yield return StartCoroutine(fadeUtility.FadeIn(screen, 3f));
        rainPrefab.gameObject.SetActive(false);
        yield return StartCoroutine(WaitNextScene()); // ���� ������ ��ȯ
    }

    private IEnumerator ShowIntroDispatch(Text t, string dispatch, string initialText = "")
    {
        t.text = initialText; // �ؽ�Ʈ �ʱ�ȭ

        foreach (char letter in dispatch)
        {
            t.text += letter; // �� ���ھ� �߰�
            yield return new WaitForSeconds(0.05f); // 0.03�� ���
        }
    }

    private IEnumerator WaitASecond(float second)
    {
        yield return new WaitForSeconds(second);
        StartCoroutine(fadeUtility.FadeOut(screen, 3f));
    }

    private IEnumerator WaitNextScene()
    {
        // ���� �� �ε����� �������� ���� ���� �񵿱�� �ε�
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(currentSceneIndex + 1);

        // �ε��� �Ϸ�� ������ ���
        while (!asyncLoad.isDone)
        {
            yield return null; // �� ������ ���
        }
    }

    /// <summary>
    /// dispatch, police officer response ���� ���� �������� ��ġ
    /// </summary>
    void SetTextsPosition()
    {
        for(int i = 0; i < 6; i++)
        {
            // RectTransform�� �����ͼ� ��ġ �� ũ�� ����
            RectTransform rectTransform = introTexts[i].GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                // �ؽ�Ʈ�� y ��ǥ�� ���� �������� ���� ��ġ�մϴ�
                rectTransform.anchoredPosition = new Vector2(0, -30 + -i * verticalSpacing);
            }
        }
    }
}
