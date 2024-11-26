using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    public Image screen;
    public Text[] introTexts;

    public FadeUtility fadeUtility;
    public GameObject rainPrefab;
    public IntroSoundManager introSoundManager;

    short textIndex = 0;
    int currentSceneIndex;
    float verticalSpacing = 150f; // 텍스트 간의 수직 간격

    string[] dispatch = {
            "차량 42, 사건 번호 1375에 응답 바랍니다.",
            "1234 엘름 스트리트에서 살인 사건 발생.",
            "피해자는 30대 남성, 이름은 앨런, 사망원인 불명.",
            "용의자는 피해자의 거주지에 초대받은 3명으로 추정.",
            "사건 현장을 수사하여 범인을 찾아내십시오!",
        };

    string officerResponse = "차량 42, 확인했습니다. 출동 중입니다.";

    string[] dispatch2 = {
            "Unit 42, please respond to call number 1375.",
            "Homicide reported at 1234 Elm Street.",
            "The victim is a male in his 30s, named Alan, cause of death unknown.",
            "Suspects are presumed to be three people invited to the residence.",
            "Investigate the crime scene and find the perpetrator!"
    };

    string officerResponse2 = "Unit 42, acknowledged. En route.";

    // Start is called before the first frame update
    void Start()
    {
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SetTextsPosition();

        StartSequence();
    }

    private void StartSequence()
    {
        StartCoroutine(WaitASecond(5f)); // 화면을 페이드아웃하고 잠시 대기
        StartCoroutine(ShowIntro());    // 인트로 메시지를 모두 출력

    }

    IEnumerator ShowIntro()
    {
        string initialText;
        initialText = "Dispatcher : ";

        foreach (string message in dispatch)
        {
            yield return new WaitForSeconds(3.5f);
            introSoundManager.PlayDispatchSound();
            
            yield return StartCoroutine(ShowIntroDispatch(introTexts[textIndex], message, initialText));
            textIndex++;
        }

        initialText = "Police Officer : ";
        yield return new WaitForSeconds(3.5f);
        introSoundManager.PlayDispatchSound();

        yield return StartCoroutine(ShowIntroDispatch(introTexts[textIndex], officerResponse, initialText));
        textIndex++;
        yield return new WaitForSeconds(3.5f);

        screen.transform.SetSiblingIndex(screen.transform.parent.childCount - 1);        

        yield return StartCoroutine(fadeUtility.FadeIn(screen, 3f));
        rainPrefab.gameObject.SetActive(false);
        yield return StartCoroutine(WaitNextScene()); // 다음 씬으로 전환
    }

    private IEnumerator ShowIntroDispatch(Text t, string dispatch, string initialText = "")
    {
        t.text = initialText; // 텍스트 초기화

        foreach (char letter in dispatch)
        {
            t.text += letter; // 한 글자씩 추가
            yield return new WaitForSeconds(0.05f); // 0.03초 대기
        }
    }

    private IEnumerator WaitASecond(float second)
    {
        yield return new WaitForSeconds(second);
        StartCoroutine(fadeUtility.FadeOut(screen, 3f));
    }

    private IEnumerator WaitNextScene()
    {
        // 현재 씬 인덱스를 기준으로 다음 씬을 비동기로 로드
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(currentSceneIndex + 1);

        // 로딩이 완료될 때까지 대기
        while (!asyncLoad.isDone)
        {
            yield return null; // 매 프레임 대기
        }
    }

    /// <summary>
    /// dispatch, police officer response 문자 일정 간격으로 배치
    /// </summary>
    void SetTextsPosition()
    {
        for(int i = 0; i < 6; i++)
        {
            // RectTransform을 가져와서 위치 및 크기 설정
            RectTransform rectTransform = introTexts[i].GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                if(i == 5)
                {
                    rectTransform.anchoredPosition = new Vector2(0, -100 + -i * verticalSpacing);
                    break;
                }

                // 텍스트의 y 좌표를 일정 간격으로 띄우며 배치합니다
                rectTransform.anchoredPosition = new Vector2(0, -30 + -i * verticalSpacing);
            }
        }
    }
}
