using OpenAI;
using System.Collections.Generic;
using UnityEngine;
using AdvancedPeopleSystem;
using UnityEngine.XR;

public class NPC : MonoBehaviour
{
    private float emotionTime = 5;
    private CharacterCustomization cc;
    private List<string> emotionsList = new List<string>();

    protected string answer;
    protected ConversationManager cm;
    protected NPCRoleInfoManager roleInfoManager;
    protected UIManager uIManager;

    private NPCEmotionHandler emotionHandler;    

    protected virtual void Start()
    {
        cm = GameObject.Find("ConversationManager").GetComponent<ConversationManager>();
        uIManager = GameObject.Find("UIManager").GetComponent<UIManager>();

        cc = GetComponent<CharacterCustomization>();

        // NPCEmotionHandler 초기화
        emotionHandler = new NPCEmotionHandler(cc);        
        roleInfoManager = new NPCRoleInfoManager();

        // 씬 시작 시 색상 초기화
        if (cc != null)
        {
            cc.InitColors();
            Debug.Log(name + "의 색상이 초기화되었습니다, start");
        }
    }

    public void PlayEmotion(ChatMessage newMessage)
    {
        emotionHandler.PlayEmotion(newMessage.Content);
    }

    public string GetRole(string npcName) => roleInfoManager.GetRole(npcName);
    public string GetInstructions(string npcName) => roleInfoManager.GetInstructions(npcName);
    public string GetBackground(string npcName) => roleInfoManager.GetBackground(npcName);
    public string GetFriends(string npcName) => roleInfoManager.GetFriends(npcName);
    public string GetAlibi(string npcName) => roleInfoManager.GetAlibi(npcName);
    public string GetResponseGuidelines(string npcName) => roleInfoManager.GetResponseGuidelines(npcName);

    // 내부 클래스: 감정 표현 처리
    private class NPCEmotionHandler
    {
        private CharacterCustomization cc;
        private List<string> emotionsList = new List<string>();
        private float emotionTime;

        public NPCEmotionHandler(CharacterCustomization characterCustomization)
        {
            cc = characterCustomization;
            InitializeEmotionsList();
        }

        public void InitializeEmotionsList()
        {
            foreach (var e in cc.Settings.characterAnimationPresets)
            {
                emotionsList.Add(e.name);
            }
        }

        public void PlayEmotion(string responseContent)
        {
            emotionTime = Mathf.Clamp(responseContent.Length * 0.2f, 2f, 5f);
            cc.PlayBlendshapeAnimation(emotionsList[5], emotionTime);
        }
    }
}