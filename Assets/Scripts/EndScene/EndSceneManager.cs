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
    public Transform[] NPCTransform;        // 3���� NPC


    private bool isClicked = false;     // npc�� Ŭ���Ǿ��°�
    private int index = 0;              // ������ ���� �ε���
    private int npcNum = -1;

    private float moveDistance = Screen.height * 1.6f;
    private float duration = 15f; // �̵��ϴ� �� �ɸ��� �ð�

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

            // str�� \"�� �����ϴ��� Ȯ��
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
                Debug.LogError("npcIndex ����!");
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

        rectTransform.localPosition = endPosition; // ��Ȯ�� �� ��ġ�� ����

        StartCoroutine(EndGameAfterDelay());
    }

    private IEnumerator EndGameAfterDelay()
    {
        FadeUtility.Instance.FadeOut(endingCredit, 2f);
        yield return new WaitForSeconds(10f); // 10�� ���                
        Application.Quit(); // ����� ���� ����
    }
}
