using System.Collections.Generic;
using System.Threading.Tasks;
using OpenAI;
using UnityEngine;

public class NPCRole : NPC
{    
    public Character currentCharacter;

    private Animator anim;
    private Player player;

    private OpenAIApi openAI = new OpenAIApi();

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
        // �ý��� �޽��� ����
        ChatMessage systemMessage = new ChatMessage
        {
            Role = "system",
            Content = GetCommonRoleDescription(name) + GetSpecificRoleDescription(name)
        };

        messages[currentCharacter].Add(systemMessage);
    }



    // �亯 ���
    /*
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

        messages[currentCharacter].Add(newMessage);

        CreateChatCompletionRequest request = new CreateChatCompletionRequest
        {
            Messages = messages[currentCharacter],
            Model = "gpt-3.5-turbo"
        };

        var response = await openAI.CreateChatCompletion(request);

        if (response.Choices != null && response.Choices.Count > 0)
        {
            var chatResponse = response.Choices[0].Message;
            messages[currentCharacter].Add(newMessage);

            answer = chatResponse.Content;
            // onResponse.Invoke(chatResponse.Content);

            cm.ShowAnswer(chatResponse.Content);
        }
    }
    */

    public async void GetResponse()
    {
        // �ش� ĳ������ ��ȭ �����丮�� ����� �޽��� �߰�
        ChatMessage newMessage = new ChatMessage
        {
            Content = uIManager.GetAskFieldText(),
            Role = "user"
        };
        messages[currentCharacter].Add(newMessage);

        // API ȣ�� �غ�
        CreateChatCompletionRequest request = new CreateChatCompletionRequest
        {
            Messages = messages[currentCharacter],  // �ش� ĳ������ �޽��� ����Ʈ ���
            Model = "gpt-3.5-turbo"
        };

        var response = await openAI.CreateChatCompletion(request);

        // ��� ó��
        if (response.Choices != null && response.Choices.Count > 0)
        {
            var chatResponse = response.Choices[0].Message;
            messages[currentCharacter].Add(chatResponse);  // ĳ���ͺ� �����丮�� ���� �߰�
            Debug.Log($"{currentCharacter}�� ����: {chatResponse.Content}");

            cm.ShowAnswer(chatResponse.Content);
        }
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
}

