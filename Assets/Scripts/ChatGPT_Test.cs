using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI;
using UnityEngine.Events;

public class ChatGPT_Test : MonoBehaviour
{
    public OnResponseEvent onResponse;    

    [System.Serializable]
    public class OnResponseEvent : UnityEvent<string> { }

    private OpenAIApi openAI = new OpenAIApi();
    private List<ChatMessage> messages = new List<ChatMessage>();

    private void Start()
    {
        // 대화 시작 전에 NPC 역할을 맡도록 시스템 메시지 추가
        ChatMessage systemMessage = new ChatMessage();
        systemMessage.Role = "system";
        systemMessage.Content = "게임 시나리오를 설명해 줄게. " +
            "4명의 친구끼리 파티 중에 파티의 주최자인 '앨런' 이라는 친구가 사망한 사건이 일어나, " +
            "플레이어인 경찰이 다른 3명의 NPC에게 질문을 하여 점점 사건을 파해처 가는 것이 게임 시나리오의 전반적인 설명이야." +
            "플레이어가 증거품을 발견할 때마다 너의 답변의 범위를 업데이트 시킬 거야." +
            "이제 각 NPC에 대해 설명할게." +
            "첫번째 NPC = 이름 : 네이슨, 28세, 남성, 변호사. 침착하고 분석적이며 앨런과 오랜 친구야. 최근 앨런과의 관계가 소원해졌어." +
            "두번째 NPC = 이름  제니, 28세, 여성, 연구원. 조용하고 내성적이지만, 앨런에게 감정적인 상처를 받은 적이 있어." +
            "세번째 NPC = 이름 : 미나, 27세, 여성, 프리랜서 사진작가. 사교적이고 활발하며, 앨런과 가까운 친구였어." +
            "이번 게임의 시작과 함께 너는 네이슨의 역할을 맡을 거야." +
                                          "너는 앨런을 죽인 범인이 아니니 자신의 상황을 인지하고 있어야 해." +
                                          "답변은 30자 이내로 하고, 존댓말을 사용해줘.";

        messages.Add(systemMessage);
    }

    public async void AskChatGPT(string newText)
    {
        ChatMessage newMessage = new ChatMessage();
        newMessage.Content = newText;
        newMessage.Role = "user";

        messages.Add(newMessage);

        CreateChatCompletionRequest request = new CreateChatCompletionRequest();
        request.Messages = messages;
        request.Model = "gpt-3.5-turbo";

        var response = await openAI.CreateChatCompletion(request);

        if (response.Choices != null && response.Choices.Count > 0)
        {
            var chatResponse = response.Choices[0].Message;
            messages.Add(chatResponse);

            Debug.Log(chatResponse.Content);

            onResponse.Invoke(chatResponse.Content);
        }
    }
}
