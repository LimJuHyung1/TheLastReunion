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
    public Slider mouseSlider;

    public CameraScript cam;
    public LogManager logManager;
    public TutorialManager tutorialManager;
    public Player player;
    public UIManager uIManager;

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
    }

    void Update()
    {        
        // ������ Ŀ�� ���� ����
        if (player.GetIsTalking())
        {
            // Esc �޴��� ���������� Ŀ���� ���̰� ����
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;       
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
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
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
        player.UnactivateIsTalking();
        evidencePage.gameObject.SetActive(false);
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
        StartCoroutine(SelectCriminalCoroutine());
    }

    public IEnumerator SelectCriminalCoroutine()
    {
        yield return StartCoroutine(FadeUtility.Instance.FadeIn(uIManager.GetScreen(), 2f));
        yield return StartCoroutine(LoadLastScene());
    }

    IEnumerator LoadLastScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
        
        yield return null;
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
