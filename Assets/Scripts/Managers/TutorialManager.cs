using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("UI")]
    public Button preBtn;  // 이전 페이지 버튼
    public Button nextBtn; // 다음 페이지 버튼
    public Button exitTutorialBtn; // 튜토리얼 종료 버튼
    public Image tutorialPage; // 튜토리얼 페이지 컨테이너

    [Header("Managers")]
    public CameraManager cameraManager;
    public EvidenceManager evidenceManager;
    public UIManager uIManager;

    [Header("Objects")]
    public Player player;
    public Timer timer;

    private int index = 0; // 현재 튜토리얼 페이지 인덱스
    private bool isPlayingTutorial = true; // 튜토리얼 진행 여부
    private Image[] tutorialPages; // 튜토리얼 페이지 배열


    // Start is called before the first frame update
    void Start()
    {
        SetPages();                 // 튜토리얼 페이지 배열 설정

        // 화면 페이드인 효과 적용
        StartCoroutine(FadeUtility.Instance.FadeIn(uIManager.screen, 2f));

        SetActiveFalseAllPages();   // 모든 페이지 숨기기
        SetTutorialUI(false);       // 튜토리얼 UI 비활성화

        ShowPlayRole();             // 첫 번째 튜토리얼 페이지 표시
    }

    /// <summary>
    /// 튜토리얼 페이지 배열을 설정하는 메서드
    /// - `tutorialPage` 내의 자식 객체들을 가져와 `tutorialPages` 배열에 저장
    /// </summary>

    private void SetPages()
    {
        if (tutorialPage == null) return;

        int pageCount = tutorialPage.transform.childCount - 3;  // 마지막 3개는 제외
        tutorialPages = new Image[pageCount];

        for (int i = 0; i < pageCount; i++)
        {
            tutorialPages[i] = tutorialPage.transform.GetChild(i).GetComponent<Image>();
        }
    }

    /// <summary>
    /// 인트로 영상이 끝난 후 게임 진행 설명을 시작
    /// - 첫 번째 튜토리얼 페이지를 표시하고 플레이어를 대화 상태로 설정
    /// </summary>
    private void ShowPlayRole()
    {
        SetTutorialUI(true);
        preBtn.gameObject.SetActive(false);                 // 첫 페이지이므로 이전 버튼 비활성화
        exitTutorialBtn.gameObject.SetActive(false);        // 종료 버튼 비활성화
        tutorialPages[index].gameObject.SetActive(true);    // 첫 페이지 활성화
        player.ActivateIsTalking();                         // 플레이어를 대화 모드로 설정
    }

    /// <summary>
    /// 튜토리얼 UI 요소들을 활성화/비활성화
    /// - 현재 페이지 인덱스에 따라 버튼 상태를 변경
    /// </summary>
    void SetTutorialUI(bool isPlaying)
    {
        tutorialPage.gameObject.SetActive(isPlaying);
        preBtn.gameObject.SetActive(isPlaying && index > 0);
        nextBtn.gameObject.SetActive(isPlaying && index < tutorialPages.Length - 1);
        exitTutorialBtn.gameObject.SetActive(isPlaying && index == tutorialPages.Length - 1);
    }

    /// <summary>
    /// 모든 튜토리얼 페이지를 비활성화
    /// </summary>
    void SetActiveFalseAllPages()
    {
        for (short i = 0; i < tutorialPages.Length; i++)
        {
            tutorialPages[i].gameObject.SetActive(false);
        }
    }

    //-------------------------------------------------------------//

    /// <summary>
    /// 튜토리얼 페이지를 표시
    /// </summary>
    public void ShowTutorialPage()
    {
        SetTutorialUI(true);
        exitTutorialBtn.gameObject.SetActive(true);
        SetActiveFalseAllPages();
        tutorialPages[index].gameObject.SetActive(true);
    }

    /// <summary>
    /// 튜토리얼 페이지를 숨김
    /// </summary>
    public void HideTutorialPage()
    {
        SetTutorialUI(false);
    }


    /// <summary>
    /// 페이지 변경을 처리하는 공통 메서드
    /// - `direction`이 1이면 다음 페이지, -1이면 이전 페이지로 이동
    /// </summary>
    private void ChangePage(int direction)
    {
        tutorialPages[index].gameObject.SetActive(false);
        index += direction;
        tutorialPages[index].gameObject.SetActive(true);

        SetTutorialUI(true);
    }

    /// <summary>
    /// 다음 튜토리얼 페이지로 이동
    /// </summary>
    public void NextButton()
    {
        if (index < tutorialPages.Length - 1)
            ChangePage(1);
    }

    /// <summary>
    /// 이전 튜토리얼 페이지로 이동
    /// </summary>
    public void PreviousButton()
    {
        if (index > 0)
            ChangePage(-1);
    }

    /// <summary>
    /// 튜토리얼 종료 버튼 클릭 시 실행
    /// - 튜토리얼이 진행 중이면 종료 시퀀스를 실행
    /// - 이미 종료된 상태라면 튜토리얼 UI를 숨김
    /// </summary>
    public void ExitTutorialButton()
    {
        if (!isPlayingTutorial)
        {
            HideTutorialPage();
            return;
        }

        tutorialPage.gameObject.SetActive(false);
        SetTutorialUI(false);
        StartCoroutine(ExitTutorialSequence());

        uIManager.SetActiveTimer(true);
        uIManager.BeginCountdown();

        CursorManager.Instance.OffVisualization();
    }

    /// <summary>
    /// 튜토리얼 종료 시퀀스를 실행하는 코루틴
    /// - 페이드아웃 후 카메라 변경 및 플레이어 상태 변경
    /// </summary>
    IEnumerator ExitTutorialSequence()
    {
        yield return FadeUtility.Instance.FadeOut(uIManager.GetScreen(), 2f);

        player.UnactivateIsTalking();
        cameraManager.ChangeCam();
        index = 0;
    }
}
