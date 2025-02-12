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

    private float[] snapPoints = { 0f, 0.5f, 1f }; // 3�ܰ� ���� ����Ʈ

    void Awake()
    {
        // Application.targetFrameRate = 60;
        // QualitySettings.vSyncCount = 0;

        audioSource = GetComponent<AudioSource>();

        escPage.gameObject.SetActive(false);
        evidencePage.gameObject.SetActive(false);

        if (mouseSlider != null)
        {
            mouseSlider.value = 115f; // �����̴� �ʱⰪ ����
            mouseSlider.onValueChanged.AddListener(OnSensitivityChanged); // �����̴� �� ���� �̺�Ʈ ����
        }

        if (qualitySlider != null) 
        {
            InitQualityLevel();
        }
    }

    void Update()
    {        
        // ������ Ŀ�� ���� ����
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

            // Esc �޴��� ���������� Ŀ�� ����� ���
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

    // ESCPage - OpenLogButton �� ����
    public void ShowLog()
    {
        escPage.gameObject.SetActive(false);

        logManager.OpenLogPage();
    }

    // Log - ExitStatementButton �� ����
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
        asyncLoad.allowSceneActivation = false; // �� �ڵ� ��ȯ ����

        // �ε��� 90%���� �Ϸ�� ������ ���൵ ǥ��
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
    /// �����̴� OnValueChanged �̺�Ʈ�� ����
    /// </summary>
    public void OnQualitySliderValueChanged()
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
