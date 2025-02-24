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
    /// 인트로 영상 끝난 후 게임 진행 설명
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

        tutorialPages[index].gameObject.SetActive(false); // 현재 페이지 비활성화
        index++;
        tutorialPages[index].gameObject.SetActive(true);  // 다음 페이지 활성화
    }

    public void PreviousButton()
    {
        if (index == 1) preBtn.gameObject.SetActive(false);
        else if (index == tutorialPages.Length - 1)
        {
            nextBtn.gameObject.SetActive(true);
            exitTutorialBtn.gameObject.SetActive(false);
        }

        tutorialPages[index].gameObject.SetActive(false); // 현재 페이지 비활성화
        index--;
        tutorialPages[index].gameObject.SetActive(true);  // 이전 페이지 활성화
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
        // 페이드아웃 코루틴이 끝날 때까지 대기
        yield return StartCoroutine(FadeUtility.Instance.FadeOut(uIManager.GetScreen(), 2f));

        // 페이드아웃이 완료된 후에 EndTutorial 실행
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
