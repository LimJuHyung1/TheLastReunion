using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("UI")]
    public Button preBtn;  // ���� ������ ��ư
    public Button nextBtn; // ���� ������ ��ư
    public Button exitTutorialBtn; // Ʃ�丮�� ���� ��ư
    public Image tutorialPage; // Ʃ�丮�� ������ �����̳�

    [Header("Managers")]
    public CameraManager cameraManager;
    public EvidenceManager evidenceManager;
    public UIManager uIManager;

    [Header("Objects")]
    public Player player;
    public Timer timer;

    private int index = 0; // ���� Ʃ�丮�� ������ �ε���
    private bool isPlayingTutorial = true; // Ʃ�丮�� ���� ����
    private Image[] tutorialPages; // Ʃ�丮�� ������ �迭


    // Start is called before the first frame update
    void Start()
    {
        SetPages();                 // Ʃ�丮�� ������ �迭 ����

        // ȭ�� ���̵��� ȿ�� ����
        StartCoroutine(FadeUtility.Instance.FadeIn(uIManager.screen, 2f));

        SetActiveFalseAllPages();   // ��� ������ �����
        SetTutorialUI(false);       // Ʃ�丮�� UI ��Ȱ��ȭ

        ShowPlayRole();             // ù ��° Ʃ�丮�� ������ ǥ��
    }

    /// <summary>
    /// Ʃ�丮�� ������ �迭�� �����ϴ� �޼���
    /// - `tutorialPage` ���� �ڽ� ��ü���� ������ `tutorialPages` �迭�� ����
    /// </summary>

    private void SetPages()
    {
        if (tutorialPage == null) return;

        int pageCount = tutorialPage.transform.childCount - 3;  // ������ 3���� ����
        tutorialPages = new Image[pageCount];

        for (int i = 0; i < pageCount; i++)
        {
            tutorialPages[i] = tutorialPage.transform.GetChild(i).GetComponent<Image>();
        }
    }

    /// <summary>
    /// ��Ʈ�� ������ ���� �� ���� ���� ������ ����
    /// - ù ��° Ʃ�丮�� �������� ǥ���ϰ� �÷��̾ ��ȭ ���·� ����
    /// </summary>
    private void ShowPlayRole()
    {
        SetTutorialUI(true);
        preBtn.gameObject.SetActive(false);                 // ù �������̹Ƿ� ���� ��ư ��Ȱ��ȭ
        exitTutorialBtn.gameObject.SetActive(false);        // ���� ��ư ��Ȱ��ȭ
        tutorialPages[index].gameObject.SetActive(true);    // ù ������ Ȱ��ȭ
        player.ActivateIsTalking();                         // �÷��̾ ��ȭ ���� ����
    }

    /// <summary>
    /// Ʃ�丮�� UI ��ҵ��� Ȱ��ȭ/��Ȱ��ȭ
    /// - ���� ������ �ε����� ���� ��ư ���¸� ����
    /// </summary>
    void SetTutorialUI(bool isPlaying)
    {
        tutorialPage.gameObject.SetActive(isPlaying);
        preBtn.gameObject.SetActive(isPlaying && index > 0);
        nextBtn.gameObject.SetActive(isPlaying && index < tutorialPages.Length - 1);
        exitTutorialBtn.gameObject.SetActive(isPlaying && index == tutorialPages.Length - 1);
    }

    /// <summary>
    /// ��� Ʃ�丮�� �������� ��Ȱ��ȭ
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
    /// Ʃ�丮�� �������� ǥ��
    /// </summary>
    public void ShowTutorialPage()
    {
        SetTutorialUI(true);
        exitTutorialBtn.gameObject.SetActive(true);
        SetActiveFalseAllPages();
        tutorialPages[index].gameObject.SetActive(true);
    }

    /// <summary>
    /// Ʃ�丮�� �������� ����
    /// </summary>
    public void HideTutorialPage()
    {
        SetTutorialUI(false);
    }


    /// <summary>
    /// ������ ������ ó���ϴ� ���� �޼���
    /// - `direction`�� 1�̸� ���� ������, -1�̸� ���� �������� �̵�
    /// </summary>
    private void ChangePage(int direction)
    {
        tutorialPages[index].gameObject.SetActive(false);
        index += direction;
        tutorialPages[index].gameObject.SetActive(true);

        SetTutorialUI(true);
    }

    /// <summary>
    /// ���� Ʃ�丮�� �������� �̵�
    /// </summary>
    public void NextButton()
    {
        if (index < tutorialPages.Length - 1)
            ChangePage(1);
    }

    /// <summary>
    /// ���� Ʃ�丮�� �������� �̵�
    /// </summary>
    public void PreviousButton()
    {
        if (index > 0)
            ChangePage(-1);
    }

    /// <summary>
    /// Ʃ�丮�� ���� ��ư Ŭ�� �� ����
    /// - Ʃ�丮���� ���� ���̸� ���� �������� ����
    /// - �̹� ����� ���¶�� Ʃ�丮�� UI�� ����
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
    /// Ʃ�丮�� ���� �������� �����ϴ� �ڷ�ƾ
    /// - ���̵�ƿ� �� ī�޶� ���� �� �÷��̾� ���� ����
    /// </summary>
    IEnumerator ExitTutorialSequence()
    {
        yield return FadeUtility.Instance.FadeOut(uIManager.GetScreen(), 2f);

        player.UnactivateIsTalking();
        cameraManager.ChangeCam();
        index = 0;
    }
}
