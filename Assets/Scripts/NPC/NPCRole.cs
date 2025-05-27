using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using OpenAI;
using UnityEngine;
using UnityEngine.XR;

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
    private Player player;
    private OpenAIApi openAI;

    /*
    private void Awake()
    {
        // OpenAI API�� �����ϱ� ���� API Ű�� �ε�Ǿ����� Ȯ��
        string apiKey = APIKeyManager.LoadDecryptedAPIKey();
        if (string.IsNullOrEmpty(apiKey))
        {
            Debug.LogError("API Ű�� �������� �ʾҽ��ϴ�! ������ �����ϱ� ���� �����ϼ���.");
            return;
        }
    }
    */

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        anim = GetComponent<Animator>();
        // anim.SetBool("walk", false);

        openAI = new OpenAIApi(); // NPC���� �������� �ν��Ͻ� ����
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

    // �� NPC ���� �н�
    public void SetRole()
    {
        string npcName = currentCharacter.ToString();
        chatMessages = new List<ChatMessage>(); // �� NPC���� �� ����Ʈ ����
        ChatMessage systemMessage = new ChatMessage
        {
            Role = "system",
            Content = GetRole(npcName) 
            + GetAudience(npcName) 
            + GetInformation(npcName) 
            + GetTask(npcName)
            + GetRule(npcName)
            + GetStyle(npcName)          
            + GetConstraint(npcName)
            + GetFormat(npcName)
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
            Content = uIManager.GetAskFieldText(),      // ���� �Է�
            Role = "user"
        };

        chatMessages.Add(newMessage);

        CreateChatCompletionRequest request = new CreateChatCompletionRequest
        {
            Messages = chatMessages,
            Model = "gpt-4o-mini"                 
            // Model = "gpt-3.5-turbo"                 // chatGPT 3.5 ���� ���
        };

        var response = await openAI.CreateChatCompletion(request);

        if (response.Choices != null && response.Choices.Count > 0)
        {
            var chatResponse = response.Choices[0].Message; // ������ ���� ���� ��ü ����
            
            chatMessages.Add(chatResponse); // �޼��� ����Ʈ�� �߰�

            answer = chatResponse.Content;  // ������ string���� ��ȯ
            Debug.Log(answer);

            string emotion = "";

            try
            {
                var parsed = Newtonsoft.Json.Linq.JObject.Parse(answer);
                emotion = parsed["emotion"]?.ToString()?.Trim();

                if (emotion == "Neutral")
                {
                    int randomValue = Random.Range(0, 3); // 0, 1, 2 �� �ϳ�
                    anim.SetInteger("NeutralValue", randomValue);
                }
                    
                anim.SetTrigger(emotion);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"JSON �Ľ� ����: {ex.Message}");
            }

            cm.ShowAnswer(answer);  // ȭ�鿡 ���
        }
    }

    public void AddMessage(ChatMessage chatMessage)
    {
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
}

