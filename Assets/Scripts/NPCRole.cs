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
        // 시스템 메시지 생성
        ChatMessage systemMessage = new ChatMessage
        {
            Role = "system",
            Content = GetCommonRoleDescription(name) + GetSpecificRoleDescription(name)
        };

        messages[currentCharacter].Add(systemMessage);
    }



    // 답변 출력
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
        // 해당 캐릭터의 대화 히스토리에 사용자 메시지 추가
        ChatMessage newMessage = new ChatMessage
        {
            Content = uIManager.GetAskFieldText(),
            Role = "user"
        };
        messages[currentCharacter].Add(newMessage);

        // API 호출 준비
        CreateChatCompletionRequest request = new CreateChatCompletionRequest
        {
            Messages = messages[currentCharacter],  // 해당 캐릭터의 메시지 리스트 사용
            Model = "gpt-3.5-turbo"
        };

        var response = await openAI.CreateChatCompletion(request);

        // 결과 처리
        if (response.Choices != null && response.Choices.Count > 0)
        {
            var chatResponse = response.Choices[0].Message;
            messages[currentCharacter].Add(chatResponse);  // 캐릭터별 히스토리에 응답 추가
            Debug.Log($"{currentCharacter}의 응답: {chatResponse.Content}");

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

