using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EndSceneManager : MonoBehaviour
{
    public LayerMask npcLayer;

    public AudioClip[] nasonClips;
    public AudioClip[] jennyClips;
    public AudioClip[] minaClips;

    public EndCamera cam;
    public GameObject ox;
    public GameObject mouseDescription;
    public Image screen;
    public Image chatBox;
    public Text endText;
    public Text endingCredit;
    public Text finalStatement;
    public Text selectNPC;
    public Transform[] NPCTransform;        // 3명의 NPC


    private bool isClicked = false;     // npc가 클릭되었는가
    private int index = 0;              // 최후의 진술 인덱스
    private int npcNum = -1;

    private float moveDistance = Screen.height * 1.6f;
    private float duration = 15f; // 이동하는 데 걸리는 시간

    private string[] nasonFinalStatement;
    private string[] jennyFinalStatement;
    private string[] minaFinalStatement;
    private string[][] allFinalStatements;

    private string[] nasonEnd;
    private string[] jennyEnd;
    private string[] minaEnd;

    private AudioClip[][] allClips;
    private GameObject hoveredObject = null;

    // Start is called before the first frame update
    void Start()
    {
        screen.gameObject.SetActive(true);
        StartCoroutine(FadeUtility.Instance.FadeOut(screen, 2f));        

        selectNPC.gameObject.SetActive(false);
        ox.gameObject.SetActive(false);
        chatBox.gameObject.SetActive(false);

        StartCoroutine(SoundManager.Instance.FadeOutAndChangeClip(SoundManager.Instance.GetSelectCriminalBGM()));

        SetStatements();
        SetEndText();
        SetClips();

        StartCoroutine(FadeUtility.Instance.FadeIn(selectNPC, 1));
    }

    void Update()
    {
        // 마우스 위치에서 레이 생성
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 마우스가 npcLayer에 해당하는 오브젝트 위에 있는지 확인
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, npcLayer))
        {
            GameObject currentHoveredObject = hit.collider.gameObject;

            // Hover 상태 처리
            if (hoveredObject != currentHoveredObject && !isClicked)
            {
                hoveredObject = currentHoveredObject;
                mouseDescription.gameObject.SetActive(true);

                switch (hoveredObject.name)
                {
                    case "Nason":
                        mouseDescription.transform.GetChild(1).GetComponent<Text>().text = "네이슨";
                        break;
                    case "Jenny":                        
                        mouseDescription.transform.GetChild(1).GetComponent<Text>().text = "제니";
                        break;
                    case "Mina":                        
                        mouseDescription.transform.GetChild(1).GetComponent<Text>().text = "미나";
                        break;
                }
            }

            // 클릭했을 때 실행
            if (Input.GetMouseButtonDown(0) && !isClicked)
            {
                int tmpIndex = -1;
                isClicked = true;

                switch (hoveredObject.name)
                {
                    case "Nason":
                        tmpIndex = 2;
                        chatBox.transform.GetChild(1).GetComponent<Text>().text = "네이슨";
                        break;
                    case "Jenny":
                        tmpIndex = 1;
                        chatBox.transform.GetChild(1).GetComponent<Text>().text = "제니";
                        break;
                    case "Mina":
                        tmpIndex = 0;
                        chatBox.transform.GetChild(1).GetComponent<Text>().text = "미나";
                        break;
                }

                mouseDescription.gameObject.SetActive(false);
                mouseDescription.transform.GetChild(1).GetComponent<Text>().text = "";

                SelectCriminal(tmpIndex);
            }
        }
        else
        {
            // 마우스가 NPC에서 벗어났을 때 hoveredObject 초기화
            hoveredObject = null;
            mouseDescription.gameObject.SetActive(false);
            mouseDescription.transform.GetChild(1).GetComponent<Text>().text = "";
        }
    }
    /// <summary>
    /// 최후의 진술 대사 설정
    /// </summary>
    void SetStatements()
    {
        nasonFinalStatement = new string[] {
            "전 절대로 앨런을 죽이지 않았습니다!",
            "전 독에 대해 아는 것이 아무것도 없어요!",
            "애초에 저희 중 범인이 있는 것이 확실합니까?"
        };

        jennyFinalStatement = new string[] {
            "약학에 박식하다는 이유만으로 저를 범인으로 지목할 수는 없어요!",
            "제 프로젝트가 앨런에 의해 무산되었다 해도 그게 살인 동기가 될 수 있다는 것인가요?",
            "전 연인이였던 미나가 오히려 앨런을 살해할 동기가 있어보여요."
        };

        minaFinalStatement = new string[] {
            "전 앨런을 죽인 범인이 아니에요!",
            "앨런을 죽인 범인이 밝혀진다면 반드시 복수하겠어요!",
            "경찰관님의 현명한 판단을 바랄께요."
        };

        allFinalStatements = new string[][] { minaFinalStatement, jennyFinalStatement, nasonFinalStatement };
    }

    /// <summary>
    /// 결말 전개 텍스트 설정
    /// </summary>
    void SetEndText()
    {
        nasonEnd = new string[] {
            "네이슨을 범인으로 지목하자,\n네이슨은 고개를 숙인 채 한숨을 내쉽니다.",
            "그는 한참 동안 침묵하다가\n차분한 목소리로 말합니다.",
            "\"정말로 제가 범인이라고 생각하십니까?\"",
            "\"앨런과 오랜 친구로서 갈등도 있었고\n그의 지병으로 인해 화가 난 적도 많았습니다.\"",
            "\"하지만, 앨런을 죽일 이유는 없습니다!\"",
            "\"저는 그를 도우려 했을 뿐입니다.\"",
            "네이슨의 눈은 슬픔으로 가득 차 있으며",
            "자신이 친구를 죽였다고\n오해받은 것에 대한 상처가 깊이 배어 있습니다.",
            "조사 과정에서 그는 앨런을 위해 헌신했지만\n사업 문제와 앨런의 우울증 때문에 자주 충돌하게 되었음을 설명합니다.",
            "사건이 종결될 무렵,\n경찰은 새롭게 발견된 증거를 통해 제니가 진범임을 밝혀냅니다.",
            "제니가 앨런에 대해 오랫동안 품어왔던\n분노와 복수심이 범행의 동기였음을 확인하였습니다.",
            "하지만 앨런이 회사의 자금을 이용하여\n투기성 투자를 한 사실 또한 밝혀졌고",
            "이에 네이슨이 가담한 사실도 밝혀졌습니다.",
            "네이슨은 법적 책임을 피할 수 없게 되며",
            "살인 혐의에서는 벗어났지만 회계 부정과\n자금 유용에 대한 책임을 지고 법정에 서게 됩니다.",
            "네이슨은 친구를 위해 내린\n잘못된 선택을 평생 동안 짊어지게 될 것입니다."
        };

        jennyEnd = new string[] {
            "제니를 범인으로 지목하자 얼굴에\n씁쓸한 미소를 띠며 천천히 고개를 듭니다.",
            "\"그래요, 제가 했습니다.\"",
            "\"앨런이 내게 남긴 상처는 너무나 깊었고\n그를 그냥 두고 볼 수는 없었습니다.\"",
            "제니의 목소리에는 긴 세월 동안\n억눌려 온 분노와 상처가 배어 있으며",
            "감정이 터져 나올 듯이 흔들리고 있습니다.",
            "제니는 대학 시절부터 앨런에게 느꼈던\n열등감과 무시당한 경험을 고백합니다.",
            "그녀는 항상 앨런보다 뒤처진다는 생각에 괴로워했고",
            "앨런이 자신의 신약 연구 프로젝트를 무산시키며\n마지막 희망마저 짓밟았을 때 모든 인내심이 한계에 다다랐다고 말합니다.",
            "그녀는 이번 모임이 앨런에게\n복수할 수 있는 절호의 기회라고 생각했고",
            "그를 독살할 계획을 세웠다고 고백합니다.",
            "그녀는 앨런에게 마지막 복수를 결심했을 때\n느꼈던 차가운 만족감을 떠올리며",
            "앨런이 자신에게 남긴 상처만큼\n그에게도 고통을 안기고 싶었다고 말합니다.",
            "제니는 모든 죄를 인정하고 체포됩니다.",
            "그녀는 더 큰 공허함과 상실감을 느끼며\n앨런의 그림자에서 벗어나지 못하는 자신을 발견합니다",
            "분노와 복수의 끝에서\n제니는 고독한 결말을 맞이했습니다.",            
        };

        minaEnd = new string[] {
            "미나를 범인으로 지목하자, 충격에 사로잡혀\n아무 말 없이 잠시 눈을 감고 깊은 숨을 내쉽니다.",
            "그녀는 천천히 고개를 들어\n침울한 표정으로 조용히 입을 엽니다.",
            "\"앨런을 사랑했습니다...\"",
            "\"하지만 그의 죽음을 바란 적은 없습니다.\"",
            "\"그에게 남아 있던 제 마음은 오로지 사랑과 미련이었어요.\"",
            "\"그가 떠난 지금, 그마저도 잃어버린 셈이죠.\"",
            "앨런과 대학 시절 연인이었다는 것,",
            "그가 사업을 시작하면서 멀어졌지만\n여전히 그를 사랑했음을 고백합니다.",
            "그녀는 앨런이 자신을 초대했을 때\n서로의 관계를 회복하려 한다고 믿고 흔쾌히 수락했다는 사실을 털어놓습니다.",
            "경찰은 사건을 재조사하여 새로운 증거를 통해\n진범이 제니임을 밝혀냅니다.",
            "제니가 앨런에 대해 오랫동안 품어왔던\n분노와 복수심이 범행의 동기였음을 확인하였습니다.",
            "미나는 사랑했던 사람을 잃고\n그를 죽였다는 의심까지 받으며 깊은 상처를 입었습니다.",
            "그녀의 마지막 선택은 씁쓸한 여운을 남기며\n사랑과 미련이 만들어낸 비극으로 마무리되었습니다."
        };
    }

    void SetClips()
    {
        allClips = new AudioClip[][] { minaClips, jennyClips, nasonClips };
    }

    public void SelectCriminal(int npcIndex)
    {
        npcNum = npcIndex;

        StartCoroutine(FadeUtility.Instance.FadeOut(selectNPC, 1));

        if (!chatBox.gameObject.activeSelf)
        {
            StartCoroutine(FadeUtility.Instance.FadeIn(chatBox, 1f, 0.15f));
            StartCoroutine(FadeUtility.Instance.FadeIn(chatBox.transform.GetChild(0).GetComponent<Graphic>(), 1f));
            StartCoroutine(FadeUtility.Instance.FadeIn(chatBox.transform.GetChild(1).GetComponent<Graphic>(), 1f));
            StartCoroutine(FadeUtility.Instance.FadeIn(chatBox.transform.GetChild(2).GetComponent<Graphic>(), 1f));
        }

        cam.FocusNPC(NPCTransform[npcIndex]);
        NPCTransform[npcIndex].parent.GetComponent<EndNPC>().TurnTowardPlayer(cam.transform);

        StartCoroutine(ShowLine(finalStatement, allFinalStatements[npcIndex][index++ % 3], NPCTransform[npcIndex].parent.GetComponent<EndNPC>()));
    }


    public void OButtton()
    {
        StartCoroutine(SoundManager.Instance.FadeOutAndChangeClip(SoundManager.Instance.GetEndingBGM()));
        StartCoroutine(OButtonCourutine(GetEnd()));
    }

    public IEnumerator OButtonCourutine(string[] endStrings)
    {
        int tmpIndex = 0;
        yield return StartCoroutine(FadeUtility.Instance.FadeIn(screen, 2f));

        foreach (string str in endStrings)
        {
            endText.text = "";

            // str이 \"로 시작하는지 확인
            if (str.StartsWith("\""))
            {
                yield return StartCoroutine(PlayNPCSound(endText, str, allClips[npcNum][tmpIndex++], 0.1f));
            }
            else
            {
                yield return StartCoroutine(ShowEnding(endText, str, 0.05f));
            }
            
            yield return new WaitForSeconds(2.5f);
        }

        endText.text = "";

        StopCoroutine(OButtonCourutine(GetEnd()));

        StartCoroutine(ShowEndingCredit());
    }

    private string[] GetEnd()
    {
        switch (npcNum)
        {
            case 0: // mina
                return minaEnd;
            case 1:
                return jennyEnd;
            case 2:
                return nasonEnd;

            default:
                Debug.LogError("npcIndex 오류!");
                return null;
        }
    }

    public void XButton()
    {
        cam.FocusAndReturnToOriginal();

        if (chatBox.gameObject.activeSelf)
        {
            StartCoroutine(FadeUtility.Instance.FadeOut(chatBox, 1f, 0.15f));
            StartCoroutine(FadeUtility.Instance.FadeOut(chatBox.transform.GetChild(0).GetComponent<Graphic>(), 1f));
            StartCoroutine(FadeUtility.Instance.FadeOut(chatBox.transform.GetChild(1).GetComponent<Graphic>(), 1f));
            StartCoroutine(FadeUtility.Instance.FadeOut(chatBox.transform.GetChild(2).GetComponent<Graphic>(), 1f));
            
            StartCoroutine(FadeUtility.Instance.FadeOut(ox.transform.GetChild(0).GetComponent<Graphic>(), 1f));
            StartCoroutine(FadeUtility.Instance.FadeOut(ox.transform.GetChild(1).GetComponent<Graphic>(), 1f));            
            ox.gameObject.SetActive(false);
        }

        isClicked = false;
        StartCoroutine(FadeUtility.Instance.FadeIn(selectNPC, 1));
    }




    public IEnumerator ShowLine(Text t, string answer, EndNPC npc, float second = 0.1f)
    {
        t.text = ""; // 텍스트 초기화
        yield return new WaitForSeconds(1f); // 1초 대기

        npc.PlayEmotion(answer);        

        Coroutine dialogSoundCoroutine = null;

        dialogSoundCoroutine = StartCoroutine(PlayDialogSound());

        for (int i = 0; i < answer.Length; i++)
        {
            t.text += answer[i]; // 한 글자씩 추가
            yield return new WaitForSeconds(second); 
        }

        // 코루틴이 실행되었을 경우에만 종료 처리
        if (dialogSoundCoroutine != null)
        {
            StopCoroutine(dialogSoundCoroutine); // PlayDialogSound 코루틴 중지

            ox.gameObject.SetActive(true);
            StartCoroutine(FadeUtility.Instance.FadeIn(ox.transform.GetChild(0).GetComponent<Graphic>(), 1f));
            StartCoroutine(FadeUtility.Instance.FadeIn(ox.transform.GetChild(1).GetComponent<Graphic>(), 1f));

            SoundManager.Instance.StopTextSound();
        }
    }

    public IEnumerator PlayNPCSound(Text t, string answer, AudioClip npcSound, float second = 0.1f)
    {
        t.text = ""; // 텍스트 초기화
        yield return new WaitForSeconds(1f); // 1초 대기

        SoundManager.Instance.ChangeTextAudioClip(npcSound);
        SoundManager.Instance.PlayTextSound();

        for (int i = 0; i < answer.Length; i++)
        {
            t.text += answer[i]; // 한 글자씩 추가
            yield return new WaitForSeconds(second);
        }

        // SoundManager.Instance.StopTextSound();
    }

    public IEnumerator ShowEnding(Text t, string answer, float second = 0.1f)
    {
        t.text = ""; // 텍스트 초기화
        yield return new WaitForSeconds(1f); // 1초 대기

        Coroutine dialogSoundCoroutine = null;

        SoundManager.Instance.SetTypingClip();
        dialogSoundCoroutine = StartCoroutine(PlayDialogSound());

        for (int i = 0; i < answer.Length; i++)
        {
            t.text += answer[i]; // 한 글자씩 추가
            yield return new WaitForSeconds(second);
        }

        // 코루틴이 실행되었을 경우에만 종료 처리
        if (dialogSoundCoroutine != null)
        {
            StopCoroutine(dialogSoundCoroutine); // PlayDialogSound 코루틴 중지

            ox.gameObject.SetActive(true);
            StartCoroutine(FadeUtility.Instance.FadeIn(ox.transform.GetChild(0).GetComponent<Graphic>(), 1f));
            StartCoroutine(FadeUtility.Instance.FadeIn(ox.transform.GetChild(1).GetComponent<Graphic>(), 1f));

            SoundManager.Instance.StopTextSound();
        }
    }

    IEnumerator PlayDialogSound()
    {
        while (true) // 무한 루프를 사용하여 반복 실행
        {
            SoundManager.Instance.PlayTextSound();
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator ShowEndingCredit()
    {
        RectTransform rectTransform = endingCredit.GetComponent<RectTransform>();
        Vector3 startPosition = rectTransform.localPosition;
        Vector3 endPosition = startPosition + new Vector3(0, moveDistance, 0);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            rectTransform.localPosition = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rectTransform.localPosition = endPosition; // 정확한 끝 위치로 설정

        StartCoroutine(EndGameAfterDelay());
    }

    private IEnumerator EndGameAfterDelay()
    {
        FadeUtility.Instance.FadeOut(endingCredit, 2f);
        yield return new WaitForSeconds(10f); // 10초 대기                
        Application.Quit(); // 빌드된 게임 종료
    }
}
