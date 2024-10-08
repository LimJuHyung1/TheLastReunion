using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static string tmpAnswer;
    public string tmpQuestion;

    public Button endConversationBtn;

    public ConversationManager conversationManager;
    public EvidenceManager evidenceManager;

    public InputField askField;
    public Image chatBox;
    public Image cursor;
    public Image screen;
    public Image[] keys;
    public Text answerText;
    public Text keyDescriptionText;
    public Text thingDescriptionText;
    public Text thingNameText;

    [SerializeField] Text NPCName;
    [SerializeField] Text NPCAnswer;
    [SerializeField] GameObject waitingMark;
    [SerializeField] GameObject clickMark;
    UIManagerSup sup;

    [SerializeField]
    private bool isAbleToSkip = true;
    [SerializeField]
    private bool isShowingDescription = false; // �ڷ�ƾ ���� ���θ� Ȯ���ϴ� ����

    // Start is called before the first frame update
    void Start()
    {
        SetUI();

        sup = new UIManagerSup(keys, keyDescriptionText, evidenceManager);
    }

    /// <summary>
    /// �ʱ� UI ����
    /// </summary>
    void SetUI()
    {
        NPCName = chatBox.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>();
        NPCAnswer = chatBox.transform.GetChild(2).GetComponent<UnityEngine.UI.Text>();
        waitingMark = chatBox.transform.GetChild(3).gameObject;
        clickMark = chatBox.transform.GetChild(4).gameObject;

        askField.gameObject.SetActive(false);
        answerText.gameObject.SetActive(false);
        chatBox.gameObject.SetActive(false);
        cursor.gameObject.SetActive(false);
        endConversationBtn.gameObject.SetActive(false);

        // ���� ��Ʈ�� ���� �����ų ���� true�� �����ϱ� - UIManager�� ����
        screen.gameObject.SetActive(true);

        // screen.gameObject.SetActive(false);
    }    

    //---------------------------------------------------------------//
    // �Ʒ� �Լ����� NPC ��ũ��Ʈ���� ȣ���

    // InputField �ؽ�Ʈ ���� ��ȯ
    public int GetAskFieldTextLength()
    {
        return askField.text.Length;
    }

    // InputField �ؽ�Ʈ ��ȯ
    public string GetAskFieldText()
    {
        tmpQuestion = askField.text;
        return askField.text;
    }

    public InputField GetAskField()
    {
        return askField;
    }

    //---------------------------------------------------------------//
    //ConversationManager���� ȣ���

    /// <summary>
    /// ��ȭ ���� �� UI ����
    /// </summary>
    /// <param �ð�ȭ_����="b"></param>
    public void SetConversationUI(bool b)
    {
        GameObject[] uiElements = {
        askField.gameObject,
        answerText.gameObject,
        chatBox.gameObject,
        endConversationBtn.gameObject
    };

        foreach (GameObject element in uiElements)
        {
            element.SetActive(b);
        }

        cursor.gameObject.SetActive(!b);
        answerText.text = "";
    }

    // ��ȭ �� NPC �̸� ���
    public void ChangeNPCName(string name)
    {
        NPCName.text = name;
    }

    // NPC �亯 ��ȯ
    public Text GetNPCAnswer()
    {
        return NPCAnswer;
    }

    // ��ȭ �� ��ũ ǥ��(?)
    public void SetSpeakingUI()
    {
        waitingMark.gameObject.SetActive(false);
        clickMark.gameObject.SetActive(false);
    }    

    /// <summary>
    /// NPC�� �߾� ���¿� ���� UI ����
    /// </summary>
    /// <param name="isSpeaking">NPC�� �߾� ������ ����</param>
    public void SetNPCSpeakingUI(bool isSpeaking)
    {
        waitingMark.gameObject.SetActive(isSpeaking);
        clickMark.gameObject.SetActive(!isSpeaking);
    }

    /// <summary>
    /// input field�� ���� ĭ�� �������� ����
    /// </summary>
    public void SetNullInputField()
    {
        askField.text = "";
    }

    public void SetInteractableAskField(bool b)
    {
        askField.interactable = b;
    }

    /// <summary>
    /// NPC �亯 �ؽ�Ʈ�� �������� ����
    /// </summary>
    public void SetBlankAnswerText()
    {
        NPCAnswer.text = "";
    }

    /// <summary>
    /// NPC ��ȭ �� ���� Ű �����ϰ� ����
    /// </summary>
    /// <param name="action"></param>
    public void onEndEditAskField(UnityAction action)
    {
        askField.onEndEdit.AddListener((string text) => {
            action();
        });
    }    

    public Button GetEndConversationButton()
    {
        return endConversationBtn;
    }

    //---------------------------------------------------------------//

    /// <summary>
    /// ��縦 �� ���ھ� ����ϴ� �ڷ�ƾ
    /// </summary>
    /// <param UI-Text="t"></param>
    /// <param �����_���ڿ�="answer"></param>
    /// <returns></returns>
    public IEnumerator ShowLine(Text t, string answer, float second = 0.1f, bool isTyping = true)
    {
        t.text = ""; // �ؽ�Ʈ �ʱ�ȭ
        Coroutine dialogSoundCoroutine = null;

        SetNPCSpeakingUI(true);
        dialogSoundCoroutine = StartCoroutine(PlayDialogSound());

        yield return new WaitUntil(() => isAbleToSkip);
        for (int i = 0; i < answer.Length; i++)
        {
            if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) && isAbleToSkip) // ���콺 ���� Ŭ�� or ����Ű ����
            {
                isAbleToSkip = false;
                t.text = answer; // ��ü �ؽ�Ʈ ��� ǥ��
                break; // ������ �ߴ��ϰ� ��ü �ؽ�Ʈ�� ǥ���ϵ��� �̵�
            }

            t.text += answer[i]; // �� ���ھ� �߰�
            yield return new WaitForSeconds(second); // 0.02�� ���
        }

        tmpAnswer = t.text;
        ChangeIsSkipping(false);

        // �ڷ�ƾ�� ����Ǿ��� ��쿡�� ���� ó��
        if (dialogSoundCoroutine != null)
        {
            SetNPCSpeakingUI(false);
            StopCoroutine(dialogSoundCoroutine); // PlayDialogSound �ڷ�ƾ ����
            SoundManager.Instance.StopTextSound();
        }

        conversationManager.IsAbleToGoNextTrue();
    }

    /// <summary>
    /// Funiture, Evidence ���� ui�� ǥ��
    /// </summary>
    /// <param ���� �̸�="name"></param>
    /// <param ���� ����="description"></param>
    /// <returns></returns>
    public IEnumerator ShowDescription(string description)
    {
        if (isShowingDescription)
        {
            yield break; // �̹� ���� ���̸� ����
        }

        isShowingDescription = true; // �ڷ�ƾ ���� ��

        thingDescriptionText.text = "";

        foreach (char letter in description)
        {
            thingDescriptionText.text += letter; // �� ���ھ� �߰�
            yield return new WaitForSeconds(0.05f);
        }

        // 2�� �Ŀ� �ؽ�Ʈ ����� ����
        StartCoroutine(ClearDescription(2f));
    }

    IEnumerator ClearDescription(float delay)
    {
        // 2�� ���
        yield return new WaitForSeconds(delay);

        isShowingDescription = false; // �ڷ�ƾ ����

        // �ؽ�Ʈ ���� �����
        thingNameText.text = "";
        thingDescriptionText.text = "";
    }


    public bool GetIsSkipping()
    {
        return isAbleToSkip;
    }

    public void ChangeIsSkipping(bool b)
    {
        isAbleToSkip = b;
    }

    IEnumerator PlayDialogSound()
    {
        while (true) // ���� ������ ����Ͽ� �ݺ� ����
        {
            SoundManager.Instance.PlayTextSound();
            yield return new WaitForSeconds(0.1f);
        }
    }

    //---------------------------------------------//
    //�Ʒ� �Լ����� TutorialManager���� ȣ��

    public void SetActiveCursor(bool isOn)
    {
        cursor.gameObject.SetActive(isOn);
    }

    public Image GetScreen()
    {
        return this.screen;
    }

    //---------------------------------------------//
    // ���� �� ����� Ű ���� �Լ�

    public void ShowKeyAndDescriontion(GameObject other)
    {
        sup.ShowKeyAndDescriontion(other);
    }

    public void HideKeyAndDescriontion()
    {
        sup.HideKeyAndDescriontion();
    }

    //---------------------------------------------//
    
}

public class UIManagerSup
{
    // 0 - ���� ����, 1 - npc ��ȭ, 2 - �� ����
    List<Image> keys = new List<Image>();
    Text description;

    EvidenceManager evidenceManager;

    string[] actionDescriptions = new string[5];
    string evidenceDescription;

    public UIManagerSup(Image[] keys, Text t, EvidenceManager evidenceManager)
    {
        foreach (Image key in keys)
        {
            this.keys.Add(key);
        }
        this.description = t;
        this.evidenceManager = evidenceManager;

        SetKeyDescriptions();
    }

    public void SetKeyDescriptions()
    {
        actionDescriptions[0] = "���̽��� ��ȭ�ϴ�";
        actionDescriptions[1] = "���Ͽ� ��ȭ�ϴ�";
        actionDescriptions[2] = "�̳��� ��ȭ�ϴ�";
        actionDescriptions[3] = "���� ����";        
        actionDescriptions[4] = "���Ÿ� �����ϴ�";

        foreach(var key in keys)
        {
            key.gameObject.SetActive(false);
        }

        description.gameObject.SetActive(false);
    }

    public void ShowKeyAndDescriontion(GameObject other)
    {
        LayerMask layer = other.gameObject.layer;
        if (IsEvidenceLayer(layer))
        {
            keys[0].gameObject.SetActive(true);

            evidenceDescription = evidenceManager.GetEvidenceName(other.GetComponent<Evidence>().Name);
            description.text = evidenceDescription;
            description.gameObject.SetActive(true);
        }
        else if (IsNPCLayer(layer))
        {
            keys[0].gameObject.SetActive(true);
            // NPC�� ���� �˸��� ���� �����ֱ�
            string name = other.GetComponent<NPCRole>().currentCharacter.ToString();
            switch (name)
            {
                case "Nason":
                    description.text = actionDescriptions[0];
                    break;
                case "Jenny":
                    description.text = actionDescriptions[1];
                    break;
                case "Mina":
                    description.text = actionDescriptions[2];
                    break;
            }
            description.gameObject.SetActive(true);
        }
        else if (IsDoorLayer(layer))
        {
            if (!other.GetComponent<Door>().isOpened)
            {
                keys[1].gameObject.SetActive(true);
                description.text = actionDescriptions[3];
                description.gameObject.SetActive(true);
            }
        }
    }

    public void HideKeyAndDescriontion()
    {
        foreach(var key in keys)
        {
            if (key.gameObject.activeSelf)
            {
                key.gameObject.SetActive(false);
                break;
            }
        }

        description.gameObject.SetActive(false);
    }

    bool IsEvidenceLayer(int layer)
    {
        return layer == LayerMask.NameToLayer("Evidence");
    }

    bool IsNPCLayer(int layer)
    {
        return layer == LayerMask.NameToLayer("NPC");
    }

    bool IsDoorLayer(int layer)
    {
        return layer == LayerMask.NameToLayer("Door");
    }
}