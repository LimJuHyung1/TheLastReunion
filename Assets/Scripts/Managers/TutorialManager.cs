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

    int index = 0;
    bool isPlayingTutorial = true;
    Image[] tutorialPages;    


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
            int i = 0;
            tutorialPages = new Image[tutorialPage.transform.childCount - 3];            

            while (i < tutorialPage.transform.childCount - 3)
            {
                tutorialPages[i++] = tutorialPage.transform.GetChild(i).GetComponent<Image>();               
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
            // evidenceManager.SendEvidenceInfo();
            StartCoroutine(ExitTutorialSequence());

            uIManager.SetActiveTimer(true);
            uIManager.BeginCountdown();

            CursorManager.Instance.OffVisualization();
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
        cameraManager.ChangeCam();
        index = 0;
        yield return null;
    }
}
