using OpenAI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using AdvancedPeopleSystem;

public class NPC : MonoBehaviour
{    
    public static string answer;

    // private bool isRecording = false;
    private float emotionTime = 5;

    // private AudioClip recording;
    private CharacterCustomization cc;
    private List<string> emotionsList = new List<string>();

    protected ConversationManager cm;
    protected UIManager uIManager;    


    // 내부 클래스
    [System.Serializable]
    public class OnResponseEvent : UnityEvent<string> { }


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
            foreach (NPCRoleInfo tmp in JsonManager.npcRoleInfoList.npcRoleInfoList)
            {
                if (tmp.npcName == npcName)
                {
                    return tmp;
                }
            }
        }

        Debug.LogWarning($"'{npcName}' 를 찾을 수 없습니다.");
        return null;
    }

    public string GetNPCName(string npcName)
    {
        NPCRoleInfo npcRoleInfo = GetNPCRoleByName(npcName);
        return npcRoleInfo?.npcName; // null이면 null 반환
    }

    public string GetCommonRoleDescription(string npcName)
    {
        NPCRoleInfo npcRoleInfo = GetNPCRoleByName(npcName);
        return npcRoleInfo?.commonRoleDescription; // null이면 null 반환
    }

    public string GetSpecificRoleDescription(string npcName)
    {
        NPCRoleInfo npcRoleInfo = GetNPCRoleByName(npcName);
        return npcRoleInfo?.specificRoleDescription; // null이면 null 반환
    }
}
