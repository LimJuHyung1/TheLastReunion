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
        // OpenAI API를 생성하기 전에 API 키가 로드되었는지 확인
        string apiKey = APIKeyManager.LoadDecryptedAPIKey();
        if (string.IsNullOrEmpty(apiKey))
        {
            Debug.LogError("API 키가 설정되지 않았습니다! 게임을 시작하기 전에 설정하세요.");
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

        openAI = new OpenAIApi(); // NPC마다 독립적인 인스턴스 생성
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

    // 각 NPC 역할 학습
    public void SetRole()
    {
        string npcName = currentCharacter.ToString();
        chatMessages = new List<ChatMessage>(); // 각 NPC마다 새 리스트 생성
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

    // 답변 출력    
    public async void GetResponse()
    {
        if (uIManager.GetAskFieldTextLength() < 1)
        {
            return;
        }        

        ChatMessage newMessage = new ChatMessage
        {
            Content = uIManager.GetAskFieldText(),      // 질문 입력
            Role = "user"
        };

        chatMessages.Add(newMessage);

        CreateChatCompletionRequest request = new CreateChatCompletionRequest
        {
            Messages = chatMessages,
            Model = "gpt-4o-mini"                 
            // Model = "gpt-3.5-turbo"                 // chatGPT 3.5 버전 사용
        };

        var response = await openAI.CreateChatCompletion(request);

        if (response.Choices != null && response.Choices.Count > 0)
        {
            var chatResponse = response.Choices[0].Message; // 질문에 대한 응답 객체 받음
            
            chatMessages.Add(chatResponse); // 메세지 리스트에 추가

            answer = chatResponse.Content;  // 응답을 string으로 변환
            Debug.Log(answer);

            string emotion = "";

            try
            {
                var parsed = Newtonsoft.Json.Linq.JObject.Parse(answer);
                emotion = parsed["emotion"]?.ToString()?.Trim();

                if (emotion == "Neutral")
                {
                    int randomValue = Random.Range(0, 3); // 0, 1, 2 중 하나
                    anim.SetInteger("NeutralValue", randomValue);
                }
                    
                anim.SetTrigger(emotion);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"JSON 파싱 오류: {ex.Message}");
            }

            cm.ShowAnswer(answer);  // 화면에 출력
        }
    }

    public void AddMessage(ChatMessage chatMessage)
    {
        chatMessages.Add(chatMessage);
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

