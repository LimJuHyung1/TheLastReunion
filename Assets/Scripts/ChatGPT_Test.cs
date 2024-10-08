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
        // ��ȭ ���� ���� NPC ������ �õ��� �ý��� �޽��� �߰�
        ChatMessage systemMessage = new ChatMessage();
        systemMessage.Role = "system";
        systemMessage.Content = "���� �ó������� ������ �ٰ�. " +
            "4���� ģ������ ��Ƽ �߿� ��Ƽ�� �������� '�ٷ�' �̶�� ģ���� ����� ����� �Ͼ, " +
            "�÷��̾��� ������ �ٸ� 3���� NPC���� ������ �Ͽ� ���� ����� ����ó ���� ���� ���� �ó������� �������� �����̾�." +
            "�÷��̾ ����ǰ�� �߰��� ������ ���� �亯�� ������ ������Ʈ ��ų �ž�." +
            "���� �� NPC�� ���� �����Ұ�." +
            "ù��° NPC = �̸� : ���̽�, 28��, ����, ��ȣ��. ħ���ϰ� �м����̸� �ٷ��� ���� ģ����. �ֱ� �ٷ����� ���谡 �ҿ�������." +
            "�ι�° NPC = �̸�  ����, 28��, ����, ������. �����ϰ� ������������, �ٷ����� �������� ��ó�� ���� ���� �־�." +
            "����° NPC = �̸� : �̳�, 27��, ����, �������� �����۰�. �米���̰� Ȱ���ϸ�, �ٷ��� ����� ģ������." +
            "�̹� ������ ���۰� �Բ� �ʴ� ���̽��� ������ ���� �ž�." +
                                          "�ʴ� �ٷ��� ���� ������ �ƴϴ� �ڽ��� ��Ȳ�� �����ϰ� �־�� ��." +
                                          "�亯�� 30�� �̳��� �ϰ�, ������ �������.";

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
