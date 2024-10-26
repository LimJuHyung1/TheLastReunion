using System.Collections.Generic;
using System.Threading.Tasks;
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
    private List<ChatMessage> chatMessages;
    private List<ChatMessage> nasonMessages = new List<ChatMessage>();
    private List<ChatMessage> jennyMessages = new List<ChatMessage>();
    private List<ChatMessage> minaMessages = new List<ChatMessage>();
    private Player player;
    private OpenAIApi openAI = new OpenAIApi();    

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        anim.SetBool("walk", false);

        if(chatMessages != null)
        {
            switch (currentCharacter)
            {
                case Character.Nason:
                    chatMessages = nasonMessages;
                    break;
                case Character.Jenny:
                    chatMessages = jennyMessages;
                    break;
                case Character.Mina:
                    chatMessages = minaMessages;
                    break;

                default:
                    Debug.LogError("�޼��� �Ҵ� ���� �߻�!");
                    break;
            }
        }
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
        // �ý��� �޽��� ����
        ChatMessage systemMessage = new ChatMessage
        {
            Role = "system",            
            // Content = GetCommonRoleDescription(name) + GetSpecificRoleDescription(name)
            Content = GetCommonRoleDescription(name)
        };

        chatMessages.Add(systemMessage);
    }

    // �亯 ���    
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

        CreateChatCompletionRequest request = new CreateChatCompletionRequest
        {
            Messages = chatMessages,
            Model = "gpt-3.5-turbo"
        };

        var response = await openAI.CreateChatCompletion(request);

        if (response.Choices != null && response.Choices.Count > 0)
        {
            var chatResponse = response.Choices[0].Message;
            Debug.Log(chatResponse.Content);
            chatMessages.Add(newMessage);

            answer = chatResponse.Content;

            cm.ShowAnswer(chatResponse.Content);
        }
    }

    public void AddMessage(ChatMessage chatMessage)
    {
        switch (currentCharacter)
        {
            case Character.Nason:
                chatMessages = nasonMessages; break;
            case Character.Jenny:
                chatMessages = jennyMessages; break;
            case Character.Mina:
                chatMessages = minaMessages; break;

            default:
                Debug.LogError("�޼��� �Ҵ� ���� �߻�!");
                break;
        }

        chatMessages.Add(chatMessage);
    }


    //--------------------------------------------------------//

    /// <summary>
    /// �÷��̾� �������� rotate
    /// </summary>
    public void TurnTowardPlayer(Transform playerTrans)
    {
        // ���� ������Ʈ�� ��ġ
        Vector3 targetPosition = playerTrans.position;

        // ���� ������Ʈ�� y ��ġ�� �������� ����
        targetPosition.y = transform.position.y;

        // Ÿ�� ��ġ�� �ٶ󺸵��� ȸ��
        transform.LookAt(targetPosition);
    }

    public void ShowMessages()
    {
        foreach (var message in chatMessages)
        {
            Debug.Log(message.Content);
        }
    }
}

