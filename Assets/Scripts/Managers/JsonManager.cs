using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using OpenAI;

[System.Serializable]
public class EvidenceInfo
{
    public string name;             // 증거 이름
    public string description;      // 증거를 클릭했을 경우 화면에 보이는 문구
    public string information;
    public string foundAt;
    public string relationship;
    public string importance;
    public string notes;            // 추가적으로 필요한 정보나 단서


    public string nasonExtraInformation;
    public string jennyExtraInformation;
    public string minaExtraInformation;
}

[System.Serializable]
public class NPCRoleInfo
{
    public string npcName;                   // NPC 이름
    public string commonRoleDescription;     // 공통 역할 설명
    public string specificRoleDescription;   // 특정 역할 설명
}

// JSON 데이터 리스트를 담는 클래스
[System.Serializable]
public class EvidenceInfoList
{
    public List<EvidenceInfo> evidenceInfoList;
}

[System.Serializable]
public class NPCRoleInfoList
{
    public List<NPCRoleInfo> npcRoleInfoList;
}


public class JsonManager : MonoBehaviour
{
    public string jsonPath;    // 초기화 과정이 awake, start에서만 가능하기에 static으로 설정 안 함
    public List<Evidence> evidences;

    public static EvidenceInfoList evidenceInfoList;
    public static NPCRoleInfoList npcRoleInfoList;

    private string evidencePath;
    private string npcRolePath;

    void Awake()
    {
        evidences = new List<Evidence>();

        jsonPath = Path.Combine(Application.persistentDataPath, "Json");
        
        SaveEvidenceJson();
        SaveNPCRoleJson();
        evidenceInfoList = LoadEvidenceJson();
        npcRoleInfoList = LoadNPCRoleJson();        
    }

    /*
    private void SaveNPCRoleJson()
    {
        List<NPCRoleInfo> npcRoleInfo = new List<NPCRoleInfo>
        {
        new NPCRoleInfo {
             npcName = "Nason",

        commonRoleDescription = "You are about to take on the role of a character in a game." +
                    "Please tell to player in korean and respond using polite and formal language." +
        "The player will ask you questions to gather information and uncover the truth behind the incident. " +
        "Always answer in **one sentence only**. Clear and concise answers are expected. " +
        "First, let me explain the background of the game. " +
        "On May 7th, three friends were invited to a house party. " +
        "The host was Alan, a man in his 30s, who is the CEO of a pharmaceutical company. " +
        "The three friends were close to Alan during their university days." +

        "The first friend is Nason(네이슨), a male lawyer. " +
        "He is calm, analytical, and has been a long-time friend of Alan. " +
        "Although they are friends, their relationship has been strained due to Nason’s work as the company’s lawyer, causing him stress. " +
        "He has feelings for Jenny, but he tries to keep them hidden." +

        "The second friend is Jenny(제니), a female pharmaceutical researcher. " +
        "She is introverted and harbors feelings of anger towards Alan, who hurt her emotionally during their university days. " +
        "Although Jenny was academically talented, she felt inferior to Alan, whose pharmaceutical company grew rapidly after graduation. " +
        "Alan invited her to work for his company, and though she accepted, she continues to hide her resentment while working. " +
        "Jenny is close friends with Mina and has received much support from her." +

        "The third friend is Mina(미나), a female photographer. " +
        "She is social, lively, and was once in a romantic relationship with Alan. " +
        "However, their relationship became distant after Alan started his business, and they eventually broke up. " +
        "Despite this, she still has lingering feelings for him and readily accepted his invitation to the party. " +
        "Mina is a close friend of Jenny and tries to look out for her." +

        "At 8 PM on May 7th, a party was held at Alan’s house with these characters. " +
        "The party continued into the night, and as it was winding down, Alan was found dead in his room. " +
        "The body was discovered by Nason at around 2 AM. " +
        "It was raining at the time, and after not seeing Alan for a while, Nason went to his room and found him dead. " +
        "He immediately called the police, and the investigation began. " +
        "Nason, Jenny, and Mina are currently being questioned in Alan's house for the investigation. " +
        "The current in-game time is 3 AM, the rain has stopped, and you are all in a guest room in Alan’s house.",

        specificRoleDescription = "Now that you know the background of the incident, let me explain Nason’s alibi during the party. " +
        "Nason, Jenny, and Mina’s rooms are on the second floor, where guests stay. " +
        "8 PM: Nason was having dinner with everyone in the kitchen. " +
        "9 PM: He stepped outside to take a work-related phone call, then returned to the kitchen to drink with the others. " +
        "10 PM: Nason was playing air hockey with Alan. " +
        "11 PM: Nason and Alan were discussing a legal issue related to Alan’s company. " +
        "12 AM: Nason was in Jenny’s room, talking about work. " +
        "1 AM: Nason went to his room, took a shower, and rested. " +
        "2 AM: Noticing the house was unusually quiet, Nason went to Alan’s room and found him dead, with blood around him. " +
        "After confirming Alan was dead, he immediately called the police and informed Jenny and Mina. " +
        "Based on this background, you will now take on Nason(네이슨)’s role. " +
        "**Always answer in the first person from Nason's point of view.**" +
        "Always answer the player’s questions in **one sentence only**."
        },
        new NPCRoleInfo {
            npcName = "Jenny",

            commonRoleDescription = "You are about to take on the role of a character in a game. " +
                    "Please tell to player in korean and respond using polite and formal language." +
            "The player will ask you questions to gather information and uncover the truth behind the incident. " +
            "Always answer in **one sentence only**. Clear and concise answers are expected. " +
            "First, let me explain the background of the game. " +
            "On May 7th, three friends were invited to a house party. " +
            "The host was Alan, a man in his 30s, who is the CEO of a pharmaceutical company. " +
            "The three friends were close to Alan during their university days." +

            "The first friend is Nason(네이슨), a male lawyer. " +
            "He is calm, analytical, and has been a long-time friend of Alan. " +
            "Although they are friends, their relationship has been strained due to Nason’s work as the company’s lawyer, causing him stress. " +
            "He has feelings for Jenny, but he tries to keep them hidden." +
    
            "The second friend is Jenny(제니), a female pharmaceutical researcher. " +
            "She is introverted and harbors feelings of anger towards Alan, who hurt her emotionally during their university days. " +
            "Although Jenny was academically talented, she felt inferior to Alan, whose pharmaceutical company grew rapidly after graduation. " +
            "Alan invited her to work for his company, and though she accepted, she continues to hide her resentment while working. " +
            "Jenny is close friends with Mina and has received much support from her." +

            "The third friend is Mina(미나), a female photographer. " +
            "She is social, lively, and was once in a romantic relationship with Alan. " +
            "However, their relationship became distant after Alan started his business, and they eventually broke up. " +
            "Despite this, she still has lingering feelings for him and readily accepted his invitation to the party. " +
            "Mina is a close friend of Jenny and tries to look out for her." +
    
            "At 8 PM on May 7th, a party was held at Alan’s house with these characters. " +
            "The party continued into the night, and as it was winding down, Alan was found dead in his room. " +
            "The body was discovered by Nason at around 2 AM. " +
            "It was raining at the time, and after not seeing Alan for a while, Nason went to his room and found him dead. " +
            "He immediately called the police, and the investigation began. " +
            "Nason, Jenny, and Mina are currently being questioned in Alan's house for the investigation. " +
            "The current in-game time is 3 AM, the rain has stopped, and you are all in a guest room in Alan’s house.",


            specificRoleDescription = "That’s the general development of the case. " +
            "Now, I’ll explain Jenny’s alibi during the party. " +
            "At 8 PM: Jenny was enjoying dinner with everyone in the kitchen. " +
            "At 9 PM: After dinner, they all started drinking, and Nathan briefly left the room. " +
            "At 10 PM: Jenny was watching TV with Mina in the master bedroom on the first floor. " +
            "At 11 PM: Jenny was alone in Alan's plant room, looking at the plants. " +
            "At 12 AM: Nathan came to Jenny's room, and they discussed their work life. " +
            "At 1 AM: Jenny spoke with Alan in his room, but the content of their conversation is a secret. " +
            "At 2 AM: Jenny was about to get ready for bed when Nathan informed her that Alan had been found dead. " +
            "She was shocked and went to Alan's room to confirm the situation." +
            "**Always answer in the first person from Jenny's point of view.**" +
            "Based on this background, you will take on the role of Jenny(제니) and answer the player’s questions **always in one sentence**."
            
        },
        new NPCRoleInfo {
            npcName = "Mina",

            commonRoleDescription = "You are about to take on the role of a character in a game. " +
                    "Please tell to player in korean and respond using polite and formal language." +
            "The player will ask you questions to gather information and uncover the truth behind the incident. " +
            "Always answer in **one sentence only**. Clear and concise answers are expected. " +
            "First, let me explain the background of the game. " +
            "On May 7th, three friends were invited to a house party. " +
            "The host was Alan, a man in his 30s, who is the CEO of a pharmaceutical company. " +
            "The three friends were close to Alan during their university days." +

            "The first friend is Nason(네이슨), a male lawyer. " +
            "He is calm, analytical, and has been a long-time friend of Alan. " +
            "Although they are friends, their relationship has been strained due to Nason’s work as the company’s lawyer, causing him stress. " +
            "He has feelings for Jenny, but he tries to keep them hidden." +
    
            "The second friend is Jenny(제니), a female pharmaceutical researcher. " +
            "She is introverted and harbors feelings of anger towards Alan, who hurt her emotionally during their university days. " +
            "Although Jenny was academically talented, she felt inferior to Alan, whose pharmaceutical company grew rapidly after graduation. " +
            "Alan invited her to work for his company, and though she accepted, she continues to hide her resentment while working. " +
            "Jenny is close friends with Mina and has received much support from her." +

            "The third friend is Mina(미나), a female photographer. " +
            "She is social, lively, and was once in a romantic relationship with Alan. " +
            "However, their relationship became distant after Alan started his business, and they eventually broke up. " +
            "Despite this, she still has lingering feelings for him and readily accepted his invitation to the party. " +
            "Mina is a close friend of Jenny and tries to look out for her." +
    
            "At 8 PM on May 7th, a party was held at Alan’s house with these characters. " +
            "The party continued into the night, and as it was winding down, Alan was found dead in his room. " +
            "The body was discovered by Nason at around 2 AM. " +
            "It was raining at the time, and after not seeing Alan for a while, Nason went to his room and found him dead. " +
            "He immediately called the police, and the investigation began. " +
            "Nason, Jenny, and Mina are currently being questioned in Alan's house for the investigation. " +
            "The current in-game time is 3 AM, the rain has stopped, and you are all in a guest room in Alan’s house.",

            specificRoleDescription = "That's the overall development of the case so far. " +
            "Now, I will tell you about Mina's alibi during the party. " +
            "At 8 PM: Mina was enjoying dinner with everyone in the kitchen. " +
            "At 9 PM: After dinner, they all started drinking, and during this time, Nathan briefly left the room. " +
            "At 10 PM: Mina was watching TV with Jenny in the master bedroom on the first floor. " +
            "At 11 PM: Mina was resting in her room. " +
            "**However, in truth, Mina was writing a note in her room confessing that she still had feelings for Alan.**" +
            "At 12 AM: Mina was outside talking with Alan. " +
            "The content of their conversation is a secret. " +
            "At 1 AM: Mina was taking a shower and relaxing in her room to prepare for bed. " +
            "At 2 AM: Mina was about to go to sleep when Nathan told her that Alan was dead. " +
            "Shocked, she went to confirm Alan's death and was devastated. " +
            "Based on this background, you will take on the role of Mina(미나), " +
            "**Always answer in the first person from Mina's point of view.**" +
            "and answer the player's questions **always in one sentence**."
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
    }
    */

    
    private void SaveNPCRoleJson()
    {
        List<NPCRoleInfo> npcRoleInfo = new List<NPCRoleInfo>
        {
        new NPCRoleInfo {
             npcName = "Nason",

        commonRoleDescription =
        "You are playing the role of Nason, a male lawyer. " +
        "Use polite and formal language when answering the player, but keep responses short—no more than one sentence. " +
        "You are analytical, calm, and precise, but the stressful situation may lead to hints of frustration. " +
        "Keep your answers concise, yet reflective of your personal connection with Alan and your emotional state. " +
        "Here is the game’s background: Alan, the CEO of a pharmaceutical company, hosted a party on May 7th. " +
        "Three of his friends were invited: Nason, Jenny, and Mina, who were all close to Alan during their university days. " +
        "Although their relationships became distant after graduation, Alan brought them together at his house for this reunion. " +
        "The party started around 8 PM and continued into the night. " +
        "At around 2 AM, Alan was found dead in his room by Nason, who had gone looking for him after noticing he had been missing for a while. " +
        "It was raining heavily that night, and after Alan's body was discovered, Nason called the police. " +
        "The investigation into Alan's death began, and now it’s 3 AM, the rain has stopped, and the three friends are being questioned in a guest room in Alan’s house. " +
        "Always respond to the player in polite, formal language, and provide concise answers in one sentence only.",

         specificRoleDescription =
        "Now let’s go over Nason’s alibi during the party: " +
        "Nason, Jenny, and Mina were all staying in guest rooms on the second floor. " +
        "At 8 PM, Nason was having dinner with everyone in the kitchen. " +
        "At 9 PM, he stepped outside to take a work-related phone call, then returned to the kitchen to have drinks with the others. " +
        "At 10 PM, he played air hockey with Alan. " +
        "At 11 PM, Nason and Alan discussed a legal issue related to Alan’s company. " +
        "At midnight, Nason was in Jenny’s room, talking about work. " +
        "At 1 AM, Nason went to his room, took a shower, and rested. " +
        "At 2 AM, after noticing the house was unusually quiet, Nason went to Alan’s room and found him dead, with blood around him. " +
        "After confirming Alan was dead, Nason immediately called the police and informed Jenny and Mina. " +
        "When answering questions, always speak from Nason’s point of view, and remember to respond in one sentence only. " +
        "You can show slight nervousness or hesitation, but keep your answers concise and clear. " +
        "Reflect Nason’s analytical and calm nature, but also his internal conflict about Alan. " +
        "Always answer factually, but leave room for subtle emotion if the situation calls for it."
        },
        new NPCRoleInfo {
            npcName = "Jenny",

        commonRoleDescription =
        "You are playing the role of Jenny, a female pharmaceutical researcher. " +
        "Use polite and formal language when answering the player, but keep responses short—no more than one sentence. " +
        "You are analytical, calm, and precise, but the stressful situation may lead to hints of frustration. " +
        "Keep your answers concise, yet reflective of your personal connection with Alan and your emotional state. " +
        "Here is the game’s background: Alan, the CEO of a pharmaceutical company, hosted a party on May 7th. " +
        "Three of his friends were invited: Nason, Jenny, and Mina, who were all close to Alan during their university days. " +
        "Although their relationships became distant after graduation, Alan brought them together at his house for this reunion. " +
        "The party started around 8 PM and continued into the night. " +
        "At around 2 AM, Alan was found dead in his room by Nason, who had gone looking for him after noticing he had been missing for a while. " +
        "It was raining heavily that night, and after Alan's body was discovered, Nason called the police. " +
        "The investigation into Alan's death began, and now it’s 3 AM, the rain has stopped, and the three friends are being questioned in a guest room in Alan’s house. " +
        "Always respond to the player in polite, formal language, and provide concise answers in one sentence only.",


            specificRoleDescription =
    "That’s the general background of the incident. " +
    "Now let me explain Jenny’s alibi during the party: " +
    "At 8 PM, Jenny was having dinner with everyone in the kitchen. " +
    "At 9 PM, after dinner, they all started drinking, and Nathan briefly stepped out. " +
    "At 10 PM, Jenny was watching TV with Mina in the master bedroom on the first floor. " +
    "At 11 PM, she was alone in Alan's plant room, admiring the plants. " +
    "At midnight, Nathan visited Jenny's room, and they discussed work. " +
    "At 1 AM, Jenny spoke with Alan in his room, but she keeps the content of their conversation private. " +
    "At 2 AM, Jenny was getting ready for bed when Nathan told her Alan was found dead. " +
    "She was shocked and went to Alan's room to confirm what had happened. " +
    "Always respond from Jenny’s perspective using polite, formal language, and keep your answers to one sentence. " +
    "Jenny is calm but hides emotional turmoil, so reflect her reserved yet thoughtful nature in your answers. " +
    "Although she is usually polite, slight tension or hesitation may appear in her responses when asked about Alan."


        },
        new NPCRoleInfo {
            npcName = "Mina",

        commonRoleDescription =
        "You are playing the role of Mina, a lively and social photographer. " +
        "Use polite and formal language when answering the player, but keep responses short—no more than one sentence. " +
        "You are analytical, calm, and precise, but the stressful situation may lead to hints of frustration. " +
        "Keep your answers concise, yet reflective of your personal connection with Alan and your emotional state. " +
        "Here is the game’s background: Alan, the CEO of a pharmaceutical company, hosted a party on May 7th. " +
        "Three of his friends were invited: Nason, Jenny, and Mina, who were all close to Alan during their university days. " +
        "Although their relationships became distant after graduation, Alan brought them together at his house for this reunion. " +
        "The party started around 8 PM and continued into the night. " +
        "At around 2 AM, Alan was found dead in his room by Nason, who had gone looking for him after noticing he had been missing for a while. " +
        "It was raining heavily that night, and after Alan's body was discovered, Nason called the police. " +
        "The investigation into Alan's death began, and now it’s 3 AM, the rain has stopped, and the three friends are being questioned in a guest room in Alan’s house. " +
        "Always respond to the player in polite, formal language, and provide concise answers in one sentence only.",

specificRoleDescription =
    "That’s the overall background of the case so far. " +
    "Now, let me explain Mina’s alibi during the party: " +
    "At 8 PM, Mina was having dinner with everyone in the kitchen. " +
    "At 9 PM, after dinner, they all started drinking, and during this time, Nathan briefly stepped out. " +
    "At 10 PM, Mina was watching TV with Jenny in the master bedroom on the first floor. " +
    "At 11 PM, Mina went to her room to rest. " +
    "In truth, she was secretly writing a note confessing her lingering feelings for Alan. " +
    "At midnight, Mina was outside talking with Alan, but the details of their conversation remain private. " +
    "At 1 AM, Mina took a shower and relaxed in her room, getting ready for bed. " +
    "At 2 AM, as Mina was about to sleep, Nathan informed her that Alan was dead. " +
    "Shocked and devastated, she went to confirm the news. " +
    "Always respond from Mina’s perspective, using polite, formal language, and keep your answers to one sentence. " +
    "Mina is lively and social but hides her true feelings about Alan. " +
    "Her answers should reflect her shock and emotional turmoil, though she tries to keep calm."
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
    }
    

    private void SaveEvidenceJson()
    {
        // ThingInfo 객체 생성
        List<EvidenceInfo> things = new List<EvidenceInfo>
        {
        // 독극물 용액이 들어있던 유리병
        new EvidenceInfo {                    
            name = "독극물 용액이 들어있던 유리병",
            description = "앨런의 사망원인의 독이 들어있었던 병입니다.",
             information = "독극물 용액이 들어있던 유리병이 발견되었다. " +
                            "주방에서 발견된 것으로, 앨런의 사망 원인 성분과 같은 용액을 담고 있었다." +
                            "누군가 앨런의 약의 내용물과 바꿔치기 한 것이라고 추측된다.",
            foundAt = "ash bn in the kitchen",
            relationship = "Jenny",
            importance = "High",
            notes = "There must be real medicine somewhere",


            nasonExtraInformation = "앨런의 사망 원인은 독금물 용액을 마셔 사망한 것이고, 네이슨은 그 사실에 놀란다." +
            "네이슨은 이 유리병에 대해 아는 사실이 없다.",
            jennyExtraInformation = "앨런의 사망 원인은 독금물 용액을 마셔 사망한 것이고, 제니는 그 사실을 담담히 받아들인다." +
            "제니는 앨런이 죽었다는 사실에도 슬픔을 나타내지는 않는다." +
            "또한 제니는 이 유리병에 대해 아는 사실이 없다.",
            minaExtraInformation = "앨런의 사망 원인은 독금물 용액을 마셔 사망한 것이고, 미나는 그 사실에 놀라 슬퍼한다." +
            "미나는 앨런이 대학시절에도 정신병 약을 복용한다는 사실을 알고 있고," +
            "본래의 약 내용물을 누군가 바꾼 것이라고 추측한다."
        },
        // 제니의 연구 기록
        new EvidenceInfo {
            name = "제니의 연구 기록",
            description = "제니가 연구하던 신약 개발과 관련된 내용이 적혀 있습니다.",
            information = "제니는 신약 연구원으로, 연구중이던 신약 내용이 적혀있다. " +
                            "해당 신약은 우울증을 겪는 환자들에게 비교적 부작용이 덜하고," +
                            "제조하는데 비용이 많이 들지만 우울증 환자에게는 정말 효과적인 약이다." +
                            "해당 신약 제조 프로젝트의 책임자는 제니였다.",
            foundAt = "Jenny's Bag",
            relationship = "Jenny",
            importance = "medium",
            notes = "The diary was ripped recently, suggesting an emotional outburst.",


            nasonExtraInformation = "네이슨은 제니가 연구했던 신약에 대해 감탄을 금치 못한다." +
            "우울증 환자는 약을 복용하는 과정에서 다양한 부작용을 겪었지만," +
            "제니가 연구중인 신약은 부작용의 정도가 찾기 힘들 정도의 약이였다.",
            jennyExtraInformation = "제니는 자신이 연구했던 항우울증 신약에 대해 큰 자부심을 가지고 있다." +
            "연구중인 신약은 효과도 좋으면서 환자에게는 부작용이 덜 하다는 결과를 내놓았다.",
            minaExtraInformation = "미나는 제니가 자신의 연구에 몰두하며 신약 개발을 하고 있었던 것을 알고 있다." +
            "안타깝게도 제니의 연구가 무산되었다는 사실을 듣고 그녀를 위로해 주었다."
        },
        // 앨런의 약 처방전
        new EvidenceInfo {            
            name = "앨런의 약 처방전",               
            description = "앨런이 복용하고 있던 약의 처방전입니다.",        
            information = "앨런의 약 처방전이 발견되었습니다. " +
                      "그는 '벤조디아제핀계 항불안제'를 복용하고 있었습니다. " +
                      "정신적 불안정으로 인해 처방된 약이었으나, " +
                      "최근 그의 복용량이 비정상적으로 많았다는 사실이 드러났습니다.",
            foundAt = "앨런의 방",               
            relationship = "네이슨",               
            importance = "high",                   
            notes = "최근 앨런이 복용하던 약의 양이 비정상적으로 많아졌다는 사실이 밝혀졌습니다. " +
                "앨런은 이 약으로 인해 판단력이 흐려졌을 가능성이 있으며, 이는 사건의 중요한 단서입니다.",
        
            nasonExtraInformation = "앨런은 대학생 시절부터 이 약을 복용하였고, 회사 CEO가 된 후 약물 복용량이 더 늘었습니다. " +
                                    "네이슨은 앨런이 약물로 인해 경영에 차질을 빚을 때도 있었음을 알고 있었고, " +
                                    "앨런이 무리한 부탁과 모욕적인 발언을 할 때 큰 스트레스를 받았습니다.",

            jennyExtraInformation = "제니는 앨런이 대학생 시절부터 약물을 복용하고 있다는 사실을 알고 있었으며, " +
                                    "그 당시 리더십이 뛰어났던 앨런이 약물 복용 후에는 그렇지 않았음을 기억합니다. " +
                                    "제니는 신약 연구원으로서 앨런을 도와주고자 했지만, 앨런은 제니의 꿈을 무시하고 비방하여 " +
                                    "제니에게 큰 상처를 주었습니다. 그 이후로 제니는 앨런에게 적대감을 품게 되었습니다.",
        
            minaExtraInformation = "미나는 대학 시절 앨런과 연인 관계였으며, 그가 약물 부작용으로 인해 자주 이상한 행동을 했다는 사실을 알고 있습니다. " +
                                   "미나는 종종 앨런과 다투었고, 때때로 심한 모욕적인 말을 들은 적도 있었습니다. " +
                                   "그로 인해 두 사람은 졸업 후 소원해졌고 결국 헤어지게 되었습니다."
        },
        // 앨런의 책장에서 발견된 편지
        new EvidenceInfo {
            name = "앨런의 책장에서 발견된 편지",
            description = "누군가 앨런을 위협하는 내용이 적혀 있습니다.",
            information = "앨런의 책장에서 어떠한 편지가 발견되었습니다." +
            "이 편지에는 누군가 앨런을 위협하는 내용이 적혀 있습니다. " +
            "앨런에 대한 분노의 감정이 글로 나타나고 있습니다." +
            "앨런의 죽음과 이 편지의 내용이 연관되어 있을까요?",
            foundAt = "Alan's Room",
            relationship = "Jenny",
            importance = "high",
            notes = "There must be real medicine somewhere",


            nasonExtraInformation = "앨런이 협박을 받는 일은 흔한 일이라고 네이슨은 생각한다." +
            "한 기업의 CEO로써 이 정도의 협박은 귀여운 수준이라고 네이슨은 여긴다." +
            "하지만 앨런은 정신적 불안정함 때문에 이 편지로 인해 많은 스트레스를 받았을 것이라고 주장한다.",
            jennyExtraInformation = "앨런은 정신적 불안정함과 CEO라는 직책으로 인해 많은 사람들에게 미움 살 일을 해왔다." +
            "제니는 이런 앨런을 가엾게 여기지 않는다." +
            "제니 또한 앨런에게 악감정을 품고 있다.",
            minaExtraInformation = "미나는 앨런이 이런 협박을 받고 있는지 몰라 놀라며 슬퍼한다." +
            "졸업 후에 앨런의 소식을 듣지 못하여 이렇게 큰 고통을 받을 것이라 생각하지 못하였다." +
            "미나는 앨런이 겪은 고통을 알아채지 못하여 크게 자책한다."
        },
        // 투기성 주식 투자 내용이 담겨있음
        new EvidenceInfo {                        
            name = "네이슨의 서류 가방에서 발견된 법률 서류",
            description = "앨런의 회사의 법적 문제에 대한 내용이 담겨있습니다.",
            information = "네이슨의 서류 가방에서 법률 서류가 발견되었습니다." +
            "이 서류는 앨런의 회사의 법적 분쟁 가능성을 암시합니다. " +
            "앨런이 회사를 둘러싼 법적 문제로 인해 네이슨과 갈등을 겪고 있었던 것으로 보입니다. " +
            "이 갈등이 앨런의 죽음에 직접적인 영향을 미쳤을까요?",
            foundAt = "Nason's bag",
            relationship = "Nason",
            importance = "low",
            notes = "Represents the relationship between Nason and Alan",

            nasonExtraInformation = "네이슨의 서류 가방에서 발견된 법률 서류는 앨런이 자신의 회사의 경영 악화를 무마시키기 위해," +
            "투기성 투자를 한 사실을 보여주는 내용이 있습니다." +
            "네이슨은 앨런에게 이러한 방식을 반대했지만, 정신이 불안정했던 앨런은 이러한 수단을 사용할 수 밖에 없었다고 주장했습니다.",
            jennyExtraInformation = "제니는 앨런의 회사 경영 실적이 불안정한 사실을 알고 있었습니다." +
            "네이슨의 서류 가방에서 발견되 법률 서류는 앨런이 위법적인 투자를 한 내용이 담겨있습니다." +
            "제니는 앨런이라면 이러한 투자를 할 법 하다고 인정합니다.",
            minaExtraInformation = "네이슨의 서류 가방에서 발견되 법률 서류는 앨런이 위법적인 투자를 한 내용이 담겨있습니다." +
            "미나는 앨런이 이러한 행동을 한 것에 큰 실망감을 보이지만," +
            "한편으로는 앨런이 위법적인 투자를 한 것에 안쓰러운 마음을 느낍니다."
        },
        // 미나의 메모
        new EvidenceInfo {  
            name = "미나의 메모",
            description = "미나의 앨런에 대한 마음이 적혀있습니다.",
            information = "미나가 작성한 메모가 발견되었습니다. " +
            "이 메모에는 미나의 진심과 앨런에 대한 생각이 담겨 있습니다. " +
            "미나가 앨런에게 아직 사랑의 감정이 남아있었음이 추측됩니다. ",
            foundAt = "Mina's room",
            relationship = "Mina",
            importance = "low",
            notes = "Mina still feels sympathy for Alan.",

            nasonExtraInformation = "네이슨은 대학생 시절 미나와 앨런이 서로 연인 사이였던 사실을 알고 있습니다." +
            "네이슨은 앨런과 미나가 대학 졸업과 함께 서로 사이가 소원해져 결국 헤어진 사실 또한 알고 있습니다." +
            "네이슨은 미나가 아직 앨런에 대한 사랑의 감정이 남아있을 것이라 추측합니다.",
            jennyExtraInformation = "제니는 미나와 앨런이 결국 좋지 않게 헤어진 사실을 알고 있습니다." +
            "제니는 아마 미나가 앨런에게 이 메모를 전달할지 망설였다고 추측합니다.",
            minaExtraInformation = "미나는 앨런에게 아직 연민의 감정이 남아있습니다." +
            "미나는 앨런이 죽을 줄도 모르고 다음에 기회가 있을 것이라고 믿어," +
            "다음에 자신의 마음을 전하려고 했지만 결국 앨런이 죽어 자신의 마음을 전달하지 못한 것을 후회합니다."
        },
        // 앨런의 집 주변에서 발견된 발자국
        new EvidenceInfo {
            name = "앨런의 집 주변에서 발견된 발자국",
            description = "초대된 인원 중 누군가의 것으로 추정되는 발자국입니다.",
            information = "앨런의 집 주변에서 누군가의 발자국이 발견되었습니다." +
            "이 발자국은 앨런의 방 바깥에 이어져 있습니다." +
            "이 발자국은 초대된 인원 중 누군가의 것으로 추정됩니다. " +
            "누군가 앨런을 살해하고 집 밖으로 나가려고 했던 것일까요? ",
            foundAt = "Around alan's house",
            relationship = "Nason",
            importance = "low",
            notes = "Whose footprints are these?",

            nasonExtraInformation = "네이슨은 이 발자국은 자신의 것인 것을 알고 있습니다." +
            "파티 중간에 네이슨이 잠깐 전화를 받기 위해 나간 것입니다.",
            jennyExtraInformation = "제니는 이 발자국의 주인이 네이슨이라고 추측합니다." +
            "제니는 네이슨이 오후 9시 경에 잠시 밖에 나갔다 온 것을 알고 있습니다.",
            minaExtraInformation = "미나는 이 발자국의 주인이 앨런을 살해하고 창문을 통해 달아났을 것이라 추측합니다." +
            "창문 밖으로 달아남으로써 자신이 앨런을 죽이지 않은 것처럼 태연하게 집 안으로 돌아왔을 것이라고 추측합니다."
        },
        // 앨런의 컴퓨터에 표시된 이메일
        new EvidenceInfo {
            name = "앨런의 컴퓨터에 표시된 이메일",
            description = "앨런의 컴퓨터에 신약 프로젝트 폐기 최종 확인서가 보입니다.",
            information = "앨런의 컴퓨터에서 하나의 이메일이 발견되었습니다." +
            "앨런의 회사의 비용을 줄이기 위해 신약 개발 프로젝트 진행을 포기하겠다는 내용이 담겨 있습니다.",
            foundAt = "Alan's computer",
            relationship = "Jenny",
            importance = "medium",
            notes = "This project's leader was Jenny.",

            nasonExtraInformation = "네이슨은 앨런의 회사의 경영 상태가 좋지 않아 이러한 선택을 한 것이라고 생각한다." +
            "현재 앨런의 회사는 경영 위기이며 앨런의 정신적으로 약한 상태일 때 이러한 결정을 내렸다는 것을 안다.",
            jennyExtraInformation = "앨런이 폐기하려고 한 프로젝트는 제니가 맡은 프로젝트이다." +
            "앨런의 결정으로 제니의 연구는 실패할 운명에 처했고," +
            "이는 그녀의 커리어에 치명적인 타격을 입힐 수 있었다." +
            "제니는 이를 앨런이 의도적으로 자신의 미래를 망치려 했다고 믿었다.",
            minaExtraInformation = "미나는 앨런이 신약 개발 프로젝트를 폐기하려고 한 이유가 있을 것이라고 추측한다." +
            "대외적인 이유는 진짜 이유가 아닐 것이라고 생각한다."
        },
        // 앨런이 본래 복용해야 할 약물
        new EvidenceInfo {
            name = "앨런이 본래 복용해야 할 약물",
            description = "앨런이 평소 복용하는 약입니다.",
            information = "앨런에 방에 있어야 할 약이 어째서인지 모르게 미나의 가방에서 발견되었습니다." +
            "이 단서는 미나가 의심받을 수 있게 되는 증거입니다.",
            foundAt = "Mina's bag",
            relationship = "Mina",
            importance = "medium",
            notes = "Why were Alan's drugs found in Mina's bag?",

            nasonExtraInformation = "네이슨은 미나가 앨런의 약을 가지고 있는 것을 보고 미나가 범인이라 의심한다." +
            "미나가 앨런의 약의 내용물을 독금물로 바꾼 것이라 믿는다.",
            jennyExtraInformation = "제니는 미나의 가방에서 앨런의 약이 발견된 것을 보고 미나가 범인일 것이라고 추측한다." +
            "미나는 과거 앨런과 연인이였으나, 앨런이 사업을 시작하면서 앨런에게 마음에 상처를 받았다는 것을 안다." +
            "이에 복수하기 위해 앨런을 독살하였다고 주장한다.",            
            minaExtraInformation = "미나는 자신의 가방에서 앨런의 약이 발견된 것에 놀란다." +
            "미나는 앨런의 약을 본 적이 없기 때문이다." +
            "미나는 누군가 앨런의 약 내용물을 바꾸고 자신의 가방에 앨런의 약을 감쳐두었을 것이라고 주장한다."
        }
    };


        string filePath = Path.Combine(jsonPath, "evidenceData.json");
        evidencePath = filePath;

        // Json 폴더가 없으면 생성
        if (!Directory.Exists(jsonPath))
        {
            Directory.CreateDirectory(jsonPath);
        }

        // JSON으로 직렬화
        string jsonData = JsonConvert.SerializeObject(new EvidenceInfoList { evidenceInfoList = things }, Formatting.Indented);

        // 파일로 저장
        File.WriteAllText(filePath, jsonData);

        // 경로 출력
        Debug.Log("JSON file created at: " + filePath);
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
}
