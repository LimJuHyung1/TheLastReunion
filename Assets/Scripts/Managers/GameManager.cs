using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private AudioSource audioSource;

    public Image escPage;
    public Image evidencePage;
    public Image settingPage;
    public Image selectCriminalPage;
    public Slider mouseSlider;
    public Slider qualitySlider;

    public GameObject loading;
    private Text loadingText;

    public CameraScript cam;
    public LogManager logManager;
    public TutorialManager tutorialManager;
    public Player player;
    public UIManager uIManager;

    private float[] snapPoints = { 0f, 0.5f, 1f }; // 3단계 스냅 포인트

    void Awake()
    {
        // Application.targetFrameRate = 60;
        // QualitySettings.vSyncCount = 0;

        audioSource = GetComponent<AudioSource>();

        escPage.gameObject.SetActive(false);
        evidencePage.gameObject.SetActive(false);

        if (mouseSlider != null)
        {
            mouseSlider.value = 115f; // 슬라이더 초기값 설정
            mouseSlider.onValueChanged.AddListener(OnSensitivityChanged); // 슬라이더 값 변경 이벤트 연결
        }

        if (qualitySlider != null) 
        {
            InitQualityLevel();
        }
    }

    void Update()
    {        
        // 강제로 커서 상태 유지
        if (player.GetIsTalking())
        {
            CursorManager.Instance.OnVisualization();            
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!player.GetIsTalking())
                {
                    audioSource.Play();
                    OpenEscPage();
                }
                else
                {
                    CloseEscPage();
                }
            }

            // Esc 메뉴가 닫혀있으면 커서 숨기고 잠금
            CursorManager.Instance.OffVisualization();            
        }  
    }

    void OpenEscPage()
    {
        player.ActivateIsTalking();
        escPage.gameObject.SetActive(true);
    }

    public void CloseEscPage()
    {
        player.UnactivateIsTalking();
        escPage.gameObject.SetActive(false);
    }

    public void OpenEvidencePage()
    {
        escPage.gameObject.SetActive(false);
        evidencePage.gameObject.SetActive(true);
    }

    public void CloseEvidencePage()
    {        
        evidencePage.gameObject.SetActive(false);
        escPage.gameObject.SetActive(true);
    }



    public void ShowTutorialPage()
    {
        escPage.gameObject.SetActive(false);

        tutorialManager.ShowTutorialPage();        
    }

    // ESCPage - OpenLogButton 에 쓰임
    public void ShowLog()
    {
        escPage.gameObject.SetActive(false);

        logManager.OpenLogPage();
    }

    // Log - ExitStatementButton 에 쓰임
    public void CloseLog()
    {
        escPage.gameObject.SetActive(true);

        logManager.CloseLogPage();
    }




    public void SelectCriminal()
    {
        selectCriminalPage.gameObject.SetActive(true);
    }

    public void SelectCriminal_O()
    {
        selectCriminalPage.gameObject.SetActive(false);
        escPage.gameObject.SetActive(false);
        StartCoroutine(SelectCriminalCoroutine());
    }

    public void SelectCriminal_X()
    {
        selectCriminalPage.gameObject.SetActive(false);
    }

    public IEnumerator SelectCriminalCoroutine()
    {
        yield return StartCoroutine(FadeUtility.Instance.FadeIn(uIManager.GetScreen(), 2f));
        loading.gameObject.SetActive(true);
        loadingText = loading.transform.GetChild(loading.transform.childCount - 1).GetComponent<Text>();
        yield return StartCoroutine(LoadLastScene());
    }

    IEnumerator LoadLastScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(currentSceneIndex + 1);
        asyncLoad.allowSceneActivation = false; // 씬 자동 전환 방지

        // 로딩이 90%까지 완료될 때까지 진행도 표시
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




    public void OpenSettingPage()
    {
        escPage.gameObject.SetActive(false);
        settingPage.gameObject.SetActive(true);
    }

    public void CloseSettingPage()
    {        
        settingPage.gameObject.SetActive(false);
        escPage.gameObject.SetActive(true);
    }

    /// <summary>
    /// 슬라이더 OnValueChanged 이벤트로 실행
    /// </summary>
    public void OnQualitySliderValueChanged()
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

    void InitQualityLevel()
    {
        int currentQualityLevel = QualitySettings.GetQualityLevel();

        if(currentQualityLevel == 1)
            qualitySlider.value = 0f;
        else if(currentQualityLevel == 3)
            qualitySlider.value = 0.5f;
        else if (currentQualityLevel == 5)
            qualitySlider.value = 1f;
    }


    public void ExitGame()
    {
        Application.Quit();
    }



    public void OnSensitivityChanged(float value)
    {
        cam.SetMouseSensitivity(value);        
    }
}
