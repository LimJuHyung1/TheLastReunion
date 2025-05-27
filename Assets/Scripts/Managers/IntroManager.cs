using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;

public class IntroManager : MonoBehaviour
{
    // ���� ������Ʈ �� ����
    public GameObject rainPrefab;
    public IntroPoliceCar introPoliceCar;

    [Header("UI")]
    public Image screen; // ���̵� ȿ���� ���� ȭ�� �̹���
    public Text[] introTexts; // ��Ʈ�� ��� �ؽ�Ʈ �迭
    public GameObject loading; // �ε� UI ������Ʈ
    private Text loadingText; // �ε� �ۼ�Ʈ ǥ�� �ؽ�Ʈ
    public GameObject quality; // ǰ�� ���� UI
    private Slider qualitySlider; // ǰ�� ���� �����̴�    

    [Header("Managers")]
    public FadeUtility fadeUtility; // ���̵� ȿ�� ����
    public IntroSoundManager introSoundManager; // ��Ʈ�� ���� ����

    private short textIndex = 0; // ���� ����� �ؽ�Ʈ �ε���
    private int currentSceneIndex; // ���� ���� �ε���
    private float[] snapPoints = { 0f, 0.5f, 1f }; // ǰ�� ���� ���� ����Ʈ

    // �ѱ��� ����ó ���
    string[] dispatch = {
        "���� 42, ��� ��ȣ 1375�� ���� �ٶ��ϴ�.",
        "1234 ���� ��Ʈ��Ʈ���� ���� ��� �߻�.",
        "�����ڴ� 30�� ����, �̸��� �ٷ�, ������� �Ҹ�.",
        "�����ڴ� �������� �������� �ʴ���� 3������ ����.",
        "��� ������ �����Ͽ� ������ ã�Ƴ��ʽÿ�!"
    };

    string officerResponse = "���� 42, Ȯ���߽��ϴ�. �⵿ ���Դϴ�.";

    // ���� ����ó ���
    string[] dispatch2 = {
        "Unit 42, please respond to call number 1375.",
        "Homicide reported at 1234 Elm Street.",
        "The victim is a male in his 30s, named Alan, cause of death unknown.",
        "Suspects are presumed to be three people invited to the residence.",
        "Investigate the crime scene and find the perpetrator!"
    };

    string officerResponse2 = "Unit 42, acknowledged. En route.";


    void Start()
    {
        Application.targetFrameRate = 90; // ������ ����

        currentSceneIndex = SceneManager.GetActiveScene().buildIndex; // ���� �� �ε��� ����
        SetTextsPosition(); // �ؽ�Ʈ ��ġ ����

        // �ε� �� ǰ�� ���� UI �ʱ�ȭ
        loadingText = loading.transform.GetChild(loading.transform.childCount - 1).GetComponent<Text>();
        qualitySlider = quality.transform.GetChild(quality.transform.childCount - 1).GetComponent<Slider>();
    }

    /// <summary>
    /// ��Ʈ�� �������� �����ϴ� �ڷ�ƾ
    /// </summary>
    private IEnumerator StartSequence()
    {
        yield return StartCoroutine(WaitASecond(3f)); // 3�ʰ� ���̵�ƿ� �� ���
        yield return StartCoroutine(ShowIntro()); // ��Ʈ�� �޽��� ���
    }

    /// <summary>
    /// ��Ʈ�� �ؽ�Ʈ�� ���������� ����ϴ� �ڷ�ƾ
    /// </summary>
    private IEnumerator ShowIntro()
    {
        string initialText = "Dispatcher : ";

        // ����ó �޽��� ���
        foreach (string message in dispatch)
        {
            yield return new WaitForSeconds(3.5f);
            introSoundManager.PlayDispatchSound();

            yield return StartCoroutine(ShowIntroDispatch(introTexts[textIndex], message, initialText));
            textIndex++;
        }

        // ������ ���� ���
        initialText = "Police Officer : ";
        yield return new WaitForSeconds(3.5f);
        introSoundManager.PlayDispatchSound();
        yield return StartCoroutine(ShowIntroDispatch(introTexts[textIndex], officerResponse, initialText));
        textIndex++;
        yield return new WaitForSeconds(3.5f);

        // ȭ�� ���̵��� �� �ε� UI Ȱ��ȭ
        screen.transform.SetSiblingIndex(screen.transform.parent.childCount - 1);
        yield return StartCoroutine(fadeUtility.FadeIn(screen, 3f));

        loading.transform.SetSiblingIndex(loading.transform.parent.childCount - 1);
        loading.gameObject.SetActive(true);
        rainPrefab.gameObject.SetActive(false);

        // ���� ������ �̵�
        yield return StartCoroutine(WaitNextScene());
    }

    /// <summary>
    /// �� ���ھ� ����ϴ� �ڷ�ƾ (Ÿ���� ȿ��)
    /// </summary>
    private IEnumerator ShowIntroDispatch(Text t, string dispatch, string initialText = "")
    {
        StringBuilder sb = new StringBuilder(initialText);
        t.text = sb.ToString();

        foreach (char letter in dispatch)
        {
            sb.Append(letter);
            t.text = sb.ToString();
            yield return new WaitForSeconds(0.05f);
        }
    }

    /// <summary>
    /// ������ �ð� ���� ��� �� ȭ�� ���̵�ƿ�
    /// </summary>
    private IEnumerator WaitASecond(float second)
    {
        yield return new WaitForSeconds(second);
        StartCoroutine(fadeUtility.FadeOut(screen, 3f));
    }

    /// <summary>
    /// ���� ���� �񵿱� �ε��ϰ� ���൵�� ǥ���ϴ� �ڷ�ƾ
    /// </summary>
    private IEnumerator WaitNextScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(currentSceneIndex + 1);
        asyncLoad.allowSceneActivation = false;

        float previousProgress = 0f; // ���� ����� ����

        while (asyncLoad.progress < 0.9f)
        {
            float currentProgress = Mathf.RoundToInt(asyncLoad.progress * 100);
            if (currentProgress != previousProgress)
            {
                loadingText.text = $"{currentProgress}%";
                previousProgress = currentProgress;
            }
            yield return null;
        }

        loadingText.text = "100%";
        yield return new WaitForSeconds(2f);
        asyncLoad.allowSceneActivation = true;
    }

    /// <summary>
    /// �ؽ�Ʈ ��ġ�� ���� �������� ��ġ
    /// </summary>
    private void SetTextsPosition()
    {
        if (introTexts.Length == 0) return;

        RectTransform canvasRect = introTexts[0].GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        float canvasHeight = canvasRect.rect.height;
        float topPadding = canvasHeight * 0.05f;
        float bottomPadding = canvasHeight * 0.1f;
        float availableHeight = (canvasHeight - (topPadding + bottomPadding)) * 0.9f;
        float spacing = availableHeight / Mathf.Max(1, introTexts.Length - 1);

        for (int i = 0; i < introTexts.Length; i++)
        {
            RectTransform rectTransform = introTexts[i]?.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                float yPosition = -topPadding - (i * spacing);
                rectTransform.anchoredPosition = new Vector2(0, yPosition);
            }
        }
    }

    /// <summary>
    /// ǰ�� ���� �Ϸ� �� ��Ʈ�� ����
    /// </summary>
    public void EndSetQuality()
    {
        quality.gameObject.SetActive(false);
        introPoliceCar.StartPoliceCar();
        introPoliceCar.ReadyToSiren();
        CursorManager.Instance.OffVisualization();
        StartCoroutine(StartSequence());
    }

    /// <summary>
    /// ���� ����
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// �����̴� OnValueChanged �̺�Ʈ�� ����
    /// </summary>
    public void OnSliderValueChanged()
    {
        // ���� ����� ���� ����Ʈ ã��
        float closest = snapPoints[0];

        foreach (float point in snapPoints)
        {
            if (Mathf.Abs(qualitySlider.value - point) < Mathf.Abs(qualitySlider.value - closest))
            {
                closest = point;
            }
        }

        // �����̴��� ���� ����� ������ ���� �̵�
        if (qualitySlider.value != closest) // �ߺ� ���� ����
        {
            qualitySlider.value = closest;
        }

        // �����̴� ���� ���� ǰ�� ���� ����
        if (closest == 0f)
        {
            SetQualityLevel(1); // ���� ȭ��
        }
        else if (closest == 0.5f)
        {
            SetQualityLevel(3); // �߰� ȭ��
        }
        else if (closest == 1f)
        {
            SetQualityLevel(5); // ���� ȭ��
        }
    }

    private void SetQualityLevel(int level)
    {
        QualitySettings.SetQualityLevel(level, true);
        Debug.Log($"ȭ�� ���� �����: {QualitySettings.names[level]} (Level {level})");
    }
}
