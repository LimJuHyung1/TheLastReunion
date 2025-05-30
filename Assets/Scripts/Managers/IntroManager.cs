using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;

public class IntroManager : MonoBehaviour
{
    // 게임 오브젝트 및 참조
    public GameObject rainPrefab;
    public IntroPoliceCar introPoliceCar;

    [Header("UI")]
    public Image screen; // 페이드 효과를 위한 화면 이미지
    public Text[] introTexts; // 인트로 대사 텍스트 배열
    public GameObject loading; // 로딩 UI 오브젝트
    private Text loadingText; // 로딩 퍼센트 표시 텍스트
    public GameObject quality; // 품질 설정 UI
    private Slider qualitySlider; // 품질 설정 슬라이더    

    [Header("Managers")]
    public FadeUtility fadeUtility; // 페이드 효과 관리
    public IntroSoundManager introSoundManager; // 인트로 사운드 관리

    private short textIndex = 0; // 현재 출력할 텍스트 인덱스
    private int currentSceneIndex; // 현재 씬의 인덱스
    private float[] snapPoints = { 0f, 0.5f, 1f }; // 품질 조절 스냅 포인트

    // 한국어 디스패처 대사
    string[] dispatch = {
        "차량 42, 사건 번호 1375에 응답 바랍니다.",
        "1234 엘름 스트리트에서 살인 사건 발생.",
        "피해자는 30대 남성, 이름은 앨런, 사망원인 불명.",
        "용의자는 피해자의 거주지에 초대받은 3명으로 추정.",
        "사건 현장을 수사하여 범인을 찾아내십시오!"
    };

    string officerResponse = "차량 42, 확인했습니다. 출동 중입니다.";

    // 영어 디스패처 대사
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
        Application.targetFrameRate = 90; // 프레임 제한

        currentSceneIndex = SceneManager.GetActiveScene().buildIndex; // 현재 씬 인덱스 저장
        SetTextsPosition(); // 텍스트 위치 설정

        // 로딩 및 품질 설정 UI 초기화
        loadingText = loading.transform.GetChild(loading.transform.childCount - 1).GetComponent<Text>();
        qualitySlider = quality.transform.GetChild(quality.transform.childCount - 1).GetComponent<Slider>();
    }

    /// <summary>
    /// 인트로 시퀀스를 시작하는 코루틴
    /// </summary>
    private IEnumerator StartSequence()
    {
        yield return StartCoroutine(WaitASecond(3f)); // 3초간 페이드아웃 후 대기
        yield return StartCoroutine(ShowIntro()); // 인트로 메시지 출력
    }

    /// <summary>
    /// 인트로 텍스트를 순차적으로 출력하는 코루틴
    /// </summary>
    private IEnumerator ShowIntro()
    {
        string initialText = "Dispatcher : ";

        // 디스패처 메시지 출력
        foreach (string message in dispatch)
        {
            yield return new WaitForSeconds(3.5f);
            introSoundManager.PlayDispatchSound();

            yield return StartCoroutine(ShowIntroDispatch(introTexts[textIndex], message, initialText));
            textIndex++;
        }

        // 마지막 응답 출력
        initialText = "Police Officer : ";
        yield return new WaitForSeconds(3.5f);
        introSoundManager.PlayDispatchSound();
        yield return StartCoroutine(ShowIntroDispatch(introTexts[textIndex], officerResponse, initialText));
        textIndex++;
        yield return new WaitForSeconds(3.5f);

        // 화면 페이드인 및 로딩 UI 활성화
        screen.transform.SetSiblingIndex(screen.transform.parent.childCount - 1);
        yield return StartCoroutine(fadeUtility.FadeIn(screen, 3f));

        loading.transform.SetSiblingIndex(loading.transform.parent.childCount - 1);
        loading.gameObject.SetActive(true);
        rainPrefab.gameObject.SetActive(false);

        // 다음 씬으로 이동
        yield return StartCoroutine(WaitNextScene());
    }

    /// <summary>
    /// 한 글자씩 출력하는 코루틴 (타이핑 효과)
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
    /// 지정된 시간 동안 대기 후 화면 페이드아웃
    /// </summary>
    private IEnumerator WaitASecond(float second)
    {
        yield return new WaitForSeconds(second);
        StartCoroutine(fadeUtility.FadeOut(screen, 3f));
    }

    /// <summary>
    /// 다음 씬을 비동기 로드하고 진행도를 표시하는 코루틴
    /// </summary>
    private IEnumerator WaitNextScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(currentSceneIndex + 1);
        asyncLoad.allowSceneActivation = false;

        float previousProgress = 0f; // 이전 진행률 저장

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
    /// 텍스트 위치를 일정 간격으로 배치
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
    /// 품질 설정 완료 후 인트로 시작
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
    /// 게임 종료
    /// </summary>
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

    private void SetQualityLevel(int level)
    {
        QualitySettings.SetQualityLevel(level, true);
        Debug.Log($"화질 설정 변경됨: {QualitySettings.names[level]} (Level {level})");
    }
}
