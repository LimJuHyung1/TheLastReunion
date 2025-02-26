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
    float verticalSpacing = 150f; // 텍스트 간의 수직 간격
    private float[] snapPoints = { 0f, 0.5f, 1f }; // 3단계 스냅 포인트

    string[] dispatch = {
            "차량 42, 사건 번호 1375에 응답 바랍니다.",
            "1234 엘름 스트리트에서 살인 사건 발생.",
            "피해자는 30대 남성, 이름은 앨런, 사망원인 불명.",
            "용의자는 피해자의 거주지에 초대받은 3명으로 추정.",
            "사건 현장을 수사하여 범인을 찾아내십시오!",
        };

    string officerResponse = "차량 42, 확인했습니다. 출동 중입니다.";

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
        StartCoroutine(WaitASecond(5f)); // 화면을 페이드아웃하고 잠시 대기
        StartCoroutine(ShowIntro());    // 인트로 메시지를 모두 출력
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
        yield return StartCoroutine(WaitNextScene()); // 다음 씬으로 전환
    }

    private IEnumerator ShowIntroDispatch(Text t, string dispatch, string initialText = "")
    {
        t.text = initialText; // 텍스트 초기화

        foreach (char letter in dispatch)
        {
            t.text += letter; // 한 글자씩 추가
            yield return new WaitForSeconds(0.05f); // 0.03초 대기
        }
    }

    private IEnumerator WaitASecond(float second)
    {
        yield return new WaitForSeconds(second);
        StartCoroutine(fadeUtility.FadeOut(screen, 3f));
    }

    private IEnumerator WaitNextScene()
    {
        // 현재 씬 인덱스를 기준으로 다음 씬을 비동기로 로드 (씬 전환을 자동으로 하지 않도록 설정)
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(currentSceneIndex + 1);
        asyncLoad.allowSceneActivation = false; // 100% 로드될 때까지 씬 자동 전환 방지

        // 로딩이 90%까지 완료될 때까지 대기
        while (asyncLoad.progress < 0.9f)
        {
            loadingText.text = $"{Mathf.RoundToInt(asyncLoad.progress * 100)}%";

            yield return null; // 매 프레임 대기
        }

        // 100% 로딩을 위한 추가 대기
        loadingText.text = "100%";

        yield return new WaitForSeconds(2f); // 2초 대기 후 씬 전환

        asyncLoad.allowSceneActivation = true; // 씬 전환 실행
    }

    /// <summary>
    /// dispatch, police officer response 문자 일정 간격으로 배치
    /// </summary>
    void SetTextsPosition()
    {
        RectTransform canvasRect = introTexts[0].GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        float canvasHeight = canvasRect.rect.height;
        float topPadding = canvasHeight * 0.05f;  // 상단 여백 (5%)
        float bottomPadding = canvasHeight * 0.1f; // 하단 여백 (10%)

        float availableHeight = canvasHeight - (topPadding + bottomPadding);
        availableHeight *= 0.9f; // 전체 높이를 90%로 줄여서 간격을 좁게

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
    /// 슬라이더 OnValueChanged 이벤트로 실행
    /// </summary>
    public void OnSliderValueChanged()
    {
        // 가장 가까운 스냅 포인트 찾기
        float closest = snapPoints[0];

        foreach (float point in snapPoints)
        {
            if (Mathf.Abs(qualitySlider.value - point) < Mathf.Abs(qualitySlider.value - closest))
            {
                closest = point;
            }
        }

        // 슬라이더를 가장 가까운 값으로 스냅 이동
        if (qualitySlider.value != closest) // 중복 실행 방지
        {
            qualitySlider.value = closest;
        }

        // 슬라이더 값에 따라 품질 설정 변경
        if (closest == 0f)
        {
            SetQualityLevel(1); // 낮은 화질
        }
        else if (closest == 0.5f)
        {
            SetQualityLevel(3); // 중간 화질
        }
        else if (closest == 1f)
        {
            SetQualityLevel(5); // 높은 화질
        }
    }

    void SetQualityLevel(int level)
    {
        QualitySettings.SetQualityLevel(level, true);
        Debug.Log($"화질 설정 변경됨: {QualitySettings.names[level]} (Level {level})");
    }
}
