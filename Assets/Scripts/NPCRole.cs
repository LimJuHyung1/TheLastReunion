using System.Collections.Generic;
using OpenAI;
using UnityEngine;

public class NPCRole : NPC
{
    public enum Character
    {
        Nason,
        Jenny,
        Mina
    }

    public Character currentCharacter;

    private Animator anim;
    private Player player;

    private OpenAIApi openAI = new OpenAIApi();
    private List<ChatMessage> messages = new List<ChatMessage>();

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        anim.SetBool("walk", false);

        SetRole();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            player = other.GetComponent<Player>();
            player.GetNPCRole(this.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            player.SetNullNPCRole();
            player = null;
        }
    }



   

    void SetRole()
    {
        ChatMessage systemMessage = new ChatMessage();
        systemMessage.Role = "system";
        
        systemMessage.Content = GetCommonRoleDescription(name) + GetSpecificRoleDescription(name);
        messages.Add(systemMessage);
    }


    public void AddMessage(ChatMessage message)
    {
        messages.Add(message);
    }

    // 답변 출력
    public async void GetResponse()
    {
        if (uIManager.GetAskFieldTextLength() < 1)
        {
            return;
        }

        ChatMessage newMessage = new ChatMessage
        {
            Content = uIManager.GetAskFieldText(),
            Role = "user"
        };

        messages.Add(newMessage);

        CreateChatCompletionRequest request = new CreateChatCompletionRequest
        {
            Messages = messages,
            Model = "gpt-3.5-turbo"
        };

        var response = await openAI.CreateChatCompletion(request);

        if (response.Choices != null && response.Choices.Count > 0)
        {
            var chatResponse = response.Choices[0].Message;
            messages.Add(chatResponse);

            answer = chatResponse.Content;
            // onResponse.Invoke(chatResponse.Content);

            cm.ShowAnswer(chatResponse.Content);
        }
    }

    //--------------------------------------------------------//

    /// <summary>
    /// 플레이어 방향으로 rotate
    /// </summary>
    public void TurnTowardPlayer(Transform playerTrans)
    {
        // 현재 오브젝트의 위치
        Vector3 targetPosition = playerTrans.position;

        // 현재 오브젝트의 y 위치는 변경하지 않음
        targetPosition.y = transform.position.y;

        // 타겟 위치를 바라보도록 회전
        transform.LookAt(targetPosition);
    }
}

