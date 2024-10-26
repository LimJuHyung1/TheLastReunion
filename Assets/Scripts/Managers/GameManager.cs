using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private AudioSource audioSource;

    public Image escPage;
    public Image evidencePage;

    public LogManager logManager;
    public TutorialManager tutorialManager;
    public Player player;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        escPage.gameObject.SetActive(false);
        evidencePage.gameObject.SetActive(false);
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

    public void ExitGame()
    {
        Application.Quit();
    }
}
