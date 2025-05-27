using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;
// using static UnityEngine.Rendering.DebugUI;

public class UIManager : MonoBehaviour
{
    // ���������� ���Ǵ� ������
    public static string tmpAnswer = "";
    public string tmpQuestion = "";
    public static bool isAttachedToEvidence = false; // ���ſ��� ��ȣ�ۿ� ����

    [Header("Managers")]
    public ConversationManager conversationManager;
    public EvidenceManager evidenceManager;

    [Header("UI")]
    public Button endConversationBtn;  // ��ȭ ���� ��ư
    public GameObject pulsing;  // Ư�� �̺�Ʈ �� �����̴� UI
    public InputField askField;  // �÷��̾� ���� �Է� �ʵ�
    public Image boundaryUI;
    public Image chatBox;
    public Image screen;
    public Image[] keys;  // ���� �� ��ȣ�ۿ� Ű
    public Text keyDescriptionText;
    public Text thingDescriptionText;
    public Text thingNameText;
    public Timer timer;  // ���� Ÿ�̸�

    [SerializeField] Text NPCName;
    [SerializeField] Text NPCAnswer;
    [SerializeField] GameObject waitingMark;  // NPC�� �߾� �غ� �� ǥ��
    [SerializeField] GameObject clickMark;  // �÷��̾ �Է� ������ �� ǥ��

    UIManagerSup sup; // ���� UI ���� Ŭ����

    [SerializeField] private bool waitToSkip = true;
    public bool isReadyToSkip = false;  // NPC�� ���� ��� ���� �� true�� �����
    public bool IsReadyToSkip
    {
        get { return isReadyToSkip; }
        set { isReadyToSkip = value; }
    }

    [SerializeField] private bool isSkipping = false;
    [SerializeField] private bool isShowingDescription = false; // �ڷ�ƾ ���� ���� Ȯ��
    public bool IsShowingDescription
    {
        get { return isShowingDescription; }
        set { isShowingDescription = value; }
    }

    void Start()
    {
        SetUI();    // UI ��� ��Ȱ��ȭ �� �ʱ� ����
        sup = new UIManagerSup(keys, keyDescriptionText, evidenceManager);
    }

    /// <summary>
    /// Ű �Է� ���� �� ��ȭ ��ŵ ��� Ȱ��ȭ
    /// </summary>
    void OnEnable()
    {
        StartCoroutine(WaitForSkipInput());
    }

    IEnumerator WaitForSkipInput()
    {
        while (true)
        {
            if (isReadyToSkip && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
            {
                isSkipping = true;
            }
            yield return null;
        }
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
        boundaryUI.gameObject.SetActive(false);
        chatBox.gameObject.SetActive(false);
        // cursor.gameObject.SetActive(false);
        endConversationBtn.gameObject.SetActive(false);
        pulsing.gameObject.SetActive(false);

        SetActiveTimer(false);

        // ���� ��Ʈ�� ���� �����ų ���� true�� �����ϱ� - UIManager�� ����
        screen.gameObject.SetActive(true);
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
        return askField.text;
    }

    public InputField GetAskField()
    {
        return askField;
    }

    public string GetQuestion()
    {
        return tmpQuestion;
    }

    //---------------------------------------------------------------//
    //ConversationManager���� ȣ���

    /// <summary>
    /// ��ȭ UI�� Ȱ��ȭ/��Ȱ��ȭ
    /// </summary>
    public void SetConversationUI(bool b)
    {
        SetActiveUIElements(b, askField.gameObject, chatBox.gameObject, endConversationBtn.gameObject);
    }

    /// <summary>
    /// UI ��ҵ��� Ȱ��ȭ/��Ȱ��ȭ�ϴ� ���� �޼���
    /// </summary>
    private void SetActiveUIElements(bool isActive, params GameObject[] elements)
    {
        foreach (GameObject element in elements)
        {
            element.SetActive(isActive);
        }
    }

    /// <summary>
    /// NPC �̸� ����
    /// </summary>
    public void ChangeNPCName(string name)
    {
        NPCName.text = name;
    }

    // NPC �亯 ��ȯ
    public Text GetNPCAnswer()
    {
        return NPCAnswer;
    }

    /// <summary>
    /// NPC�� ���� �� ��Ŀ UI ������Ʈ
    /// </summary>
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

    public void SetActiveEndConversationButton(bool b)
    {
        endConversationBtn.gameObject.SetActive(b);
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
    public void OnEndEditAskField(UnityAction action)
    {
        askField.onEndEdit.AddListener((string text) =>
        {
            if (!string.IsNullOrWhiteSpace(text) && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
            {
                tmpQuestion = text.Trim();
                action();
            }
        });
    }

    public void RemoveOnEndEditListener()
    {
        askField.onEndEdit.RemoveAllListeners();
    }

    public void FocusOnAskField()
    {
        askField.Select();
        askField.ActivateInputField(); // InputField Ȱ��ȭ
    }

    //---------------------------------------------------------------//

    /// <summary>
    /// ��縦 �� ���ھ� ����ϴ� �ڷ�ƾ
    /// </summary>
    public IEnumerator ShowLine(Text t, string answer, int interrogation_pressure, float second = 0.1f, bool isTyping = true)
    {
        t.text = ""; // �ؽ�Ʈ �ʱ�ȭ

        t.color = interrogation_pressure switch
        {
            <= 5 => Color.black,
            <= 8 => Color.yellow,
            _ => Color.red
        };


        Coroutine dialogSoundCoroutine = null;

        SetNPCSpeakingUI(true);
        dialogSoundCoroutine = StartCoroutine(PlayDialogSound());

        yield return new WaitUntil(() => waitToSkip);
        for (int i = 0; i < answer.Length; i++)
        {
            if (isSkipping) // ���콺 ���� Ŭ�� or ����Ű ����
            {
                isSkipping = false;
                waitToSkip = false;
                t.text = answer; // ��ü �ؽ�Ʈ ��� ǥ��
                break; // ������ �ߴ��ϰ� ��ü �ؽ�Ʈ�� ǥ���ϵ��� �̵�
            }

            t.text += answer[i]; // �� ���ھ� �߰�
            yield return new WaitForSeconds(second); // 0.02�� ���
        }

        tmpAnswer = t.text;
        isReadyToSkip = false;
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
        return waitToSkip;
    }

    public void ChangeIsSkipping(bool b)
    {
        waitToSkip = b;
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
        // cursor.gameObject.SetActive(isOn);
    }

    public Image GetScreen()
    {
        return this.screen;
    }

    public void SetActiveTimer(bool whether)
    {
        timer.gameObject.SetActive(whether);
    }

    public void BeginCountdown()
    {
        timer.StartTimer();
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

    public void ActivateBoundaryUI()
    {
        boundaryUI.gameObject.SetActive(true);
    }

    public void UnactivateBoundaryUI()
    {
        boundaryUI.gameObject.SetActive(false);
    }

    public Image GetBoundaryUI()
    {
        return boundaryUI;
    }




    public GameObject GetPulsing()
    {
        return pulsing;
    }

    public void SetActivePulsing(bool isOn)
    {
        pulsing.gameObject.SetActive(isOn);
    }

    // ������Ƽ ����
    public bool IsAttachedToEvidenceProperty
    {
        get => isAttachedToEvidence;
        set
        {
            if (isAttachedToEvidence != value) // ���� ����� ���� ����
            {
                isAttachedToEvidence = value;
                IsAttachedToEvidenceChanged(); // Ư�� �Լ� ȣ��
            }
        }
    }

    private void IsAttachedToEvidenceChanged()
    {
        if (isAttachedToEvidence) SetActivePulsing(true);
        else SetActivePulsing(false);
    }
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

        foreach (var key in keys)
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

            evidenceDescription = evidenceManager.GetEvidenceName(other.GetComponent<Evidence>().GetName());
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
            if (!other.GetComponent<Door>().IsOpened())
            {
                keys[1].gameObject.SetActive(true);
                description.text = actionDescriptions[3];
                description.gameObject.SetActive(true);
            }
        }
    }

    public void HideKeyAndDescriontion()
    {
        foreach (var key in keys)
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