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
    // 감정 구현

    public void PlayEmotion(ChatMessage newMessage)
    {
        string emotion = DetermineEmotion(newMessage.Content);
        // SetNPCExpression(emotion);

        emotionTime = Mathf.Clamp(emotion.Length * 0.2f, 2f, 5f); // 최소 2초, 최대 5초
        cc.PlayBlendshapeAnimation(emotionsList[5], emotionTime);
    }

    /// <summary>
    /// 키워드 매칭을 통해 NPC 답변에 알맞은 감정을 출력
    /// </summary>
    /// <param ChatGPTAnswer="response"></param>
    /// <returns></returns>
    private string DetermineEmotion(string response)
    {
        string lowerCaseResponse = response.ToLower();

        Dictionary<string, string[]> emotionKeywords = new Dictionary<string, string[]>()
        {
            { "Smile", new string[] { "행복", "기쁨", "웃다", "즐겁다", "좋다" } },
            { "Sadness", new string[] { "슬프다", "우울하다", "울다", "비통하다", "힘들다" } },
            { "Surprise", new string[] { "놀라다", "충격", "깜짝", "경악" } },
            { "Thoughtful", new string[] { "생각하다", "고민하다", "숙고하다", "생각에 잠기다", "숙고" } },
            { "Angry", new string[] { "화나다", "화", "분노", "짜증", "열받다" } }
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

        // 기본값으로 return합니다.
        return "Smile";
    }

    /// <summary>
    /// NPC의 표정을 설정
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
                    Debug.Log("성공");
                    return info;
                }
            }
        }

        Debug.LogWarning($"'{npcName}' 를 찾을 수 없습니다.");
        return null;
    }

    public string GetRole(string npcName)
    {
        NPCRoleInfo npcRoleInfo = GetNPCRoleByName(npcName);
        return npcRoleInfo?.role; // null이면 null 반환
    }

    public string GetInstructions(string npcName)
    {
        NPCRoleInfo npcRoleInfo = GetNPCRoleByName(npcName);
        return npcRoleInfo?.instructions; // null이면 null 반환
    }

    public string GetBackground(string npcName)
    {
        NPCRoleInfo npcRoleInfo = GetNPCRoleByName(npcName);
        return npcRoleInfo?.background; // null이면 null 반환
    }

    public string GetAlibi(string npcName)
    {
        NPCRoleInfo npcRoleInfo = GetNPCRoleByName(npcName);
        return npcRoleInfo?.alibi; // null이면 null 반환
    }

    public string GetResponseGuidelines(string npcName)
    {
        NPCRoleInfo npcRoleInfo = GetNPCRoleByName(npcName);
        return npcRoleInfo?.responseGuidelines; // null이면 null 반환
    }
}
