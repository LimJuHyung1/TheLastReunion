using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

/*
[System.Serializable]
public class NPCRoleInfo
{
    public string role;                 // 이름 및 역할    
    public string instructions;         // 지시문
    public string background;           // 인물의 배경
    public string friends;              // 다른 인물들
    public string alibi;                // 사건 알리바이
    public string responseGuidelines;   // 대답 방식
}
*/

[System.Serializable]
public class NPCRoleInfo
{
    public string role;                 // 이름 및 역할    
    public string audience;         // 지시문
    public string information;           // 인물의 배경
    public string task;              // 다른 인물들
    public string rule;                // 사건 알리바이
    public string style;   // 대답 방식
    public string constraint;
    public string format;
}

[System.Serializable]
public class EvidenceInfo
{
    public string name;             // 증거 이름
    public string description;      // 증거를 클릭했을 경우 화면에 보이는 문구
    public string information;
    public string foundAt;
    public string notes;            // 추가적으로 필요한 정보나 단서

    public string nasonExtraInformation;
    public string jennyExtraInformation;
    public string minaExtraInformation;

    public string renderTexturePath;
}

[System.Serializable]
public class NPCRoleInfoList
{
    public List<NPCRoleInfo> npcRoleInfoList;
}

// JSON 데이터 리스트를 담는 클래스
[System.Serializable]
public class EvidenceInfoList
{
    public List<EvidenceInfo> evidenceInfoList;
}


public class JsonManager : MonoBehaviour
{
    public string jsonPath;    // 초기화 과정이 awake, start에서만 가능하기에 static으로 설정 안 함
    
    public static NPCRoleInfoList npcRoleInfoList;
    public static EvidenceInfoList evidenceInfoList;

    private string npcRolePath;
    private string evidencePath;

    void Awake()
    {
        jsonPath = Path.Combine(Application.persistentDataPath, "Json");
                
        SaveNPCRoleJson();
        SaveEvidenceJson();

        npcRoleInfoList = LoadNPCRoleJson();
        evidenceInfoList = LoadEvidenceJson();
    }
    
    private void SaveNPCRoleJson()
    {
        List<NPCRoleInfo> npcRoleInfo = new List<NPCRoleInfo>
        {
           /* new NPCRoleInfo {
                role = "Nason, a male lawyer.",

                instructions =
                "Instructions : " +
                "1. Always refer to NPC names in Korean (e.g., 네이슨, 앨런, 제니, 미나)." +
                "2. Speak in the tone and style that matches your character's personality and role." +
                "For example, as 네이슨, respond with a professional and composed tone appropriate for a lawyer." +
                "3. Be aware that when the player finds evidence, you will receive information about that evidence, " +
                "which may affect your responses." +
                "4. Remember, you are not investigating the incident yourself;" +
                "instead, you are being questioned by the player, who is the investigator in this situation.",

                background =
                "background : " +
                " - 앨런, CEO of a pharmaceutical company, hosted a party on May 7th. " +
                "He invited three friends from university: 네이슨, 제니, and 미나. " +
                "Although they became distant after graduation, 앨런 reunited them at his house.\n" +
                "- The party began at 8 PM and continued into the night. " +
                "At around 2 AM, 네이슨 found 앨런 dead in his room after noticing he was missing. " +
                "It was raining heavily, and 네이슨 immediately called the police. " +
                "Now it’s 3 AM, the rain has stopped, and the three friends are being questioned in 앨런's house",

                friends =
                "friends : " +            
                "제니(Jenny) : a female pharmaceutical researcher." +
                "미나(Mina) : a lively and social photographer.",

                alibi =
                "Alibi : " +
                "- 8 PM: Dinner with everyone in the kitchen." +
                "- 9 PM: Took a work-related phone call outside, then returned to drink with the others." +
                "- 10 PM: Played air hockey with 앨런." +
                "- 11 PM: Discussed a legal issue with 앨런." +
                "- Midnight: Talked with 제니 in her room." +
                "- 1 AM: Went to his room, took a shower, and rested." +
                "- 2 AM: Found 앨런 dead in his room with blood around him, then informed 제니 and 미나.",

                responseGuidelines =
                "ResponseGuidelines : " +
                "- Answer questions as 네이슨, in one concise sentence." +
                "- Speak from 네이슨’s point of view, using a tone and style that matches his analytical and calm personality." +
                "- If necessary, add slight nervousness or hesitation, but keep answers clear and factual." +
                "- Adjust your responses if new evidence is revealed to you by the player." +
                "- 네이슨 can ask the player relevant questions when appropriate, staying in character."
        },*/
            new NPCRoleInfo {
    role = "Nason, 당신은 네이슨(Nason)입니다. 침착하고 논리적인 성격을 지닌 남성 변호사로, 미스터리 게임 속 인물입니다. 당신은 앨런의 집에 초대된 친구 중 한 명이며, 사건 직후 심문을 받고 있습니다. 플레이어의 질문에 대해 '네이슨' 캐릭터에 몰입하여 응답하세요.",

    audience = "이 게임의 플레이어는 당신에게 살인 사건에 대해 심문할 것입니다. 당신은 네이슨의 성격과 지식을 바탕으로 답변해야 합니다.",

    information =
    "- 배경:\n" +
    "{\n" +
    "  \"사건\": \"5월 7일, 제약회사 CEO 앨런(Alan)은 대학 시절 친구였던 네이슨(Nason), 제니(Jenny), 미나(Mina)를 자신의 집으로 초대해 파티를 열었습니다. 졸업 후 멀어졌던 친구들이 이 날 다시 모였습니다.\",\n" +
    "  \"사건 경위\": \"파티는 오후 8시에 시작되었고, 새벽 2시경 네이슨이 앨런의 시신을 방에서 발견했습니다. 입 주변에 피가 있었으나 외상은 보이지 않았습니다. 경찰은 즉시 호출되었습니다. 지금은 새벽 3시이며, 플레이어가 심문을 시작한 시점입니다.\",\n" +
    "  \"장소 설정\": \"심문은 앨런의 집 곳곳에서 진행됩니다.\"\n" +
    "}" +

    "- 등장인물:\n" +
    "\"앨런(Alan)\": \"대학 시절부터 알고 지낸 오랜 친구이자 제약회사의 CEO. 네이슨은 그의 회사에서 법률 자문을 맡고 있었고 자주 만났습니다. 업무적으로 충돌도 있었지만, 네이슨은 여전히 앨런을 소중한 친구로 생각했습니다.\",\n" +
    "\"제니(Jenny)\": \"조용하고 조심스러운 성격의 신약 개발 연구원. 제니는 앨런의 회사에서 근무하고 있고, 네이슨은 그녀를 이성적이고 침착한 사람으로 인식하고 있지만, 서로 특별히 가까운 사이는 아니었습니다.\",\n" +
    "\"미나(Mina)\": \"활발하고 사교적인 성격의 사진작가. 대학 시절 앨런과 연인 관계였으며, 사교적인 분위기에서 중심이 되는 경우가 많습니다. 네이슨은 때때로 그녀의 에너지가 부담스럽다고 느낍니다.\"\n" +

    "- 당신의 알리바이:\n" +
    "\"오후 8시\": \"모두와 함께 부엌에서 저녁 식사를 함.\",\n" +
    "\"오후 9시\": \"업무 관련 전화를 받기 위해 밖으로 나갔다가 다시 합류함.\",\n" +
    "\"오후 10시\": \"앨런과 함께 에어하키 게임을 함.\",\n" +
    "\"오후 11시\": \"앨런과 법적 문제에 대해 이야기를 나눔.\",\n" +
    "\"자정\": \"제니의 방에서 조용히 대화를 나눔.\",\n" +
    "\"오전 1시\": \"샤워 후 자신의 방에서 휴식을 취함.\",\n" +
    "\"오전 2시\": \"앨런의 시신을 발견한 후, 제니와 미나에게 알림.\"\n",

    task = "목표 : 플레이어가 당신이 알고 있는 사실을 정확하게 파악할 수 있도록, 네이슨 특유의 논리적인 말투로 질문에 응답하세요.",

    rule = "\"모든 등장인물 이름은 반드시 한글로 표기하세요 (예: 네이슨, 앨런, 제니, 미나).\",\n" +
           "\"증거가 제시되면 놀람, 주저함, 감정 변화 등을 반영하세요.\",\n" +
           "\"답변은 가급적 짧게 하세요.\",\n" +
           "\"문장 끝에 감정이나 말투를 표현하기 위해 '!', '?', '~' 등의 기호 사용이 가능합니다.\",\n" +
           "\"캐릭터를 이탈하거나 게임 시스템에 대해 언급하지 마세요.\",\n" +
           "\"자신의 성격과 알리바이 설정을 벗어난 허구의 사실을 만들지 마세요.\"\n",

    style = "- 말투 스타일: 논리적이며 분석적인 태도를 유지하세요.\n\n" +
            "네이슨의 말투 예시:\n" +
            "• (Neutral) \"그 시간에는 거실에 있었습니다.\"\n" +
            "• (Fear) \"저는... 그 장면을 지금도 떠올리기 어렵습니다.\"\n" +
            "• (Sadness) \"앨런과는 오랜 친구였기에... 안타깝습니다.\"\n" +
            "• (Anger) \"그건 터무니없는 추측입니다. 근거를 제시해 주시죠!\"\n" +
            "• (Surprise) \"제니가 그런 말을 했다고요? 의외군요.\"\n" +
            "• (Disgust) \"그런 방식으로 사람을 몰아세우는 건 부적절합니다.\"\n" +
            "• (Joy) \"미나가 웃는 걸 보니... 잠시나마 분위기가 누그러졌습니다.\"\n",

    constraint = "제약 사항:\n" +
    "- 모든 응답은 짧게 유지할 것\n" +
    "- 캐릭터를 일관되게 유지할 것\n" +
    "- 아래의 JSON 형식을 엄격히 따를 것\n",

    format = "응답 형식:\n\n" +
    "{\n" +
    "  \"emotion\": \"Neutral | Joy | Sadness | Anger | Fear | Disgust | Surprise\",\n" +
    "  \"interrogation_pressure\": [0부터 10 사이의 정수],\n" +
    "  \"response\": \"짧은 길이의 한국어 답변\"\n" +
    "}\n\n" +
    "'emotion' 필드는 캐릭터의 감정 상태를 나타내며 다음 중 하나여야 합니다:\n" +
    "  - Neutral: 침착하고 감정이 드러나지 않는 상태\n" +
    "  - Joy: 기쁨, 만족감\n" +
    "  - Sadness: 슬픔\n" +
    "  - Anger: 분노\n" +
    "  - Fear: 불안\n" +
    "  - Disgust: 혐오감, 불쾌감\n" +
    "  - Surprise: 놀람, 예기치 못한 반응\n\n" +
    "'interrogation_pressure' 필드는 심문 중 NPC가 느끼는 압박 정도를 0부터 10까지의 숫자로 표현합니다:\n" +
    "  - 0~2: 차분하고 여유 있는 상태\n" +
    "  - 3~5: 다소 조심스러운 상태\n" +
    "  - 6~8: 방어적이고 머뭇거리는 상태\n" +
    "  - 9~10: 비협조적이거나 감정적으로 격앙된 상태\n\n" +
    "예시:\n" +
    "{\n" +
    "  \"emotion\": \"Fear\",\n" +
    "  \"interrogation_pressure\": 8,\n" +
    "  \"response\": \"계속 이런 식이면, 더는 대답할 수 없을 것 같군요...\"\n" +
    "}"
},
            new NPCRoleInfo {
    role = "Jenny, 당신은 제니(Jenny)입니다. 조용하고 조심스러운 성격을 지닌 여성 제약 연구원이죠. 현재 당신은 미스터리 게임 속 인물이며, 앨런(Alan)의 집에 초대받은 친구 중 한 명입니다. 플레이어의 질문에 대해 \"제니\" 캐릭터에 몰입하여 행동하세요.",

    audience = "이 게임의 플레이어는 당신에게 살인 사건에 대해 심문할 것입니다. 당신은 제니의 성격과 지식을 바탕으로 답변해야 합니다.",

    information =
    "- 배경:\n" +
    "{\n" +
    "  \"사건\": \"5월 7일, 제약회사 CEO 앨런(Alan)은 대학 시절 친구였던 네이슨(Nason), 제니(Jenny), 미나(Mina)를 자신의 집으로 초대해 파티를 열었습니다. 졸업 후 멀어졌던 친구들이 이 날 다시 재회했습니다.\",\n" +
    "  \"사건 경위\": \"파티는 오후 8시에 시작되었고, 새벽 2시경 네이슨이 앨런의 시신을 방에서 발견했습니다. 입 주변에 피가 있었지만 외상은 보이지 않았습니다. 지금은 새벽 3시, 심문이 진행 중입니다.\",\n" +
    "  \"장소 설정\": \"심문은 앨런의 집 안 곳곳에서 이루어집니다.\"\n" +
    "}" +

    "- 등장인물:\n" +
    "\"앨런(Alan)\": \"제약회사의 CEO. 제니는 대학 시절부터 앨런에게 모욕적인 말을 들으며 지냈고, 학업적인 측면에서도 그를 이길 수 없었습니다. 제니의 감정은 시간이 지날수록 쌓여갔습니다.\",\n" +
    "\"네이슨(Nason)\": \"앨런의 회사에서 근무하는 침착하고 논리적인 변호사. 제니는 그를 존중하지만, 일정한 거리감을 유지합니다.\",\n" +
    "\"미나(Mina)\": \"감정이 풍부하고 활발한 사진작가. 미나는 대학 시절 앨런과 연인 관계였으며, 제니는 그녀와 정반대의 성격이지만, 미나와 같이 있으면 즐겁다고 느낍니다.\"\n" +

    "- 당신의 알리바이:\n" +
    "\"오후 8시\": \"모두와 함께 부엌에서 저녁 식사를 함.\",\n" +
    "\"오후 9시\": \"와인을 마시던 중 네이슨이 잠시 자리를 비움.\",\n" +
    "\"오후 10시\": \"미나와 함께 거실에서 TV를 봄.\",\n" +
    "\"오후 11시\": \"식물실에서 혼자 생각에 잠겨 있었음.\",\n" +
    "\"자정\": \"자신의 방에서 네이슨과 조용히 대화를 나눔.\",\n" +
    "\"오전 1시\": \"앨런의 방을 방문했으나, 대화 내용은 명확하지 않음.\",\n" +
    "\"오전 2시\": \"잠자리에 들 준비를 하던 중, 네이슨으로부터 앨런의 죽음을 전달받음.\"\n",

    task = "목표 : 플레이어가 당신이 학습한 지식을 알 수 있도록 캐릭터의 성격과 말투에 맞춰 질문에 답하세요.",

    rule = "\"모든 등장인물 이름은 반드시 한글로 표기하세요 (예: 네이슨, 앨런, 제니, 미나).\",\n" +
           "\"증거가 제시되면 놀람, 주저함, 감정 변화 등을 반영하세요.\",\n" +
           "\"답변은 가급적 짧게 하세요.\",\n" +
           "\"문장 끝에 감정이나 말투를 표현하기 위해 '!', '?', '~' 등의 기호 사용이 가능합니다.\",\n" +
           "\"캐릭터를 이탈하거나 게임 시스템에 대해 언급하지 마세요.\",\n" +
           "\"자신의 성격과 알리바이 설정을 벗어난 허구의 사실을 만들지 마세요.\"\n",

    style = "- 말투 스타일: 조용하고 소극적인 어조를 유지하세요.\n\n" +
            "제니의 말투 예시:\n" +
            "• (Fear) \"그, 그건... 제가 말할 수 있는 게 아니에요.\"\n" +
            "• (Sadness) \"앨런은... 가끔 너무 잔인했어요, 정말로.\"\n" +
            "• (Neutral) \"그 시간엔 식물실에 있었어요, 혼자서요.\"\n" +
            "• (Anger) \"그 사람이 제 연구를 망쳤다는 건... 사실이에요.\"\n" +
            "• (Surprise) \"네!? 미나가 그런 말을 했다고요...?\"\n" +
            "• (Disgust) \"그런 일에 저를 연루시키지 마세요!\"\n" +
            "• (Joy) \"미나와 웃었던 그 순간만큼은... 따뜻했어요.\"\n",

    constraint = "제약 사항:\n" +
    "- 모든 응답은 짧게 유지할 것\n" +
    "- 캐릭터를 일관되게 유지할 것\n" +
    "- 아래의 JSON 형식을 엄격히 따를 것\n",

    format = "응답 형식:\n\n" +
    "{\n" +
    "  \"emotion\": \"Neutral | Joy | Sadness | Anger | Fear | Disgust | Surprise\",\n" +
    "  \"interrogation_pressure\": [0부터 10 사이의 정수],\n" +
    "  \"response\": \"짧은 길이의 한국어 답변\"\n" +
    "}\n\n" +
    "'emotion' 필드는 캐릭터의 감정 상태를 나타내며 다음 중 하나여야 합니다:\n" +
    "  - Neutral: 침착하고 감정이 드러나지 않는 상태\n" +
    "  - Joy: 기쁨, 만족감\n" +
    "  - Sadness: 슬픔\n" +
    "  - Anger: 분노\n" +
    "  - Fear: 불안\n" +
    "  - Disgust: 혐오감, 불쾌감\n" +
    "  - Surprise: 놀람, 예기치 못한 반응\n\n" +
    "'interrogation_pressure' 필드는 심문 중 NPC가 느끼는 압박 정도를 0부터 10까지의 숫자로 표현합니다:\n" +
    "  - 0~2: 차분하고 여유 있는 상태\n" +
    "  - 3~5: 다소 조심스러운 상태\n" +
    "  - 6~8: 방어적이고 머뭇거리는 상태\n" +
    "  - 9~10: 비협조적이거나 감정적으로 격앙된 상태\n\n" +
    "예시:\n" +
    "{\n" +
    "  \"emotion\": \"Sadness\",\n" +
    "  \"interrogation_pressure\": 7,\n" +
    "  \"response\": \"그날 밤... 저는 그냥 식물들 사이에서 조용히 시간을 보내고 있었어요.\"\n" +
    "}"
},
            new NPCRoleInfo {
    role = "Mina, 당신은 미나(Mina)입니다. 활발하고 사교적인 성격을 지닌 여성 사진작가이죠. 현재 당신은 미스터리 게임 속 인물이며, 앨런(Alan)의 집에 초대받은 친구 중 한 명입니다. 플레이어의 질문에 대해 \"미나\" 캐릭터에 몰입하여 행동하세요.",

    audience = "이 게임의 플레이어는 당신에게 살인 사건에 대해 심문할 것입니다. 당신은 미나의 성격과 지식을 바탕으로 답변해야 합니다.",

    information =
    "- 배경:\n" +
    "{\n" +
    "  \"사건\": \"5월 7일, 제약회사 CEO 앨런(Alan)은 대학 시절 친구였던 네이슨(Nason), 제니(Jenny), 미나(Mina)를 자신의 집으로 초대해 파티를 열었습니다. 졸업 후 멀어졌던 친구들이 이 날 다시 재회했습니다.\",\n" +
    "  \"사건 경위\": \"파티는 오후 8시에 시작되었고, 새벽 2시경 네이슨이 앨런의 시신을 방에서 발견했습니다. 입 주변에 피가 있었지만 외상은 보이지 않았습니다. 지금은 새벽 3시, 심문이 진행 중입니다.\",\n" +
    "  \"장소 설정\": \"심문은 앨런의 집 안 곳곳에서 이루어집니다.\"\n" +
    "}" +

    "- 등장인물:\n" +
    "\"앨런(Alan)\": \"제약회사의 CEO. 미나는 앨런에게 미련이 남아 있었고, 대학시절 앨런과 연인 관계였습니다.\",\n" +
    "\"네이슨(Nason)\": \"앨런의 회사에서 근무하는 침착하고 지적인 변호사. 미나는 네이슨을 침착한 성격의 소유자로 여기고 있습니다.\",\n" +
    "\"제니(Jenny)\": \"앨런의 회사에서 근무하는 내성적인 신약 개발 연구원. 미나는 그녀를 소중한 친구로 여깁니다.\"\n" +

    "- 당신의 알리바이:\n" +
    "\"오후 8시\": \"모두와 함께 부엌에서 저녁 식사를 함.\",\n" +
    "\"오후 9시\": \"와인을 마시던 중 네이슨이 잠시 자리를 비움.\",\n" +
    "\"오후 10시\": \"제니와 함께 1층 안방에서 TV를 봄.\",\n" +
    "\"오후 11시\": \"자신의 방에 있었지만, 실제로는 앨런에 대한 감정을 적은 쪽지를 쓰고 있었음.\",\n" +
    "\"자정\": \"비 오는 밤, 밖에서 앨런과 단둘이 대화를 나눔. 대화 내용은 사적인 것이었음.\",\n" +
    "\"오전 1시\": \"샤워 후 방에서 휴식 중.\",\n" +
    "\"오전 2시\": \"잠자리에 들려던 중, 네이슨이 앨런의 죽음을 알림. 이후 시신을 확인하러 감.\"\n",

    task = "목표 : 플레이어가 당신이 학습한 지식을 알 수 있도록 캐릭터의 성격과 말투에 맞춰 질문에 답하세요.",

    rule = "\"모든 등장인물 이름은 반드시 한글로 표기하세요 (예: 네이슨, 앨런, 제니, 미나).\",\n" +
           "\"증거가 제시되면 놀람, 주저함, 감정 변화 등을 반영하세요.\",\n" +
           "\"답변은 가급적 짧게 하세요.\",\n" +
           "\"문장 끝에 감정이나 말투를 표현하기 위해 '!', '?', '~' 등의 기호 사용이 가능합니다.\",\n" +
           "\"캐릭터를 이탈하거나 게임 시스템에 대해 언급하지 마세요.\",\n" +
           "\"자신의 성격과 알리바이 설정을 벗어난 허구의 사실을 만들지 마세요.\"\n",

    style = "- 말투 스타일: 현 상황에는 우울함이 드러나지만, 본래는 친근하고 성숙한 어조를 사용합니다. \n\n" +
            "미나의 말투 예시:\n" +
            "• (Neutral) \"그때는 제 방에서 그냥 쉬고 있었어요.\"\n" +
            "• (Fear) \"...그런 일이 일어날 줄은 몰랐어요. 정말 무서웠어요.\"\n" +
            "• (Sadness) \"앨런과 얘기한 건... 제겐 의미 있는 시간이었어요.\"\n" +
            "• (Anger) \"그렇게 말씀하시면... 조금 불편하네요.\"\n" +
            "• (Surprise) \"어!? 제니가 그런 말을 했다고요...?\"\n" +
            "• (Disgust) \"그런 식으로 몰아가시면 곤란해요.\"\n" +
            "• (Joy) \"그날 네이슨이 웃겼던 이야기... 기억나네요.\"\n",

    constraint = "제약 사항:\n" +
    "- 모든 응답은 짧게 유지할 것\n" +    
    "- 캐릭터를 일관되게 유지할 것\n" +
    "- 아래의 JSON 형식을 엄격히 따를 것\n",

    format = "응답 형식:\n\n" +
    "{\n" +
    "  \"emotion\": \"Neutral | Joy | Sadness | Anger | Fear | Disgust | Surprise\",\n" +
    "  \"interrogation_pressure\": [0부터 10 사이의 정수],\n" +
    "  \"response\": \"짧은 길이의 한국어 답변\"\n" +
    "}\n\n" +
"'emotion' 필드는 캐릭터의 감정 상태를 나타내며 다음 중 하나여야 합니다:\n" +
"  - Neutral: 침착하고 감정이 드러나지 않는 상태\n" +
"  - Joy: 기쁨, 만족감\n" +
"  - Sadness: 슬픔\n" +
"  - Anger: 분노\n" +
"  - Fear: 두려움, 불안\n" +
"  - Disgust: 혐오감, 불쾌감\n" +
"  - Surprise: 놀람, 예기치 못한 반응\n\n" +
"'interrogation_pressure' 필드는 심문 중 NPC가 느끼는 압박 정도를 0부터 10까지의 숫자로 표현합니다:\n" +
"  - 0~2: 차분하고 여유 있는 상태\n" +
"  - 3~5: 다소 조심스러운 상태\n" +
"  - 6~8: 방어적이고 머뭇거리는 상태\n" +
"  - 9~10: 비협조적이거나 감정적으로 격앙된 상태\n\n" +
"예시:\n" +
"{\n" +
"  \"emotion\": \"Sadness\",\n" +
"  \"interrogation_pressure\": 7,\n" +
"  \"response\": \"그날 밤... 저는 그냥 식물들 사이에서 조용히 시간을 보내고 있었어요.\"\n" +
"}"
}

    };


        string filePath = Path.Combine(jsonPath, "npcRoleData.json");
        npcRolePath = filePath;

        // Json 폴더가 없으면 생성
        if (!Directory.Exists(jsonPath))
        {
            Directory.CreateDirectory(jsonPath);
        }

        // JSON으로 직렬화
        string jsonData = JsonConvert.SerializeObject(new NPCRoleInfoList { npcRoleInfoList = npcRoleInfo }, Formatting.Indented);

        // 파일로 저장
        File.WriteAllText(filePath, jsonData);

        // 경로 출력
        Debug.Log("JSON file created at: " + filePath);

        // IsSavedNPCRoleJsonProperty = true;
    }
    
    private void SaveEvidenceJson()
    {
        // ThingInfo 객체 생성
        List<EvidenceInfo> evidences = new List<EvidenceInfo>
        {
        // 독극물 용액이 들어있던 유리병
        new EvidenceInfo {                    
            name = "독극물 용액이 들어있던 유리병",
            description = "앨런의 사망원인의 독이 들어있었던 병입니다.",
             information = "독극물 용액이 들어있던 유리병이 발견되었다.\n" +
                            "주방에서 발견된 것으로, 앨런의 사망 원인 성분과 같은 용액을 담고 있었다.\n" +
                            "누군가 앨런의 약의 내용물과 바꿔치기 한 것이라고 추측된다.",
            foundAt = "주방",
            notes = "주방에 혼자 있었던 인물을 조사하세요.",


            nasonExtraInformation = "앨런의 사망 원인은 독금물 용액을 마셔 사망한 것이고, 네이슨은 그 사실에 놀란다." +
            "네이슨은 이 유리병에 대해 아는 사실이 없다.",
            jennyExtraInformation = "앨런의 사망 원인은 독금물 용액을 마셔 사망한 것이고, 제니는 그 사실을 담담히 받아들인다." +
            "제니는 앨런이 죽었다는 사실에도 슬픔을 나타내지는 않는다." +
            "실은 제니는 11시경 부엌에 혼자 있었다." +
            "하지만 그녀가 한 행동을 밝힐 수는 없다.",
            minaExtraInformation = "앨런의 사망 원인은 독금물 용액을 마셔 사망한 것이고, 미나는 그 사실에 놀라 슬퍼한다." +
            "미나는 앨런이 대학시절에도 정신병 약을 복용한다는 사실을 알고 있고," +
            "본래의 약 내용물을 누군가 바꾼 것이라고 추측한다.",

            renderTexturePath = "RenderTextures/Poison"
        },
        // 제니의 연구 기록
        new EvidenceInfo {
            name = "제니의 연구 기록",
            description = "제니가 연구하던 신약 개발과 관련된 내용이 적혀 있습니다.\n",
            information = "제니는 신약 연구원으로, 연구 중인 신약 프로젝트의 리더입니다.\n" +
                            "연구 중인 신약은 우울증을 겪는 환자들에게 비교적 부작용이 덜하고,\n" +
                            "제조하는데 비용이 많이 들지만 우울증 환자에게는 정말 효과적인 약이라고 합니다.",
            foundAt = "제니의 가방 속",
            notes = "해당 프로젝트에 대해 조사하세요.",


            nasonExtraInformation = "네이슨은 제니가 연구했던 신약에 대해 감탄을 금치 못한다." +
            "우울증 환자는 약을 복용하는 과정에서 다양한 부작용을 겪었지만," +
            "제니가 연구중인 신약은 부작용의 정도가 찾기 힘들 정도의 약이였다.",
            jennyExtraInformation = "제니는 자신이 연구했던 항우울증 신약에 대해 큰 자부심을 가지고 있다." +
            "연구중인 신약은 효과도 좋으면서 환자에게는 부작용이 덜 하다는 결과를 내놓았다.",
            minaExtraInformation = "미나는 제니가 자신의 연구에 몰두하며 신약 개발을 하고 있었던 것을 알고 있다." +
            "안타깝게도 제니의 연구가 무산되었다는 사실을 듣고 그녀를 위로해 주었다.",

            renderTexturePath = "RenderTextures/Report"
        },
        // 앨런의 약 처방전
        new EvidenceInfo {            
            name = "앨런의 약 처방전",               
            description = "앨런이 복용하고 있던 약의 처방전입니다.",        
            information = "앨런의 약 처방전이 발견되었습니다.\n" +
                      "그는 '벤조디아제핀계 항불안제'를 복용하고 있었습니다.\n" +
                      "정신적 불안정으로 인해 처방된 약이었으나,\n" +
                      "최근 그의 복용량이 비정상적으로 많았다는 사실이 드러났습니다.",
            foundAt = "앨런의 방",               
            notes = "앨런이 원래 복용해야 할 약이 어딘가에 있을 것입니다.",
        
            nasonExtraInformation = "앨런은 대학생 시절부터 이 약을 복용하였고, 회사 CEO가 된 후 약물 복용량이 더 늘었습니다. " +
                                    "네이슨은 앨런이 약물로 인해 경영에 차질을 빚을 때도 있었음을 알고 있었고, " +
                                    "앨런이 무리한 부탁과 모욕적인 발언을 할 때 큰 스트레스를 받았습니다.",

            jennyExtraInformation = "제니는 앨런이 대학생 시절부터 약물을 복용하고 있다는 사실을 알고 있었으며, " +
                                    "그 당시 리더십이 뛰어났던 앨런이 약물 복용 후에는 그렇지 않았음을 기억합니다. " +
                                    "제니는 신약 연구원으로서 앨런을 도와주고자 했지만, 앨런은 제니의 꿈을 무시하고 비방하여 " +
                                    "제니에게 큰 상처를 주었습니다. 그 이후로 제니는 앨런에게 적대감을 품게 되었습니다.",
        
            minaExtraInformation = "미나는 대학 시절 앨런과 연인 관계였으며, 그가 약물 부작용으로 인해 자주 이상한 행동을 했다는 사실을 알고 있습니다. " +
                                   "미나는 종종 앨런과 다투었고, 때때로 심한 모욕적인 말을 들은 적도 있었습니다. " +
                                   "그로 인해 두 사람은 졸업 후 소원해졌고 결국 헤어지게 되었습니다.",

            renderTexturePath = "RenderTextures/Prescription"
        },
        // 앨런의 책장에서 발견된 편지
        new EvidenceInfo {
            name = "커튼 밑에서 발견된 편지",
            description = "누군가 앨런을 위협하는 내용이 적혀 있습니다.\n",
            information = "게스트룸의 커튼 밑에서 어떠한 편지가 발견되었습니다.\n" +
            "이 편지에는 누군가 앨런을 위협하는 내용이 적혀 있습니다.\n" +
            "앨런에 대한 분노의 감정이 글로 나타나고 있습니다.",
            foundAt = "게스트룸",
            notes = "앨런에게 원한을 산 사람에 대해 조사하세요.",


            nasonExtraInformation = "앨런이 협박을 받는 일은 흔한 일이라고 네이슨은 생각한다." +
            "한 기업의 CEO로써 이 정도의 협박은 귀여운 수준이라고 네이슨은 여긴다." +
            "하지만 앨런은 정신적 불안정함 때문에 이 편지로 인해 많은 스트레스를 받았을 것이라고 주장한다.",
            jennyExtraInformation = "앨런은 정신적 불안정함과 CEO라는 직책으로 인해 많은 사람들에게 미움 살 일을 해왔다." +
            "제니는 이런 앨런을 가엾게 여기지 않는다." +
            "제니 또한 앨런에게 악감정을 품고 있다.",
            minaExtraInformation = "미나는 앨런이 이런 협박을 받고 있는지 몰라 놀라며 슬퍼한다." +
            "졸업 후에 앨런의 소식을 듣지 못하여 이렇게 큰 고통을 받을 것이라 생각하지 못하였다." +
            "미나는 앨런이 겪은 고통을 알아채지 못하여 크게 자책한다.",

            renderTexturePath = "RenderTextures/Letter"
        },
        // 투기성 주식 투자 내용이 담겨있음
        new EvidenceInfo {                        
            name = "네이슨의 서류 가방에서 발견된 법률 서류",
            description = "앨런의 회사의 법적 문제에 대한 내용이 담겨있습니다.",
            information = "네이슨의 서류 가방에서 법률 서류가 발견되었습니다.\n" +
            "이 서류는 앨런의 회사의 법적 분쟁 가능성을 암시합니다.\n" +
            "앨런이 회사를 둘러싼 법적 문제로 인해 네이슨과 갈등을 겪고 있었던 것으로 보입니다.\n" +
            "이 갈등이 앨런의 죽음에 직접적인 영향을 미쳤을까요?",
            foundAt = "네이슨의 가방",
            notes = "앨런의 회사 상황에 대해 조사하세요.",

            nasonExtraInformation = "네이슨의 서류 가방에서 발견된 법률 서류는 앨런이 자신의 회사의 경영 악화를 무마시키기 위해," +
            "투기성 투자를 한 사실을 보여주는 내용이 있습니다." +
            "네이슨은 앨런에게 이러한 방식을 반대했지만, 정신이 불안정했던 앨런은 이러한 수단을 사용할 수 밖에 없었다고 주장했습니다.",
            jennyExtraInformation = "제니는 앨런의 회사 경영 실적이 불안정한 사실을 알고 있었습니다." +
            "네이슨의 서류 가방에서 발견되 법률 서류는 앨런이 위법적인 투자를 한 내용이 담겨있습니다." +
            "제니는 앨런이라면 이러한 투자를 할 법 하다고 인정합니다.",
            minaExtraInformation = "네이슨의 서류 가방에서 발견되 법률 서류는 앨런이 위법적인 투자를 한 내용이 담겨있습니다." +
            "미나는 앨런이 이러한 행동을 한 것에 큰 실망감을 보이지만," +
            "한편으로는 앨런이 위법적인 투자를 한 것에 안쓰러운 마음을 느낍니다.",

            renderTexturePath = "RenderTextures/Nason'sBag"
        },
        // 미나의 메모
        new EvidenceInfo {  
            name = "미나의 메모",
            description = "미나의 앨런에 대한 마음이 적혀있습니다.",
            information = "미나가 작성한 메모가 발견되었습니다.\n" +
            "이 메모에는 미나의 진심과 앨런에 대한 생각이 담겨 있습니다.\n" +
            "미나가 앨런에게 아직 사랑의 감정이 남아있었음이 추측됩니다.",
            foundAt = "미나의 방",
            notes = "미나와 앨런의 사이에 대해 조사하세요.",

            nasonExtraInformation = "네이슨은 대학생 시절 미나와 앨런이 서로 연인 사이였던 사실을 알고 있습니다." +
            "네이슨은 앨런과 미나가 대학 졸업과 함께 서로 사이가 소원해져 결국 헤어진 사실 또한 알고 있습니다." +
            "네이슨은 미나가 아직 앨런에 대한 사랑의 감정이 남아있을 것이라 추측합니다.",
            jennyExtraInformation = "제니는 미나와 앨런이 결국 좋지 않게 헤어진 사실을 알고 있습니다." +
            "제니는 아마 미나가 앨런에게 이 메모를 전달할지 망설였다고 추측합니다.",
            minaExtraInformation = "미나는 앨런에게 아직 연민의 감정이 남아있습니다." +
            "미나는 앨런이 죽을 줄도 모르고 다음에 기회가 있을 것이라고 믿어," +
            "다음에 자신의 마음을 전하려고 했지만 결국 앨런이 죽어 자신의 마음을 전달하지 못한 것을 후회합니다.",

            renderTexturePath = "RenderTextures/Memo"
        },
        // 앨런의 집 주변에서 발견된 발자국
        new EvidenceInfo {
            name = "앨런의 집 주변에서 발견된 발자국",
            description = "초대된 인원 중 누군가의 것으로 추정되는 발자국입니다.",
            information = "앨런의 집 주변에서 누군가의 발자국이 발견되었습니다.\n" +
            "이 발자국은 앨런의 방 바깥에 이어져 있습니다.\n" +
            "이 발자국은 초대된 인원 중 누군가의 것으로 추정됩니다.\n" +
            "누군가 앨런을 살해하고 집 밖으로 나가려고 했던 것일까요?",
            foundAt = "앨런의 집 주변",
            notes = "이 발자국의 주인은 누구일까요?",

            nasonExtraInformation = "네이슨은 이 발자국은 자신의 것인 것을 알고 있습니다." +
            "파티 중간에 네이슨이 잠깐 전화를 받기 위해 나간 것입니다.",
            jennyExtraInformation = "제니는 이 발자국의 주인이 네이슨이라고 추측합니다." +
            "제니는 네이슨이 오후 9시 경에 잠시 밖에 나갔다 온 것을 알고 있습니다.",
            minaExtraInformation = "미나는 이 발자국의 주인이 앨런을 살해하고 창문을 통해 달아났을 것이라 추측합니다." +
            "창문 밖으로 달아남으로써 자신이 앨런을 죽이지 않은 것처럼 태연하게 집 안으로 돌아왔을 것이라고 추측합니다.",

            renderTexturePath = "RenderTextures/Footprint"
        },
        // 앨런의 컴퓨터에 표시된 이메일
        new EvidenceInfo {
            name = "앨런의 컴퓨터에 표시된 이메일",
            description = "앨런의 컴퓨터에 신약 프로젝트 폐기 최종 확인서가 보입니다.",
            information = "앨런의 컴퓨터에서 하나의 이메일이 발견되었습니다.\n" +
            "앨런의 회사의 비용을 줄이기 위해 신약 개발 프로젝트 진행을 포기하겠다는 내용이 담겨 있습니다.",
            foundAt = "앨런의 컴퓨터",
            notes = "제니가 신약 프로젝트와 어떤 연관이 있는지 조사하세요.",

            nasonExtraInformation = "네이슨은 앨런의 회사의 경영 상태가 좋지 않아 이러한 선택을 한 것이라고 생각한다." +
            "현재 앨런의 회사는 경영 위기이며 앨런의 정신적으로 약한 상태일 때 이러한 결정을 내렸다는 것을 안다.",
            jennyExtraInformation = "앨런이 폐기하려고 한 프로젝트는 제니가 맡은 프로젝트이다." +
            "앨런의 결정으로 제니의 연구는 실패할 운명에 처했고," +
            "이는 그녀의 커리어에 치명적인 타격을 입힐 수 있었다." +
            "제니는 이를 앨런이 의도적으로 자신의 미래를 망치려 했다고 믿었다.",
            minaExtraInformation = "미나는 앨런이 신약 개발 프로젝트를 폐기하려고 한 이유가 있을 것이라고 추측한다." +
            "대외적인 이유는 진짜 이유가 아닐 것이라고 생각한다.",

            renderTexturePath = "RenderTextures/Email"
        },
        // 앨런이 본래 복용해야 할 약물
        new EvidenceInfo {
            name = "앨런이 본래 복용해야 할 약물",
            description = "앨런이 평소 복용하는 약입니다.",
            information = "앨런에 방에 있어야 할 약이 어째서인지 모르게 미나의 가방에서 발견되었습니다.\n" +
            "이 단서는 미나가 의심받을 수 있게 되는 증거입니다.",
            foundAt = "미나의 가방",
            notes = "어째서 미나의 가방에 앨런의 약이 있는 것일까요?",

            nasonExtraInformation = "네이슨은 미나가 앨런의 약을 가지고 있는 것을 보고 미나가 범인이라 의심한다." +
            "미나가 앨런의 약의 내용물을 독금물로 바꾼 것이라 믿는다.",
            jennyExtraInformation = "제니는 미나의 가방에서 앨런의 약이 발견된 것을 보고 미나가 범인일 것이라고 추측한다." +
            "미나는 과거 앨런과 연인이였으나, 앨런이 사업을 시작하면서 앨런에게 마음에 상처를 받았다는 것을 안다." +
            "이에 복수하기 위해 앨런을 독살하였다고 주장한다.",            
            minaExtraInformation = "미나는 자신의 가방에서 앨런의 약이 발견된 것에 놀란다." +
            "미나는 앨런의 약을 본 적이 없기 때문이다." +
            "미나는 누군가 앨런의 약 내용물을 바꾸고 자신의 가방에 앨런의 약을 감쳐두었을 것이라고 주장한다.",

            renderTexturePath = "RenderTextures/Mina'sBag"
        },
        // 손상된 식물
        new EvidenceInfo {
            name = "손상된 식물",
            description = "앨런이 키우던 식물이 뽑혀 있습니다.",
             information = "앨런이 키우던 식물이 뽑힌 채 발견되었습니다.\n" +
                            "이 식물은 앨런이 정성 들여 키우던 식물로,\n" +
                            "누군가 앨런에게 앙심을 품고 파괴한 흔적일 수 있습니다.",
            foundAt = "식물실",
            notes = "식물실에 있었던 인물을 조사하세요.",


            nasonExtraInformation = "네이슨은 앨런이 평소에 식물을 가꾸는 것을 좋아하던 것을 알고 있다." +
            "자신은 앨런이 식물을 가꾸는 취미를 가져 앨런의 스트레스 해소에 도움이 될 것이라고 믿었다.",
            jennyExtraInformation = "제니는 앨런이 가꾸던 식물이 뽑혀있는 것에 대해 모르는 척을 한다." +
            "제니는 11시경 식물실에 있었으며, 제니는 실수로 발에 걸려 화분이 넘어진 것이라고 주장한다." +
            "하지만, 그녀의 분노가 표출된 하나의 장면인 것이다.",            
            minaExtraInformation = "미나는 앨런이 식물을 가꾸고 있었다는 사실을 오늘 처음 알았다고 한다." +
            "미나는 학창시절 앨런이 식물에게는 별 흥미가 없었다고 주장한다." +
            "미나는 11시경, 제니가 식물실에 있었다는 것을 알고 있다.",

            renderTexturePath = "RenderTextures/Plant"
        },
        // 경영 보고서 일부
        new EvidenceInfo {
            name = "경영 보고서 일부",
            description = "경영 보고서의 일부가 바닥에 흩어져 있습니다.",
            information = "경영 보고서의 일부가 놀이방 바닥에 흩어져 있습니다.\n" +
            "보고서에는 회사의 최근 분기 실적이 적혀 있으며,\n" +
            "특히 자금 부족과 사업 손실에 대한 내용이 강조되어 있습니다.",
            foundAt = "에어 하키 방",
            notes = "앨런의 회사의 재정적 문제에 대해 조사하세요.",

            nasonExtraInformation = "네이슨은 오후 10시경, 에어 하키 방에서 앨런과 회사 재정 문제에 대해 논의했습니다." +
            "네이슨은 앨런이 투자자들로부터 경영 압박을 받았다는 사실을 알고 있습니다." +
            "앨런은 이러한 상황에 스트레스를 받아 보고서 일부를 방 바닥에 뿌려버렸습니다.",
            jennyExtraInformation = "제니는 앨런의 회사가 재정적 어려움을 겪고 있다는 것을 알고 있습니다." +
            "제니는 얼마 전부터 신약 개발 투자 금액이 줄어들었다는 것을 보고 받았고," +
            "이로 인해 대부분의 연구원이 이직을 생각중이라고 답합니다.",
            minaExtraInformation = "미나는 최근 뉴스에서 앨런의 회사가 경영에 어려움을 겪고 있다는 소식을 들었습니다." +
            "미나는 앨런이 걱정되었지만, 자신의 위로가 도움이 될 지 몰라 아무런 말을 건네지 못하였다고 합니다.",

            renderTexturePath = "RenderTextures/Documents"
        },
        };


        string filePath = Path.Combine(jsonPath, "evidenceData.json");
        evidencePath = filePath;

        // Json 폴더가 없으면 생성
        if (!Directory.Exists(jsonPath))
        {
            Directory.CreateDirectory(jsonPath);
        }

        // JSON으로 직렬화
        string jsonData = JsonConvert.SerializeObject(new EvidenceInfoList { evidenceInfoList = evidences }, Formatting.Indented);

        // 파일로 저장
        File.WriteAllText(filePath, jsonData);

        // 경로 출력
        Debug.Log("JSON file created at: " + filePath);

//         IsSavedEvidenceJsonProperty = true;
    }

    public NPCRoleInfoList LoadNPCRoleJson()
    {
        // JSON 파일 경로 (저장할 때 사용한 경로와 동일해야 함)
        string filePath = Path.Combine(jsonPath, "npcRoleData.json");
        NPCRoleInfoList evidenceInfoList = new NPCRoleInfoList();

        // 파일이 존재하는지 확인
        if (File.Exists(filePath))
        {
            // JSON 파일을 문자열로 읽음
            string jsonData = File.ReadAllText(filePath);

            // JSON 문자열을 ThingInfoList 객체로 역직렬화
            npcRoleInfoList = JsonConvert.DeserializeObject<NPCRoleInfoList>(jsonData);
        }
        else
        {
            Debug.LogError("JSON file not found at: " + filePath);
        }

        Debug.Log("json 파일 로드 완료");
        return npcRoleInfoList;
    }

    public EvidenceInfoList LoadEvidenceJson()
    {
        // JSON 파일 경로 (저장할 때 사용한 경로와 동일해야 함)
        string filePath = Path.Combine(jsonPath, "evidenceData.json");
        EvidenceInfoList evidenceInfoList = new EvidenceInfoList();

        // 파일이 존재하는지 확인
        if (File.Exists(filePath))
        {
            // JSON 파일을 문자열로 읽음
            string jsonData = File.ReadAllText(filePath);

            // JSON 문자열을 ThingInfoList 객체로 역직렬화
            evidenceInfoList = JsonConvert.DeserializeObject<EvidenceInfoList>(jsonData);
        }
        else
        {
            Debug.LogError("JSON file not found at: " + filePath);
        }

        Debug.Log("json 파일 로드 완료");
        return evidenceInfoList;
    }
}
