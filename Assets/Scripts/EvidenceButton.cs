using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvidenceButton : MonoBehaviour
{
    public Text evidenceNameText;
    Image evidenceIntroductionPage;
    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;
        GetComponent<Button>().onClick.AddListener(PlaySound);
    }

    public void SetAnchor()
    {
        RectTransform rect = GetComponent<RectTransform>();

        // ��Ŀ�� �߾� ������� ����
        rect.anchorMin = new Vector2(0.5f, 1); // �߾� ���
        rect.anchorMax = new Vector2(0.5f, 1); // �߾� ���
    }

    public void SetText(Evidence evidence)
    {
        this.evidenceNameText.text = "�� " + evidence.GetName();        
    }

    /// <summary>
    /// �ش� ������Ʈ�� onClick�� �ΰ��� ��Ұ� ���ԵǾ� ����
    /// 1. ��� ������ ��Ȱ��ȭ
    /// 2. �ش� ������Ʈ�� ������ Ȱ��ȭ
    /// </summary>
    /// <param name="introduction"></param>
    public void SetEvidenceIntroduction(Image introduction)
    {
        evidenceIntroductionPage = introduction;
        GetComponent<Button>().onClick.AddListener(ShowEvidenIntroductionPage);
    }

    public void ShowEvidenIntroductionPage()
    {
        evidenceIntroductionPage.gameObject.SetActive(true);
    }

    public Image GetEvidenceIntroductionPage()
    {
        return this.evidenceIntroductionPage;
    }





    void PlaySound()
    {
        audioSource.Play();
    }
}