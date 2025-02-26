using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    private Slider qualitySlider;
    public Image screen;
    public Text[] introTexts;

    public FadeUtility fadeUtility;
    public GameObject loading;
    private Text loadingText;
    public GameObject quality;
    public GameObject rainPrefab;    
    public IntroPoliceCar introPoliceCar;
    public IntroSoundManager introSoundManager;

    short textIndex = 0;
    int currentSceneIndex;
    float verticalSpacing = 150f; // �ؽ�Ʈ ���� ���� ����
    private float[] snapPoints = { 0f, 0.5f, 1f }; // 3�ܰ� ���� ����Ʈ

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

        loadingText = loading.transform.GetChild(loading.transform.childCount - 1).GetComponent<Text>();
        qualitySlider = quality.transform.GetChild(quality.transform.childCount - 1).GetComponent<Slider>();
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
            yield return new WaitForSeconds(3.5f);
            introSoundManager.PlayDispatchSound();
            
            yield return StartCoroutine(ShowIntroDispatch(introTexts[textIndex], message, initialText));
            textIndex++;
        }

        initialText = "Police Officer : ";
        yield return new WaitForSeconds(3.5f);
        introSoundManager.PlayDispatchSound();

        yield return StartCoroutine(ShowIntroDispatch(introTexts[textIndex], officerResponse, initialText));
        textIndex++;
        yield return new WaitForSeconds(3.5f);
        
        screen.transform.SetSiblingIndex(screen.transform.parent.childCount - 1);        

        yield return StartCoroutine(fadeUtility.FadeIn(screen, 3f));
        loading.transform.SetSiblingIndex(loading.transform.parent.childCount - 1);
        loading.gameObject.SetActive(true);
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
        // ���� �� �ε����� �������� ���� ���� �񵿱�� �ε� (�� ��ȯ�� �ڵ����� ���� �ʵ��� ����)
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(currentSceneIndex + 1);
        asyncLoad.allowSceneActivation = false; // 100% �ε�� ������ �� �ڵ� ��ȯ ����

        // �ε��� 90%���� �Ϸ�� ������ ���
        while (asyncLoad.progress < 0.9f)
        {
            loadingText.text = $"{Mathf.RoundToInt(asyncLoad.progress * 100)}%";

            yield return null; // �� ������ ���
        }

        // 100% �ε��� ���� �߰� ���
        loadingText.text = "100%";

        yield return new WaitForSeconds(2f); // 2�� ��� �� �� ��ȯ

        asyncLoad.allowSceneActivation = true; // �� ��ȯ ����
    }

    /// <summary>
    /// dispatch, police officer response ���� ���� �������� ��ġ
    /// </summary>
    void SetTextsPosition()
    {
        RectTransform canvasRect = introTexts[0].GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        float canvasHeight = canvasRect.rect.height;
        float topPadding = canvasHeight * 0.05f;  // ��� ���� (5%)
        float bottomPadding = canvasHeight * 0.1f; // �ϴ� ���� (10%)

        float availableHeight = canvasHeight - (topPadding + bottomPadding);
        availableHeight *= 0.9f; // ��ü ���̸� 90%�� �ٿ��� ������ ����

        float spacing = availableHeight / (introTexts.Length - 1);

        for (int i = 0; i < introTexts.Length; i++)
        {
            RectTransform rectTransform = introTexts[i].GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                float yPosition = -topPadding - (i * spacing);
                rectTransform.anchoredPosition = new Vector2(0, yPosition);
            }
        }
    }


    void StartSetQuality()
    {
        quality.gameObject.SetActive(true);        
    }

    public void EndSetQuality()
    {
        quality.gameObject.SetActive(false);
        introPoliceCar.StartPoliceCar();
        introPoliceCar.ReadyToSiren();

        CursorManager.Instance.OffVisualization();
        StartSequence();
    }

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

    void SetQualityLevel(int level)
    {
        QualitySettings.SetQualityLevel(level, true);
        Debug.Log($"ȭ�� ���� �����: {QualitySettings.names[level]} (Level {level})");
    }
}
