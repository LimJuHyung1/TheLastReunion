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

        // 앵커를 중앙 상단으로 설정
        rect.anchorMin = new Vector2(0.5f, 1); // 중앙 상단
        rect.anchorMax = new Vector2(0.5f, 1); // 중앙 상단
    }

    public void SetText(Evidence evidence)
    {
        this.evidenceNameText.text = "▶ " + evidence.GetName();        
    }

    /// <summary>
    /// 해당 오브젝트는 onClick에 두가지 요소가 포함되어 있음
    /// 1. 모든 페이지 비활성화
    /// 2. 해당 오브젝트의 페이지 활성화
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