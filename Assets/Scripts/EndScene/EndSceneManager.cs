using Steamworks;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class EndSceneManager : MonoBehaviour
{
    public LayerMask npcLayer;
    public EndCamera cam;
    public Transform[] NPCTransform;        // 3명의 NPC

    [Header("UI")]
    public GameObject ox;
    public GameObject mouseDescription;
    public Image screen;
    public Image chatBox;
    public Text finalStatement;
    public Text selectNPC;

    [Header("EndingCredit")]
    public Text finalDialogueText;
    public Graphic endingCredit;
    public Image NPCEndingImage;
    public Sprite[] nasonEndingImages;
    public Sprite[] jennyEndingImages;
    public Sprite[] minaEndingImages;
    private int[] nasonEndingIndex = { 0, 4, 9, 11, 13 };
    private int[] jennyEndingIndex = { 0, 3, 8, 10, 13 };
    private int[] minaEndingIndex = { 0, 2, 6, 9, 11 };
    
    [Header("Audio")]
    public AudioClip[] nasonClips;
    public AudioClip[] jennyClips;
    public AudioClip[] minaClips;

    [Header("2배속 모드")]
    public GameObject _2x_Parent;       // 2배속 클래스
    private _2x _2xClass;

    private bool isClicked = false;         // npc가 클릭되었는가
    private bool isReadyToClick = false;    // 엔드 씬 클릭 버그 방지
    private bool isEndingStarted = false; // 중복 실행 방지용 플래그
    private int index = 0;                  // 최후의 진술 인덱스
    private int npcNum = -1;                // 선택된 NPC의 번호

    // NPC별 최후의 진술
    private string[] nasonFinalStatement;
    private string[] jennyFinalStatement;
    private string[] minaFinalStatement;
    private string[][] allFinalStatements;

    // NPC별 엔딩 대사
    private string[] nasonEnd;
    private string[] jennyEnd;
    private string[] minaEnd;

    private AudioClip[][] allClips;
    private GameObject hoveredObject = null;    // 마우스가 가리키는 NPC    
    private Locale locale; // 로컬라이제이션 설정

    // Start is called before the first frame update
    void Start()
    {
        screen.gameObject.SetActive(true);
        NPCEndingImage.gameObject.SetActive(false);
        StartCoroutine(FadeUtility.Instance.FadeOut(screen, 2f));

        selectNPC.gameObject.SetActive(true);
        ox.gameObject.SetActive(false);
        chatBox.gameObject.SetActive(false);
        _2xClass = new _2x(_2x_Parent);

        StartCoroutine(SoundManager.Instance.FadeOutAndChangeClip(SoundManager.Instance.GetSelectCriminalBGM()));

        locale = LocalizationSettings.SelectedLocale;

        SetStatements();
        SetEndText();
        SetClips();        

        StartCoroutine(WaitClick(false, screen, 2f));
    }

    void Update()
    {
        if (isReadyToClick)
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
                            if (locale.Identifier.Code == "en")
                            {
                                mouseDescription.transform.GetChild(1).GetComponent<Text>().text = "Nason";
                            }
                            else if (locale.Identifier.Code == "ja")
                            {
                                mouseDescription.transform.GetChild(1).GetComponent<Text>().text = "ネイソン";
                            }
                            else if (locale.Identifier.Code == "ko")
                            {
                                mouseDescription.transform.GetChild(1).GetComponent<Text>().text = "네이슨";
                            }                            
                            break;
                        case "Jenny":
                            if (locale.Identifier.Code == "en")
                            {
                                mouseDescription.transform.GetChild(1).GetComponent<Text>().text = "Jenny";
                            }
                            else if (locale.Identifier.Code == "ja")
                            {
                                mouseDescription.transform.GetChild(1).GetComponent<Text>().text = "ジェニー";
                            }
                            else if (locale.Identifier.Code == "ko")
                            {
                                mouseDescription.transform.GetChild(1).GetComponent<Text>().text = "제니";
                            }
                            break;
                        case "Mina":
                            if (locale.Identifier.Code == "en")
                            {
                                mouseDescription.transform.GetChild(1).GetComponent<Text>().text = "Mina";
                            }
                            else if (locale.Identifier.Code == "ja")
                            {
                                mouseDescription.transform.GetChild(1).GetComponent<Text>().text = "ミナ";
                            }
                            else if (locale.Identifier.Code == "ko")
                            {
                                mouseDescription.transform.GetChild(1).GetComponent<Text>().text = "미나";
                            }
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
                            if (locale.Identifier.Code == "en")
                            {
                                tmpIndex = 2;
                                chatBox.transform.GetChild(1).GetComponent<Text>().text = "Nason";
                            }
                            else if (locale.Identifier.Code == "ja")
                            {
                                tmpIndex = 2;
                                chatBox.transform.GetChild(1).GetComponent<Text>().text = "ネイソン";
                            }
                            else if (locale.Identifier.Code == "ko")
                            {
                                tmpIndex = 2;
                                chatBox.transform.GetChild(1).GetComponent<Text>().text = "네이슨";
                            }                            
                            break;
                        case "Jenny":
                            if (locale.Identifier.Code == "en")
                            {
                                tmpIndex = 1;
                                chatBox.transform.GetChild(1).GetComponent<Text>().text = "Jenny";
                            }
                            else if (locale.Identifier.Code == "ja")
                            {
                                tmpIndex = 1;
                                chatBox.transform.GetChild(1).GetComponent<Text>().text = "ジェニー";
                            }
                            else if (locale.Identifier.Code == "ko")
                            {
                                tmpIndex = 1;
                                chatBox.transform.GetChild(1).GetComponent<Text>().text = "제니";
                            }
                            break;
                        case "Mina":
                            if (locale.Identifier.Code == "en")
                            {
                                tmpIndex = 0;
                                chatBox.transform.GetChild(1).GetComponent<Text>().text = "Mina";
                            }
                            else if (locale.Identifier.Code == "ja")
                            {
                                tmpIndex = 0;
                                chatBox.transform.GetChild(1).GetComponent<Text>().text = "ミナ";
                            }
                            else if (locale.Identifier.Code == "ko")
                            {
                                tmpIndex = 0;
                                chatBox.transform.GetChild(1).GetComponent<Text>().text = "미나";
                            }
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
    }

    /// <summary>
    /// 최후의 진술 대사 설정
    /// </summary>
    void SetStatements()
    {
        if (locale.Identifier.Code == "en")
        {
            nasonFinalStatement = new string[] {
            "I would never kill Alan!",
            "I don’t know anything about poison!",
            "Are we even sure the culprit is one of us?"
        };

            jennyFinalStatement = new string[] {
            "You can’t accuse me just because I’m knowledgeable in pharmacology!",
            "Even if Alan sabotaged my project, does that really make it a motive for murder?",
            "Mina, his ex-girlfriend, seems to have more of a motive to kill Alan than I do."
        };

            minaFinalStatement = new string[] {
            "I’m not the one who killed Alan!",
            "If the one who killed Alan is revealed, I’ll make sure to get revenge!",
            "I trust in your wise judgment, officer."
        };
        }
        else if (locale.Identifier.Code == "ja")
        {
            nasonFinalStatement = new string[] {
            "僕がアランを殺すなんて、絶対にありえません！",
            "毒については何も知りません！",
            "そもそも、犯人が僕たちの中にいると確信できますか？"
        };

            jennyFinalStatement = new string[] {
            "薬学に詳しいという理由だけで、私を犯人扱いしないでください！",
            "アランにプロジェクトを潰されたとしても、それが殺人の動機になるんですか？",
            "元恋人のミナの方が、アランを殺す動機があるように思えます。"
        };

            minaFinalStatement = new string[] {
            "私はアランを殺した犯人じゃありません！",
            "アランを殺した犯人が分かったら、必ず復讐します！",
            "警察官さんの賢明な判断を信じています。"
        };
        }
        else if (locale.Identifier.Code == "ko")
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

        }


        allFinalStatements = new string[][] { minaFinalStatement, jennyFinalStatement, nasonFinalStatement };
    }

    /// <summary>
    /// 결말 전개 텍스트 설정
    /// </summary>
    void SetEndText()
    {
        if (locale.Identifier.Code == "en")
        {
            nasonEnd = new string[] {
            "When Nason is identified as the culprit,\nhe lowers his head and lets out a sigh.",
            "After a long silence,\r\nhe speaks in a calm voice.",
            "\"Do you truly believe I’m the culprit?\"",
            "\"As a longtime friend of Alan,\nwe had our conflicts,\nand I was often angry about his illness.\"",
            "\"But I had no reason to kill him!\"",
            "\"I only wanted to help him.\"",
            "Nason’s eyes are filled with sorrow,",
            "deeply wounded by being misunderstood\nas his friend’s killer.",
            "During the investigation, he explains\nthat he was devoted to helping Alan but often clashed with him\nover business issues and Alan’s depression.",
            "As the case nears its conclusion,\r\nthe police discover new evidence\nthat reveals Jenny as the true culprit.",
            "It is confirmed that Jenny's long-held anger\r\nand desire for revenge against Alan\nwere the motives behind the crime.",
            "However, it is also revealed\nthat Alan used company funds\nfor speculative investments,",
            "and that Nason was complicit in this.",
            "Nason is unable to escape legal responsibility.",
            "Though cleared of the murder charge,\r\nhe stands trial for accounting fraud and embezzlement.",
            "Nason will carry the burden\r\nof his wrong choice made for a friend\nfor the rest of his life."
            };

            jennyEnd = new string[] {
            "When Jenny is identified as the culprit,\r\nshe lifts her head slowly with a bitter smile on her face.",
            "\"Yes, I did it.\"",
            "\"The wound Alan left in me was too deep...\r\nI couldn’t just let it go.\"",
            "Her voice carries the weight\nof long-suppressed anger and pain,",
            "trembling on the verge of emotional outburst.",
            "Jenny confesses the inferiority complex\r\nand the feeling of being ignored\nby Alan since their university days.",
            "She says she was always tormented\nby the thought that she was falling behind Alan.",
            "And when Alan sabotaged her new drug research project,\r\ncrushing her last hope,\r\nher patience reached its limit.",
            "She thought this reunion\nwas the perfect opportunity\nto take revenge on Alan.",
            "She confesses that she planned to poison him.",
            "Recalling the cold satisfaction\nshe felt when she decided to carry out her revenge,",
            "she says she wanted Alan to feel pain\r\njust as deeply as the pain he left in her.",
            "Jenny admits to all her crimes and is arrested.",
            "She is overwhelmed by an even greater emptiness and loss,\r\nrealizing she still can’t escape Alan’s shadow.",
            "At the end of her rage and vengeance,\r\nJenny meets a lonely fate.",
        };

            minaEnd = new string[] {
            "When Mina is identified as the culprit,\r\nshe closes her eyes in shock\nand takes a deep breath in silence.",
            "Slowly lifting her head,\r\nshe begins to speak with a sorrowful expression.",
            "\"I loved Alan...\"",
            "\"But I never wished for his death.\"",
            "\"What remained in my heart\nfor him was only love... and longing.\"",
            "\"Now that he's gone, I've lost even that.\"",
            "She confesses that she and Alan were lovers in college,",
            "And though they grew apart\nwhen he started his business,\r\nshe still loved him.",
            "She reveals that when Alan invited her,\r\nshe believed it was a chance to repair their relationship\nand accepted willingly.",
            "The police reinvestigate the case,\r\nand new evidence reveals that Jenny is the true culprit.",
            "Jenny’s long-held anger and desire\nfor revenge toward Alan are confirmed\nas the motives for the crime.",
            "Mina not only lost the man she loved,\r\nbut was also deeply wounded\nby the suspicion of having killed him.",
            "Her final choice leaves behind a bitter aftertaste,\r\nconcluding in a tragedy born from love and lingering affection."
        };

        }
        else if (locale.Identifier.Code == "ja")
        {
            nasonEnd = new string[] {
            "ネイソンが犯人として指摘されると、\r\n彼はうつむいてため息をつきます。",
            "長い沈黙の後、\r\n落ち着いた声で話し始めます。",
            "「本当に私が犯人だと思いますか？」",
            "「アランとは長年の友人で、衝突もありましたし、\r\n彼の持病に苛立ったこともあります。」",
            "「でも、アランを殺す理由なんてありません！」",
            "「私は彼を助けたかっただけです。」",
            "ネイソンの目は悲しみに満ちており、",
            "友人を殺したと誤解されたことに深く傷ついています。",
            "彼は捜査の過程で、\r\nアランを支えるために尽力していたが、\r\nビジネスの問題やアランのうつ病で衝突が多かったと説明します。",
            "事件の終盤、\r\n新たに発見された証拠により、\r\n真犯人がジェニーであることが警察によって明らかになります。",
            "ジェニーがアランに対して長年抱いていた\r\n怒りと復讐心が犯行の動機であったことが確認されました。",
            "しかし、アランが会社の資金を使って\r\n投機的な投資をしていた事実も明らかになり、",
            "それにネイソンが関与していたことも判明します。",
            "ネイソンは法的責任を免れることはできず、",
            "殺人の容疑は晴れたものの、\r\n会計不正と資金流用の責任を問われて法廷に立つことになります。",
            "ネイソンは友人のために下した間違った選択を、\r\n一生背負って生きていくことになるでしょう。"
        };

            jennyEnd = new string[] {
            "ジェニーが犯人として指摘されると、\r\n彼女は苦い笑みを浮かべながら、ゆっくりと顔を上げます。",
            "「そうです、私がやりました。」",
            "「アランが私に残した傷はあまりにも深く、\r\n見過ごすことなんてできなかったんです。」",
            "彼女の声には、長年押し殺してきた\r\n怒りと傷がにじんでおり、",
            "感情が今にも溢れ出しそうに揺れています。",
            "ジェニーは大学時代からアランに感じていた劣等感と、\r\n無視されてきた経験を告白します。",
            "彼女は常にアランに遅れを取っているという思いに苦しみ、",
            "アランが彼女の新薬研究プロジェクトを潰したとき、\r\n最後の希望さえも踏みにじられたと語ります。",
            "彼女は今回の再会が、\r\nアランに復讐できる絶好の機会だと考えました。",
            "そして、彼を毒殺する計画を立てたと告白します。",
            "復讐を決意した時に感じた\r\n冷たい満足感を思い出しながら、",
            "彼女は、アランが自分に残した傷と同じくらい\r\n彼にも苦しみを与えたかったと語ります。",
            "ジェニーはすべての罪を認め、逮捕されます。",
            "彼女はさらに大きな虚無と喪失感に襲われ、\r\nいまだにアランの影から抜け出せない自分に気づきます。",
            "怒りと復讐の果てに、\r\nジェニーは孤独な結末を迎えました。",
        };

            minaEnd = new string[] {
            "ミナが犯人として指摘されると、\r\n彼女はショックで黙ったまま目を閉じ、深く息を吐きます。",
            "ゆっくりと顔を上げ、\r\n沈んだ表情で静かに口を開きます。",
            "「アランを愛していました...」",
            "「でも、彼の死を望んだことは一度もありません。」",
            "「彼に対する私の気持ちは、愛と未練だけでした。」",
            "「彼がいなくなった今、その気持ちさえも失ってしまいました。」",
            "彼女はアランと大学時代に恋人だったこと、",
            "彼が事業を始めてから距離ができたが、\r\nそれでもずっと彼を愛していたと告白します。",
            "アランから招待を受けたとき、\r\n関係を修復できる機会だと信じて、喜んで応じたことも明かします。",
            "警察は事件を再調査し、\r\n新たな証拠により、真犯人がジェニーであることが明らかになります。",
            "ジェニーが長年アランに抱いていた\r\n怒りと復讐心が犯行の動機だったことが確認されました。",
            "ミナは愛する人を失っただけでなく、\r\n彼を殺したと疑われたことで深い傷を負いました。",
            "彼女の最後の選択はほろ苦い余韻を残し、\r\n愛と未練が生んだ悲劇で幕を閉じました。"
        };

        }
        else if (locale.Identifier.Code == "ko")
        {
            nasonEnd = new string[] {
            "네이슨을 범인으로 지목하자,\n네이슨은 고개를 숙인 채 한숨을 내쉽니다.",     // 1
            "그는 한참 동안 침묵하다가\n차분한 목소리로 말합니다.",
            "\"정말로 제가 범인이라고 생각하십니까?\"",
            "\"앨런과 오랜 친구로서 갈등도 있었고\n그의 지병으로 인해 화가 난 적도 많았습니다.\"",
            "\"하지만, 앨런을 죽일 이유는 없습니다!\"",        // 2
            "\"저는 그를 도우려 했을 뿐입니다.\"",
            "네이슨의 눈은 슬픔으로 가득 차 있으며",
            "자신이 친구를 죽였다고\n오해받은 것에 대한 상처가 깊이 배어 있습니다.",
            "조사 과정에서 그는 앨런을 위해 헌신했지만\n사업 문제와 앨런의 우울증 때문에 자주 충돌하게 되었음을 설명합니다.",
            "사건이 종결될 무렵,\n경찰은 새롭게 발견된 증거를 통해 제니가 진범임을 밝혀냅니다.",      // 3
            "제니가 앨런에 대해 오랫동안 품어왔던\n분노와 복수심이 범행의 동기였음을 확인하였습니다.",
            "하지만 앨런이 회사의 자금을 이용하여\n투기성 투자를 한 사실 또한 밝혀졌고",   // 4
            "이에 네이슨이 가담한 사실도 밝혀졌습니다.",
            "네이슨은 법적 책임을 피할 수 없게 되며",       // 5
            "살인 혐의에서는 벗어났지만 회계 부정과\n자금 유용에 대한 책임을 지고 법정에 서게 됩니다.",
            "네이슨은 친구를 위해 내린\n잘못된 선택을 평생 동안 짊어지게 될 것입니다."
            };

            jennyEnd = new string[] {
            "제니를 범인으로 지목하자 얼굴에\n씁쓸한 미소를 띠며 천천히 고개를 듭니다.",       // 1
            "\"그래요, 제가 했습니다.\"",
            "\"앨런이 내게 남긴 상처는 너무나 깊었고\n그를 그냥 두고 볼 수는 없었습니다.\"",
            "제니의 목소리에는 긴 세월 동안\n억눌려 온 분노와 상처가 배어 있으며",      // 2
            "감정이 터져 나올 듯이 흔들리고 있습니다.",
            "제니는 대학 시절부터 앨런에게 느꼈던\n열등감과 무시당한 경험을 고백합니다.",       
            "그녀는 항상 앨런보다 뒤처진다는 생각에 괴로워했고",
            "앨런이 자신의 신약 연구 프로젝트를 무산시키며\n마지막 희망마저 짓밟았을 때 모든 인내심이 한계에 다다랐다고 말합니다.",
            "그녀는 이번 모임이 앨런에게\n복수할 수 있는 절호의 기회라고 생각했고",      // 3
            "그를 독살할 계획을 세웠다고 고백합니다.",
            "그녀는 앨런에게 마지막 복수를 결심했을 때\n느꼈던 차가운 만족감을 떠올리며",       // 4
            "앨런이 자신에게 남긴 상처만큼\n그에게도 고통을 안기고 싶었다고 말합니다.",
            "제니는 모든 죄를 인정하고 체포됩니다.",
            "그녀는 더 큰 공허함과 상실감을 느끼며\n앨런의 그림자에서 벗어나지 못하는 자신을 발견합니다",      // 5
            "분노와 복수의 끝에서\n제니는 고독한 결말을 맞이했습니다.",
            };

            minaEnd = new string[] {
            "미나를 범인으로 지목하자, 충격에 사로잡혀\n아무 말 없이 잠시 눈을 감고 깊은 숨을 내쉽니다.",    // 1
            "그녀는 천천히 고개를 들어\n침울한 표정으로 조용히 입을 엽니다.",
            "\"앨런을 사랑했습니다...\"",        // 2
            "\"하지만 그의 죽음을 바란 적은 없습니다.\"",
            "\"그에게 남아 있던 제 마음은 오로지 사랑과 미련이었어요.\"",
            "\"그가 떠난 지금, 그마저도 잃어버린 셈이죠.\"",
            "앨런과 대학 시절 연인이었다는 것,",      // 3
            "그가 사업을 시작하면서 멀어졌지만\n여전히 그를 사랑했음을 고백합니다.",
            "그녀는 앨런이 자신을 초대했을 때\n서로의 관계를 회복하려 한다고 믿고 흔쾌히 수락했다는 사실을 털어놓습니다.",
            "경찰은 사건을 재조사하여 새로운 증거를 통해\n진범이 제니임을 밝혀냅니다.",        // 4
            "제니가 앨런에 대해 오랫동안 품어왔던\n분노와 복수심이 범행의 동기였음을 확인하였습니다.",
            "미나는 사랑했던 사람을 잃고\n그를 죽였다는 의심까지 받으며 깊은 상처를 입었습니다.",      // 5
            "그녀의 마지막 선택은 씁쓸한 여운을 남기며\n사랑과 미련이 만들어낸 비극으로 마무리되었습니다."
            };
        }
    }

    void SetClips()
    {
        allClips = new AudioClip[][] { minaClips, jennyClips, nasonClips };
    }

    /// <summary>
    /// 선택된 NPC의 엔딩 진행
    /// </summary>
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

        if (locale.Identifier.Code == "en")
            StartCoroutine(ShowLine(finalStatement, allFinalStatements[npcIndex][index++ % 3], NPCTransform[npcIndex].parent.GetComponent<EndNPC>(), 0.05f));
        else
            StartCoroutine(ShowLine(finalStatement, allFinalStatements[npcIndex][index++ % 3], NPCTransform[npcIndex].parent.GetComponent<EndNPC>()));
    }

    /// <summary>
    /// O 버튼 클릭 시 엔딩 진행
    /// </summary>
    public void OButtton()
    {
        if (isEndingStarted) return; // 이미 실행 중이라면 return
        isEndingStarted = true; // 실행 시작 플래그 설정

        StartCoroutine(SoundManager.Instance.FadeOutAndChangeClip(SoundManager.Instance.GetEndingBGM()));
        StartCoroutine(OButtonCourutine(GetEnd()));        
    }

    public IEnumerator OButtonCourutine(string[] endStrings)
    {
        int tmpIndex = 0; // 오디오 클립의 인덱스를 추적하는 변수
        int dialogueIndex = 0; // 현재 몇 번째 대사인지 추적

        // 화면을 페이드 인하여 엔딩 화면으로 전환 (2초 동안)
        yield return StartCoroutine(FadeUtility.Instance.FadeIn(screen, 2f));

        _2xClass.FadeIn2xButton();

        // 엔딩 텍스트 배열을 하나씩 출력
        foreach (string str in endStrings)
        {
            finalDialogueText.text = ""; // 기존 텍스트 초기화            

            switch (npcNum)
            {
                case 0: // Mina
                    for (int i = 0; i < minaEndingIndex.Length; i++)
                    {
                        if (minaEndingIndex[i] == dialogueIndex)
                        {
                            if(i != 0)
                            {
                                yield return StartCoroutine(FadeUtility.Instance.FadeOut(NPCEndingImage, 1f));
                                NPCEndingImage.sprite = GetEndingImageSprite(dialogueIndex);
                                yield return StartCoroutine(FadeUtility.Instance.FadeIn(NPCEndingImage, 1f));
                            }
                            else
                            {
                                NPCEndingImage.sprite = GetEndingImageSprite(dialogueIndex);
                                yield return StartCoroutine(FadeUtility.Instance.FadeIn(NPCEndingImage, 1f));
                            }
                        }                            
                    }
                    break;

                case 1: // Jenny
                    for (int i = 0; i < jennyEndingIndex.Length; i++)
                    {
                        if (jennyEndingIndex[i] == dialogueIndex)
                            if (i != 0)
                            {
                                yield return StartCoroutine(FadeUtility.Instance.FadeOut(NPCEndingImage, 1f));
                                NPCEndingImage.sprite = GetEndingImageSprite(dialogueIndex);
                                yield return StartCoroutine(FadeUtility.Instance.FadeIn(NPCEndingImage, 1f));
                            }
                            else
                            {
                                NPCEndingImage.sprite = GetEndingImageSprite(dialogueIndex);
                                yield return StartCoroutine(FadeUtility.Instance.FadeIn(NPCEndingImage, 1f));
                            }

                    }
                    break;

                case 2: // Nason
                    for (int i = 0; i < nasonEndingIndex.Length; i++)
                    {
                        if (nasonEndingIndex[i] == dialogueIndex)
                            if (i != 0)
                            {
                                yield return StartCoroutine(FadeUtility.Instance.FadeOut(NPCEndingImage, 1f));
                                NPCEndingImage.sprite = GetEndingImageSprite(dialogueIndex);
                                yield return StartCoroutine(FadeUtility.Instance.FadeIn(NPCEndingImage, 1f));
                            }
                            else
                            {
                                NPCEndingImage.sprite = GetEndingImageSprite(dialogueIndex);
                                yield return StartCoroutine(FadeUtility.Instance.FadeIn(NPCEndingImage, 1f));
                            }
                    }
                    break;
            }

            // 대사가 "로 시작하는 경우 (NPC가 말하는 부분)
            if (str.StartsWith("\"") || str.StartsWith("「"))
            {
                // 해당 NPC의 음성을 재생하면서 대사 출력 (글자 하나씩 표시)
                if(locale.Identifier.Code == "en")
                    yield return StartCoroutine(PlayNPCSound(finalDialogueText, str, allClips[npcNum][tmpIndex++], 0.05f));
                else
                    yield return StartCoroutine(PlayNPCSound(finalDialogueText, str, allClips[npcNum][tmpIndex++], 0.1f));
            }
            else
            {
                // 일반적인 내레이션 텍스트 출력 (글자 하나씩 표시)
                if (locale.Identifier.Code == "en")
                    yield return StartCoroutine(ShowEnding(finalDialogueText, str, 0.025f));
                else
                    yield return StartCoroutine(ShowEnding(finalDialogueText, str, 0.05f));
            }

            dialogueIndex++;
            yield return new WaitForSeconds(2.5f); // 각 대사 출력 후 2.5초 대기
        }

        finalDialogueText.text = ""; // 마지막 텍스트 초기화
        StartCoroutine(FadeUtility.Instance.FadeOut(NPCEndingImage, 1f));

        // 엔딩 크레딧 출력 코루틴 실행
        StartCoroutine(ShowEndingCredit());
    }

    private string[] GetEnd()
    {
        switch (npcNum)
        {
            case 0: // Mina
                UnlockAchievement("ACH_MINA_END");
                return minaEnd;
            case 1: // Jenny
                UnlockAchievement("ACH_JENNY_END");
                return jennyEnd;
            case 2: // Nason
                UnlockAchievement("ACH_NASON_END");
                return nasonEnd;

            default:
                Debug.LogError("npcIndex 오류!");
                return null;
        }
    }    

    // index에 해당하는 엔딩 Sprite 반환
    private Sprite GetEndingImageSprite(int dialogueIndex)
    {
        switch (npcNum)
        {
            case 0: // Mina
                for (int i = 0; i < minaEndingIndex.Length; i++)
                {
                    if (minaEndingIndex[i] == dialogueIndex)
                        return minaEndingImages[i];
                }
                break;

            case 1: // Jenny
                for (int i = 0; i < jennyEndingIndex.Length; i++)
                {
                    if (jennyEndingIndex[i] == dialogueIndex)
                        return jennyEndingImages[i];
                }
                break;

            case 2: // Nason
                for (int i = 0; i < nasonEndingIndex.Length; i++)
                {
                    if (nasonEndingIndex[i] == dialogueIndex)
                        return nasonEndingImages[i];
                }
                break;
        }
        return null;
    }


    public void XButton()
    {
        isReadyToClick = false;        

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

        StartCoroutine(WaitClick(true, selectNPC, 1f));
    }
    
    IEnumerator WaitClick(bool fadeIn, Graphic target, float duration)
    {        
        if (fadeIn)
        {
            cam.FocusAndReturnToOriginal();
            yield return StartCoroutine(FadeUtility.Instance.FadeIn(target, duration)); // FadeIn 실행
        }
        else
        {            
            yield return StartCoroutine(FadeUtility.Instance.FadeOut(target, duration)); // FadeOut 실행
        }

        isReadyToClick = true;
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

    /// <summary>
    /// 엔딩 크레딧 출력
    /// </summary>
    private IEnumerator ShowEndingCredit()
    {
        _2xClass.Disable2xClass();
        yield return new WaitForSeconds(2f); // 2초 대기
        StartCoroutine(FadeUtility.Instance.FadeIn(endingCredit.GetComponent<Graphic>(), 3f));
        yield return new WaitForSeconds(10f); // 10초 대기                
        StartCoroutine(EndGameAfterDelay());
    }

    private IEnumerator EndGameAfterDelay()
    {
        yield return FadeUtility.Instance.FadeOut(endingCredit, 3f);        
        Application.Quit(); // 빌드된 게임 종료
    }

    /// <summary>
    /// 2배속 버튼 OnClick 등록 이벤트
    /// </summary>
    public void OnClick2x()
    {
        _2xClass.OnClick2xButton();
    }

    private void UnlockAchievement(string achievementId)
    {
        if (!SteamManager.Initialized) return;

        bool alreadyAchieved;
        SteamUserStats.GetAchievement(achievementId, out alreadyAchieved);

        if (!alreadyAchieved)
        {
            SteamUserStats.SetAchievement(achievementId);
            SteamUserStats.StoreStats(); // 저장까지 해야 도전과제 UI 팝업이 뜸
            Debug.Log($"Steam 업적 달성: {achievementId}");
        }
    }
}

/// <summary>
/// 2x 속도를 조절하는 클래스
/// </summary>
class _2x : MonoBehaviour
{
    private bool is2x; // 현재 2x 속도가 활성화되어 있는지 여부
    private float originTimeScale = 1f; // 기본 시간 속도
    private float _2xTimeScale = 1.5f; // 2배속 시 적용할 시간 속도
    private float originAlpha = 0.3f;
    private float maxAlpha = 1f;


    private GameObject _2x_Parent; // 2x 부모 오브젝트
    private GameObject circle; // 2x 모드 활성화 시 애니메이션 오브젝트
    private Button _2x_Button; // 2x 모드를 토글하는 버튼

    /// <summary>
    /// 생성자: 2x 모드의 UI 요소를 초기화
    /// </summary>
    /// <param name="parent">2x 모드 UI의 부모 오브젝트</param>
    public _2x(GameObject parent)
    {
        _2x_Parent = parent;
        circle = parent.transform.GetChild(0).gameObject; // 첫 번째 자식 오브젝트 (원형 UI)
        _2x_Button = parent.transform.GetChild(1).GetComponent<Button>(); // 두 번째 자식 오브젝트 (버튼)

        is2x = false; // 기본적으로 2x 모드는 비활성화 상태
    }

    /// <summary>
    /// 2x 버튼을 페이드 인하여 활성화
    /// </summary>
    public void FadeIn2xButton()
    {
        _2x_Parent.gameObject.SetActive(true);  // 2x UI 부모 오브젝트 활성화
        circle.SetActive(false);                // 원형 UI 비활성화
        _2x_Button.gameObject.SetActive(true);  // 2x 버튼 활성화
    }

    /// <summary>
    /// 2x 버튼 클릭 시 실행 (2x 모드 On/Off)
    /// </summary>
    public void OnClick2xButton()
    {
        is2x = !is2x; // 현재 상태를 반전 (true ↔ false)

        // 시간 속도 변경: 2x 모드 활성화 시 1.75배, 비활성화 시 기본 속도
        Time.timeScale = is2x ? _2xTimeScale : originTimeScale;

        // 원형 UI를 2x 활성 상태에 따라 표시 또는 숨김
        circle.gameObject.SetActive(is2x);

        // 버튼의 알파값을 조절하여 활성화 상태를 반영
        SetButtonAlpha(_2x_Button.GetComponent<Image>(), is2x ? maxAlpha : originAlpha);
    }

    /// <summary>
    /// 버튼의 알파 값을 변경하여 투명도 조절
    /// </summary>
    /// <param name="_2x_Button_Image">알파 값을 변경할 버튼의 이미지</param>
    /// <param name="alpha">설정할 알파 값 (0 ~ 1)</param>
    private void SetButtonAlpha(Image _2x_Button_Image, float alpha)
    {
        Color newColor = _2x_Button_Image.color;
        newColor.a = alpha; // 알파 값 변경
        _2x_Button_Image.color = newColor;
    }

    /// <summary>
    /// 2x 모드를 비활성화하고 원래 속도로 복구
    /// </summary>
    public void Disable2xClass()
    {
        Time.timeScale = originTimeScale; // 시간 속도를 기본 값으로 변경
        is2x = false; // 2x 모드 비활성화
        _2x_Parent.gameObject.SetActive(false); // 2x UI 전체 비활성화
    }
}
