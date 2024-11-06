using OpenAI;
using System.Collections.Generic;
using UnityEngine;
using AdvancedPeopleSystem;

public class NPC : MonoBehaviour
{    
    protected string answer;

    private float emotionTime = 5;

    private CharacterCustomization cc;
    private List<string> emotionsList = new List<string>();

    protected ConversationManager cm;
    protected UIManager uIManager;       


    protected virtual void Start()
    {
        cm = GameObject.Find("ConversationManager").GetComponent<ConversationManager>();
        uIManager = GameObject.Find("UIManager").GetComponent<UIManager>();

        cc = GetComponent<CharacterCustomization>();

        foreach (var e in cc.Settings.characterAnimationPresets)
        {
            emotionsList.Add(e.name);
        }
    }    


    public void faceAnimation()
    {
        cc.PlayBlendshapeAnimation(emotionsList[5], 5);
    }
    

    //----------------------------------------------------//
    // ���� ����

    public void PlayEmotion(ChatMessage newMessage)
    {
        string emotion = DetermineEmotion(newMessage.Content);
        // SetNPCExpression(emotion);

        emotionTime = Mathf.Clamp(emotion.Length * 0.2f, 2f, 5f); // �ּ� 2��, �ִ� 5��
        cc.PlayBlendshapeAnimation(emotionsList[5], emotionTime);
    }

    /// <summary>
    /// Ű���� ��Ī�� ���� NPC �亯�� �˸��� ������ ���
    /// </summary>
    /// <param ChatGPTAnswer="response"></param>
    /// <returns></returns>
    private string DetermineEmotion(string response)
    {
        string lowerCaseResponse = response.ToLower();

        Dictionary<string, string[]> emotionKeywords = new Dictionary<string, string[]>()
        {
            { "Smile", new string[] { "�ູ", "���", "����", "��̴�", "����" } },
            { "Sadness", new string[] { "������", "����ϴ�", "���", "�����ϴ�", "�����" } },
            { "Surprise", new string[] { "����", "���", "��¦", "���" } },
            { "Thoughtful", new string[] { "�����ϴ�", "����ϴ�", "�����ϴ�", "������ ����", "����" } },
            { "Angry", new string[] { "ȭ����", "ȭ", "�г�", "¥��", "���޴�" } }
        };

        foreach (var emotion in emotionKeywords)
        {
            foreach (var keyword in emotion.Value)
            {
                if (lowerCaseResponse.Contains(keyword))
                {
                    return emotion.Key;
                }
            }
        }

        // �⺻������ return�մϴ�.
        return "Smile";
    }

    /// <summary>
    /// NPC�� ǥ���� ����
    /// </summary>
    /// <param NPCAnswer="emotion"></param>
    private void SetNPCExpression(string emotion)
    {
        int index = emotionsList.IndexOf(emotion);
        if (index != -1)
            cc.PlayBlendshapeAnimation(emotionsList[index], emotionTime);
        else
            cc.PlayBlendshapeAnimation(emotionsList[5], emotionTime);
    }





    private NPCRoleInfo GetNPCRoleByName(string npcName)
    {
        if (JsonManager.npcRoleInfoList != null)
        {
            foreach (NPCRoleInfo info in JsonManager.npcRoleInfoList.npcRoleInfoList)
            {
                string name = info.role.Substring(0, info.role.IndexOf(','));                
                if (name == npcName)
                {
                    Debug.Log("����");
                    return info;
                }
            }
        }

        Debug.LogWarning($"'{npcName}' �� ã�� �� �����ϴ�.");
        return null;
    }

    public string GetRole(string npcName)
    {
        NPCRoleInfo npcRoleInfo = GetNPCRoleByName(npcName);
        return npcRoleInfo?.role; // null�̸� null ��ȯ
    }

    public string GetInstructions(string npcName)
    {
        NPCRoleInfo npcRoleInfo = GetNPCRoleByName(npcName);
        return npcRoleInfo?.instructions; // null�̸� null ��ȯ
    }

    public string GetBackground(string npcName)
    {
        NPCRoleInfo npcRoleInfo = GetNPCRoleByName(npcName);
        return npcRoleInfo?.background; // null�̸� null ��ȯ
    }

    public string GetAlibi(string npcName)
    {
        NPCRoleInfo npcRoleInfo = GetNPCRoleByName(npcName);
        return npcRoleInfo?.alibi; // null�̸� null ��ȯ
    }

    public string GetResponseGuidelines(string npcName)
    {
        NPCRoleInfo npcRoleInfo = GetNPCRoleByName(npcName);
        return npcRoleInfo?.responseGuidelines; // null�̸� null ��ȯ
    }
}
