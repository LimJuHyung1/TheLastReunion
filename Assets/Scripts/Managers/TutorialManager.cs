using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public Button preBtn;
    public Button nextBtn;
    public Button exitTutorialBtn;
    public Image tutorialPage;

    public CameraManager cameraManager;
    public EvidenceManager evidenceManager;

    public Player player;
    public Timer timer;
    public UIManager uIManager;
    
    bool isPlayingTutorial = true;
    [SerializeField] Image[] tutorialPages;
    [SerializeField] int index = 0;


    // Start is called before the first frame update
    void Start()
    {
        SetPages();
        StartCoroutine(FadeUtility.Instance.FadeIn(uIManager.screen, 2f));

        SetActiveFalseAllPages();
        SetTutorialUI(false);

        ShowPlayRole();
    }

    private void SetPages()
    {
        if (tutorialPage != null)
        {
            int index = 0;
            foreach (Transform page in tutorialPage.transform)
            {
                if(page.GetComponent<Button>() == null)
                {
                    tutorialPages[index] = page.GetComponent<Image>();
                    index++;
                }                    
            }
        }
    }

    /// <summary>
    /// ��Ʈ�� ���� ���� �� ���� ���� ����
    /// </summary>
    private void ShowPlayRole()
    {
        SetTutorialUI(true);
        preBtn.gameObject.SetActive(false);
        exitTutorialBtn.gameObject.SetActive(false);
        tutorialPages[index].gameObject.SetActive(true);
        // CursorManager.Instance.OnVisualization();
        player.ActivateIsTalking();
    }

    void SetTutorialUI(bool isPlayingTutorial)
    {
        tutorialPage.gameObject.SetActive(isPlayingTutorial);
        preBtn.gameObject.SetActive(isPlayingTutorial);
        nextBtn.gameObject.SetActive(isPlayingTutorial);

        if(!isPlayingTutorial)
            exitTutorialBtn.gameObject.SetActive(isPlayingTutorial);

        if(index == 0)
            preBtn.gameObject.SetActive(false);
        else if(index == tutorialPages.Length - 1)
            nextBtn.gameObject.SetActive(false);
    }

    void SetActiveFalseAllPages()
    {
        for (short i = 0; i < tutorialPages.Length; i++)
        {
            tutorialPages[i].gameObject.SetActive(false);
        }
    }

    //-------------------------------------------------------------//
    
    public void ShowTutorialPage()
    {
        SetTutorialUI(true);
        exitTutorialBtn.gameObject.SetActive(true);
        SetActiveFalseAllPages();
        tutorialPages[index].gameObject.SetActive(true);
    }

    public void HideTutorialPage()
    {
        SetTutorialUI(false);
    }


    public void NextButton()
    {
        if (index == 0)
            preBtn.gameObject.SetActive(true);
        else if (index == tutorialPages.Length - 2)
        {
            nextBtn.gameObject.SetActive(false);
            exitTutorialBtn.gameObject.SetActive(true);
        }            

        tutorialPages[index].gameObject.SetActive(false); // ���� ������ ��Ȱ��ȭ
        index++;
        tutorialPages[index].gameObject.SetActive(true);  // ���� ������ Ȱ��ȭ
    }

    public void PreviousButton()
    {
        if (index == 1) preBtn.gameObject.SetActive(false);
        else if (index == tutorialPages.Length - 1)
        {
            nextBtn.gameObject.SetActive(true);
            exitTutorialBtn.gameObject.SetActive(false);
        }

        tutorialPages[index].gameObject.SetActive(false); // ���� ������ ��Ȱ��ȭ
        index--;
        tutorialPages[index].gameObject.SetActive(true);  // ���� ������ Ȱ��ȭ
    }

    public void ExitTutorialButton()
    {
        if (isPlayingTutorial)
        {
            tutorialPage.gameObject.SetActive(false);
            SetTutorialUI(false);
            evidenceManager.SendEvidenceInfo();
            StartCoroutine(ExitTutorialSequence());
            timer.gameObject.SetActive(true);
            timer.StartTimer();
        }
        else
        {
            HideTutorialPage();
        }
    }

    IEnumerator ExitTutorialSequence()
    {
        // ���̵�ƿ� �ڷ�ƾ�� ���� ������ ���
        yield return StartCoroutine(FadeUtility.Instance.FadeOut(uIManager.GetScreen(), 2f));

        // ���̵�ƿ��� �Ϸ�� �Ŀ� EndTutorial ����
        yield return StartCoroutine(EndTutorial());
    }

    IEnumerator EndTutorial()
    {
        player.UnactivateIsTalking();
        uIManager.SetActiveCursor(true);
        // CursorManager.Instance.OffVisualization();
        cameraManager.ChangeCam();
        index = 0;
        yield return null;
    }
}
