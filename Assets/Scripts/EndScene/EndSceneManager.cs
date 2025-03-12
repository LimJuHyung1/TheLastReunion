using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EndSceneManager : MonoBehaviour
{
    public LayerMask npcLayer;
    public EndCamera cam;
    public Transform[] NPCTransform;        // 3���� NPC

    [Header("UI")]
    public GameObject ox;
    public GameObject mouseDescription;
    public Image screen;
    public Image chatBox;
    public Text finalDialogueText;
    public Text endingCredit;
    public Text finalStatement;
    public Text selectNPC;

    [Header("Audio")]
    public AudioClip[] nasonClips;
    public AudioClip[] jennyClips;
    public AudioClip[] minaClips;

    [Header("2��� ���")]
    public GameObject _2x_Parent;       // 2��� Ŭ����
    private _2x _2xClass;

    private bool isClicked = false;         // npc�� Ŭ���Ǿ��°�
    private bool isReadyToClick = false;    // ���� �� Ŭ�� ���� ����
    private bool isEndingStarted = false; // �ߺ� ���� ������ �÷���
    private int index = 0;                  // ������ ���� �ε���
    private int npcNum = -1;                // ���õ� NPC�� ��ȣ

    // NPC�� ������ ����
    private string[] nasonFinalStatement;
    private string[] jennyFinalStatement;
    private string[] minaFinalStatement;
    private string[][] allFinalStatements;

    // NPC�� ���� ���
    private string[] nasonEnd;
    private string[] jennyEnd;
    private string[] minaEnd;

    private AudioClip[][] allClips;
    private GameObject hoveredObject = null;    // ���콺�� ����Ű�� NPC    

    // Start is called before the first frame update
    void Start()
    {
        screen.gameObject.SetActive(true);
        StartCoroutine(FadeUtility.Instance.FadeOut(screen, 2f));

        selectNPC.gameObject.SetActive(true);
        ox.gameObject.SetActive(false);
        chatBox.gameObject.SetActive(false);
        _2xClass = new _2x(_2x_Parent);

        StartCoroutine(SoundManager.Instance.FadeOutAndChangeClip(SoundManager.Instance.GetSelectCriminalBGM()));

        SetStatements();
        SetEndText();
        SetClips();

        StartCoroutine(WaitClick(false, screen, 2f));
    }

    void Update()
    {
        if (isReadyToClick)
        {
            // ���콺 ��ġ���� ���� ����
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // ���콺�� npcLayer�� �ش��ϴ� ������Ʈ ���� �ִ��� Ȯ��
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, npcLayer))
            {
                GameObject currentHoveredObject = hit.collider.gameObject;

                // Hover ���� ó��
                if (hoveredObject != currentHoveredObject && !isClicked)
                {
                    hoveredObject = currentHoveredObject;
                    mouseDescription.gameObject.SetActive(true);

                    switch (hoveredObject.name)
                    {
                        case "Nason":
                            mouseDescription.transform.GetChild(1).GetComponent<Text>().text = "���̽�";
                            break;
                        case "Jenny":
                            mouseDescription.transform.GetChild(1).GetComponent<Text>().text = "����";
                            break;
                        case "Mina":
                            mouseDescription.transform.GetChild(1).GetComponent<Text>().text = "�̳�";
                            break;
                    }
                }

                // Ŭ������ �� ����
                if (Input.GetMouseButtonDown(0) && !isClicked)
                {
                    int tmpIndex = -1;
                    isClicked = true;

                    switch (hoveredObject.name)
                    {
                        case "Nason":
                            tmpIndex = 2;
                            chatBox.transform.GetChild(1).GetComponent<Text>().text = "���̽�";
                            break;
                        case "Jenny":
                            tmpIndex = 1;
                            chatBox.transform.GetChild(1).GetComponent<Text>().text = "����";
                            break;
                        case "Mina":
                            tmpIndex = 0;
                            chatBox.transform.GetChild(1).GetComponent<Text>().text = "�̳�";
                            break;
                    }

                    mouseDescription.gameObject.SetActive(false);
                    mouseDescription.transform.GetChild(1).GetComponent<Text>().text = "";

                    SelectCriminal(tmpIndex);
                }
            }
            else
            {
                // ���콺�� NPC���� ����� �� hoveredObject �ʱ�ȭ
                hoveredObject = null;
                mouseDescription.gameObject.SetActive(false);
                mouseDescription.transform.GetChild(1).GetComponent<Text>().text = "";
            }
        }
    }

    /// <summary>
    /// ������ ���� ��� ����
    /// </summary>
    void SetStatements()
    {
        nasonFinalStatement = new string[] {
            "�� ����� �ٷ��� ������ �ʾҽ��ϴ�!",
            "�� ���� ���� �ƴ� ���� �ƹ��͵� �����!",
            "���ʿ� ���� �� ������ �ִ� ���� Ȯ���մϱ�?"
        };

        jennyFinalStatement = new string[] {
            "���п� �ڽ��ϴٴ� ���������� ���� �������� ������ ���� �����!",
            "�� ������Ʈ�� �ٷ��� ���� ����Ǿ��� �ص� �װ� ���� ���Ⱑ �� �� �ִٴ� ���ΰ���?",
            "�� �����̿��� �̳��� ������ �ٷ��� ������ ���Ⱑ �־����."
        };

        minaFinalStatement = new string[] {
            "�� �ٷ��� ���� ������ �ƴϿ���!",
            "�ٷ��� ���� ������ �������ٸ� �ݵ�� �����ϰھ��!",
            "���������� ������ �Ǵ��� �ٶ�����."
        };

        allFinalStatements = new string[][] { minaFinalStatement, jennyFinalStatement, nasonFinalStatement };
    }

    /// <summary>
    /// �ḻ ���� �ؽ�Ʈ ����
    /// </summary>
    void SetEndText()
    {
        nasonEnd = new string[] {
            "���̽��� �������� ��������,\n���̽��� ���� ���� ä �Ѽ��� �����ϴ�.",
            "�״� ���� ���� ħ���ϴٰ�\n������ ��Ҹ��� ���մϴ�.",
            "\"������ ���� �����̶�� �����Ͻʴϱ�?\"",
            "\"�ٷ��� ���� ģ���μ� ��� �־���\n���� �������� ���� ȭ�� �� ���� ���ҽ��ϴ�.\"",
            "\"������, �ٷ��� ���� ������ �����ϴ�!\"",
            "\"���� �׸� ����� ���� ���Դϴ�.\"",
            "���̽��� ���� �������� ���� �� ������",
            "�ڽ��� ģ���� �׿��ٰ�\n���ع��� �Ϳ� ���� ��ó�� ���� ��� �ֽ��ϴ�.",
            "���� �������� �״� �ٷ��� ���� ���������\n��� ������ �ٷ��� ����� ������ ���� �浹�ϰ� �Ǿ����� �����մϴ�.",
            "����� ����� ����,\n������ ���Ӱ� �߰ߵ� ���Ÿ� ���� ���ϰ� �������� �������ϴ�.",
            "���ϰ� �ٷ��� ���� �������� ǰ��Դ�\n�г�� �������� ������ ���⿴���� Ȯ���Ͽ����ϴ�.",
            "������ �ٷ��� ȸ���� �ڱ��� �̿��Ͽ�\n���⼺ ���ڸ� �� ��� ���� ��������",
            "�̿� ���̽��� ������ ��ǵ� ���������ϴ�.",
            "���̽��� ���� å���� ���� �� ���� �Ǹ�",
            "���� ���ǿ����� ������� ȸ�� ������\n�ڱ� ���뿡 ���� å���� ���� ������ ���� �˴ϴ�.",
            "���̽��� ģ���� ���� ����\n�߸��� ������ ��� ���� �������� �� ���Դϴ�."
        };

        jennyEnd = new string[] {
            "���ϸ� �������� �������� �󱼿�\n������ �̼Ҹ� ��� õõ�� ���� ��ϴ�.",
            "\"�׷���, ���� �߽��ϴ�.\"",
            "\"�ٷ��� ���� ���� ��ó�� �ʹ��� �����\n�׸� �׳� �ΰ� �� ���� �������ϴ�.\"",
            "������ ��Ҹ����� �� ���� ����\n�ﴭ�� �� �г�� ��ó�� ��� ������",
            "������ ���� ���� ���� ��鸮�� �ֽ��ϴ�.",
            "���ϴ� ���� �������� �ٷ����� ������\n����� ���ô��� ������ ����մϴ�.",
            "�׳�� �׻� �ٷ����� ��ó���ٴ� ������ ���ο��߰�",
            "�ٷ��� �ڽ��� �ž� ���� ������Ʈ�� �����Ű��\n������ ������� ������� �� ��� �γ����� �Ѱ迡 �ٴٶ��ٰ� ���մϴ�.",
            "�׳�� �̹� ������ �ٷ�����\n������ �� �ִ� ��ȣ�� ��ȸ��� �����߰�",
            "�׸� ������ ��ȹ�� �����ٰ� ����մϴ�.",
            "�׳�� �ٷ����� ������ ������ ������� ��\n������ ������ �������� ���ø���",
            "�ٷ��� �ڽſ��� ���� ��ó��ŭ\n�׿��Ե� ������ �ȱ�� �;��ٰ� ���մϴ�.",
            "���ϴ� ��� �˸� �����ϰ� ü���˴ϴ�.",
            "�׳�� �� ū �����԰� ��ǰ��� ������\n�ٷ��� �׸��ڿ��� ����� ���ϴ� �ڽ��� �߰��մϴ�",
            "�г�� ������ ������\n���ϴ� ���� �ḻ�� �����߽��ϴ�.",            
        };

        minaEnd = new string[] {
            "�̳��� �������� ��������, ��ݿ� �������\n�ƹ� �� ���� ��� ���� ���� ���� ���� �����ϴ�.",
            "�׳�� õõ�� ���� ���\nħ���� ǥ������ ������ ���� ���ϴ�.",
            "\"�ٷ��� ����߽��ϴ�...\"",
            "\"������ ���� ������ �ٶ� ���� �����ϴ�.\"",
            "\"�׿��� ���� �ִ� �� ������ ������ ����� �̷��̾����.\"",
            "\"�װ� ���� ����, �׸����� �Ҿ���� ������.\"",
            "�ٷ��� ���� ���� �����̾��ٴ� ��,",
            "�װ� ����� �����ϸ鼭 �־�������\n������ �׸� ��������� ����մϴ�.",
            "�׳�� �ٷ��� �ڽ��� �ʴ����� ��\n������ ���踦 ȸ���Ϸ� �Ѵٰ� �ϰ� ������ �����ߴٴ� ����� �о�����ϴ�.",
            "������ ����� �������Ͽ� ���ο� ���Ÿ� ����\n������ �������� �������ϴ�.",
            "���ϰ� �ٷ��� ���� �������� ǰ��Դ�\n�г�� �������� ������ ���⿴���� Ȯ���Ͽ����ϴ�.",
            "�̳��� ����ߴ� ����� �Ұ�\n�׸� �׿��ٴ� �ǽɱ��� ������ ���� ��ó�� �Ծ����ϴ�.",
            "�׳��� ������ ������ ������ ������ �����\n����� �̷��� ���� ������� �������Ǿ����ϴ�."
        };
    }

    void SetClips()
    {
        allClips = new AudioClip[][] { minaClips, jennyClips, nasonClips };
    }

    /// <summary>
    /// ���õ� NPC�� ���� ����
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

        StartCoroutine(ShowLine(finalStatement, allFinalStatements[npcIndex][index++ % 3], NPCTransform[npcIndex].parent.GetComponent<EndNPC>()));
    }

    /// <summary>
    /// O ��ư Ŭ�� �� ���� ����
    /// </summary>
    public void OButtton()
    {
        if (isEndingStarted) return; // �̹� ���� ���̶�� return
        isEndingStarted = true; // ���� ���� �÷��� ����

        StartCoroutine(SoundManager.Instance.FadeOutAndChangeClip(SoundManager.Instance.GetEndingBGM()));
        StartCoroutine(OButtonCourutine(GetEnd()));        
    }

    public IEnumerator OButtonCourutine(string[] endStrings)
    {
        int tmpIndex = 0; // ����� Ŭ���� �ε����� �����ϴ� ����

        // ȭ���� ���̵� ���Ͽ� ���� ȭ������ ��ȯ (2�� ����)
        yield return StartCoroutine(FadeUtility.Instance.FadeIn(screen, 2f));

        _2xClass.FadeIn2xButton();

        // ���� �ؽ�Ʈ �迭�� �ϳ��� ���
        foreach (string str in endStrings)
        {
            finalDialogueText.text = ""; // ���� �ؽ�Ʈ �ʱ�ȭ

            // ��簡 "�� �����ϴ� ��� (NPC�� ���ϴ� �κ�)
            if (str.StartsWith("\""))
            {
                // �ش� NPC�� ������ ����ϸ鼭 ��� ��� (���� �ϳ��� ǥ��)
                yield return StartCoroutine(PlayNPCSound(finalDialogueText, str, allClips[npcNum][tmpIndex++], 0.1f));
            }
            else
            {
                // �Ϲ����� �����̼� �ؽ�Ʈ ��� (���� �ϳ��� ǥ��)
                yield return StartCoroutine(ShowEnding(finalDialogueText, str, 0.05f));
            }

            yield return new WaitForSeconds(2.5f); // �� ��� ��� �� 2.5�� ���
        }

        finalDialogueText.text = ""; // ������ �ؽ�Ʈ �ʱ�ȭ

        // ���� ũ���� ��� �ڷ�ƾ ����
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
                Debug.LogError("npcIndex ����!");
                return null;
        }
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
            yield return StartCoroutine(FadeUtility.Instance.FadeIn(target, duration)); // FadeIn ����
        }
        else
        {            
            yield return StartCoroutine(FadeUtility.Instance.FadeOut(target, duration)); // FadeOut ����
        }

        isReadyToClick = true;
    }



    public IEnumerator ShowLine(Text t, string answer, EndNPC npc, float second = 0.1f)
    {
        t.text = ""; // �ؽ�Ʈ �ʱ�ȭ
        yield return new WaitForSeconds(1f); // 1�� ���

        npc.PlayEmotion(answer);        

        Coroutine dialogSoundCoroutine = null;

        dialogSoundCoroutine = StartCoroutine(PlayDialogSound());

        for (int i = 0; i < answer.Length; i++)
        {
            t.text += answer[i]; // �� ���ھ� �߰�
            yield return new WaitForSeconds(second); 
        }

        // �ڷ�ƾ�� ����Ǿ��� ��쿡�� ���� ó��
        if (dialogSoundCoroutine != null)
        {
            StopCoroutine(dialogSoundCoroutine); // PlayDialogSound �ڷ�ƾ ����

            ox.gameObject.SetActive(true);
            StartCoroutine(FadeUtility.Instance.FadeIn(ox.transform.GetChild(0).GetComponent<Graphic>(), 1f));
            StartCoroutine(FadeUtility.Instance.FadeIn(ox.transform.GetChild(1).GetComponent<Graphic>(), 1f));

            SoundManager.Instance.StopTextSound();
        }
    }

    public IEnumerator PlayNPCSound(Text t, string answer, AudioClip npcSound, float second = 0.1f)
    {
        t.text = ""; // �ؽ�Ʈ �ʱ�ȭ
        yield return new WaitForSeconds(1f); // 1�� ���

        SoundManager.Instance.ChangeTextAudioClip(npcSound);
        SoundManager.Instance.PlayTextSound();

        for (int i = 0; i < answer.Length; i++)
        {
            t.text += answer[i]; // �� ���ھ� �߰�
            yield return new WaitForSeconds(second);
        }

        // SoundManager.Instance.StopTextSound();
    }

    public IEnumerator ShowEnding(Text t, string answer, float second = 0.1f)
    {
        t.text = ""; // �ؽ�Ʈ �ʱ�ȭ
        yield return new WaitForSeconds(1f); // 1�� ���

        Coroutine dialogSoundCoroutine = null;

        SoundManager.Instance.SetTypingClip();
        dialogSoundCoroutine = StartCoroutine(PlayDialogSound());

        for (int i = 0; i < answer.Length; i++)
        {
            t.text += answer[i]; // �� ���ھ� �߰�
            yield return new WaitForSeconds(second);
        }

        // �ڷ�ƾ�� ����Ǿ��� ��쿡�� ���� ó��
        if (dialogSoundCoroutine != null)
        {
            StopCoroutine(dialogSoundCoroutine); // PlayDialogSound �ڷ�ƾ ����

            ox.gameObject.SetActive(true);
            StartCoroutine(FadeUtility.Instance.FadeIn(ox.transform.GetChild(0).GetComponent<Graphic>(), 1f));
            StartCoroutine(FadeUtility.Instance.FadeIn(ox.transform.GetChild(1).GetComponent<Graphic>(), 1f));

            SoundManager.Instance.StopTextSound();
        }
    }

    IEnumerator PlayDialogSound()
    {
        while (true) // ���� ������ ����Ͽ� �ݺ� ����
        {
            SoundManager.Instance.PlayTextSound();
            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// ���� ũ���� ���
    /// </summary>
    private IEnumerator ShowEndingCredit()
    {
        _2xClass.Disable2xClass();
        yield return new WaitForSeconds(2f); // 2�� ���
        StartCoroutine(FadeUtility.Instance.FadeIn(endingCredit.GetComponent<Graphic>(), 3f));
        yield return new WaitForSeconds(10f); // 10�� ���                
        StartCoroutine(EndGameAfterDelay());
    }

    private IEnumerator EndGameAfterDelay()
    {
        yield return FadeUtility.Instance.FadeOut(endingCredit, 3f);        
        Application.Quit(); // ����� ���� ����
    }

    /// <summary>
    /// 2��� ��ư OnClick ��� �̺�Ʈ
    /// </summary>
    public void OnClick2x()
    {
        _2xClass.OnClick2xButton();
    }
}

/// <summary>
/// 2x �ӵ��� �����ϴ� Ŭ����
/// </summary>
class _2x : MonoBehaviour
{
    private bool is2x; // ���� 2x �ӵ��� Ȱ��ȭ�Ǿ� �ִ��� ����
    private float originTimeScale = 1f; // �⺻ �ð� �ӵ�
    private float _2xTimeScale = 1.75f; // 2��� �� ������ �ð� �ӵ�
    private float originAlpha = 0.3f;
    private float maxAlpha = 1f;


    private GameObject _2x_Parent; // 2x �θ� ������Ʈ
    private GameObject circle; // 2x ��� Ȱ��ȭ �� �ִϸ��̼� ������Ʈ
    private Button _2x_Button; // 2x ��带 ����ϴ� ��ư

    /// <summary>
    /// ������: 2x ����� UI ��Ҹ� �ʱ�ȭ
    /// </summary>
    /// <param name="parent">2x ��� UI�� �θ� ������Ʈ</param>
    public _2x(GameObject parent)
    {
        _2x_Parent = parent;
        circle = parent.transform.GetChild(0).gameObject; // ù ��° �ڽ� ������Ʈ (���� UI)
        _2x_Button = parent.transform.GetChild(1).GetComponent<Button>(); // �� ��° �ڽ� ������Ʈ (��ư)

        is2x = false; // �⺻������ 2x ���� ��Ȱ��ȭ ����
    }

    /// <summary>
    /// 2x ��ư�� ���̵� ���Ͽ� Ȱ��ȭ
    /// </summary>
    public void FadeIn2xButton()
    {
        _2x_Parent.gameObject.SetActive(true);  // 2x UI �θ� ������Ʈ Ȱ��ȭ
        circle.SetActive(false);                // ���� UI ��Ȱ��ȭ
        _2x_Button.gameObject.SetActive(true);  // 2x ��ư Ȱ��ȭ
    }

    /// <summary>
    /// 2x ��ư Ŭ�� �� ���� (2x ��� On/Off)
    /// </summary>
    public void OnClick2xButton()
    {
        is2x = !is2x; // ���� ���¸� ���� (true �� false)

        // �ð� �ӵ� ����: 2x ��� Ȱ��ȭ �� 1.75��, ��Ȱ��ȭ �� �⺻ �ӵ�
        Time.timeScale = is2x ? _2xTimeScale : originTimeScale;

        // ���� UI�� 2x Ȱ�� ���¿� ���� ǥ�� �Ǵ� ����
        circle.gameObject.SetActive(is2x);

        // ��ư�� ���İ��� �����Ͽ� Ȱ��ȭ ���¸� �ݿ�
        SetButtonAlpha(_2x_Button.GetComponent<Image>(), is2x ? maxAlpha : originAlpha);
    }

    /// <summary>
    /// ��ư�� ���� ���� �����Ͽ� ���� ����
    /// </summary>
    /// <param name="_2x_Button_Image">���� ���� ������ ��ư�� �̹���</param>
    /// <param name="alpha">������ ���� �� (0 ~ 1)</param>
    private void SetButtonAlpha(Image _2x_Button_Image, float alpha)
    {
        Color newColor = _2x_Button_Image.color;
        newColor.a = alpha; // ���� �� ����
        _2x_Button_Image.color = newColor;
    }

    /// <summary>
    /// 2x ��带 ��Ȱ��ȭ�ϰ� ���� �ӵ��� ����
    /// </summary>
    public void Disable2xClass()
    {
        Time.timeScale = originTimeScale; // �ð� �ӵ��� �⺻ ������ ����
        is2x = false; // 2x ��� ��Ȱ��ȭ
        _2x_Parent.gameObject.SetActive(false); // 2x UI ��ü ��Ȱ��ȭ
    }
}
