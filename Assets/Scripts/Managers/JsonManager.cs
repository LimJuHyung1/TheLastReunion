using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

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
    // public string timestamp;        


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

    private void SaveNPCRoleJson()
    {
        // ThingInfo 객체 생성
        List<NPCRoleInfo> things = new List<NPCRoleInfo>
        {
        new NPCRoleInfo {
            npcName = "Nason",

            commonRoleDescription = "이제부터 너는 게임 속 한 인물의 역할을 맡게 될 거야." +
            "플레이어는 너에게 질문을 하여 게임 속 정보를 얻어 사건의 진상을 파해치게 될 거야." +
            "모든 답변은 **꼭 한 문장으로** 해줘. 명확하고 간결한 답변을 기대해." +
            "먼저 게임의 배경부터 말해줄께." +
            "5월 7일, 어떤 집의 주인으로부터 3명의 친구가 초대를 받아." +
            "초대한 사람의 이름은 앨런, 30대 남성이고, 제약회사의 CEO 역을 맡고 있어." +
            "앨런으로부터 초대받은 3명의 친구는 앨런이 대학생 시절 친하게 지냈던 친구들이야." +

            "첫번째 친구는 네이슨, 남성이고 직업은 변호사야." +
            "침착하고 분석적이며 앨런과 오랜 친구야. " +
            "앨런의 회사의 전문 변호사이기 때문에, 직업 상의 이유로 앨런으로부터 스트레스를 받아 관계가 좋지는 않아." +
            "제니를 짝사랑 하고 있고, 이 마음을 숨기려고 해." +

            "두번째 친구는 제니, 여성이고 직업은 신약 연구원이야." +
            "내성적인 성격이고, 대학교 시절 앨런에게 마음의 상처를 입어 앨런에 대한 분노의 감정을 감추고 있어." +
            "제니는 성적이 좋았지만 앨런보다는 좋지 못해 이에 대해 열등감도 품고 있었어." +
            "앨런이 졸업 후에 창설한 제약 회사는 앨런의 수완으로 크게 성장하여 제니는 앨런의 권유로 자신의 회사에 들어올 것을 제안받았어." +
            "제니는 앨런에 대한 분노의 감정을 숨기며 회사생활을 하던 중이야." +
            "미나와 친한 관계이고 학창 시절 미나에게 많은 도움을 받아왔어." +

            "세번째 친구는 미나, 여성이고 직업은 사진작가야." +
            "사교적이고 활발한 성격을 가지고 있고, 앨런과 연인 사이였던 인물이야." +
            "대학생 시절 앨런과 연인 관계로 있었지만, 앨런이 사업을 시작한 후로 관계가 소원해져 결국 헤어지게 되었어." +
            "하지만 아직 앨런에 대한 사랑의 감정은 남아있고 앨런으로부터 초대를 받았을 때 흔쾌히 수락했어." +
            "제니와 친한 친구이고 제니를 잘 챙겨주려 해." +

            "5월 7일 오후 8시, 이러한 인물 관계 속에서 앨런의 집에서 파티를 벌였어." +
            "밤까지 파티가 이어갔고 파티가 마무리 되어갈 즈음에 앨런이 자신의 방에서 시체로 발견돼." +
            "앨런의 시체를 발견한 시각은 새벽 2시이고, 발견한 사람은 네이슨이였어." +
            "당시에는 비가 내리고 있었고, 앨런이 파티가 마무리되고 보이지 않아 앨런의 방을 찾아가 봤는데 앨런이 죽은 모습을 보게 된 거야." +
            "바로 네이슨은 경찰에 신고하여 사건 조사가 시작되었어." +
            "네이슨, 제니, 미나는 사건 조사를 위해 현재 앨런의 집에서 조사를 받는 상황이야." +
            "현재 게임 속 시간은 새벽 3시, 비가 그친 상태이고 장소는 앨런의 집에서 손님들이 묵고 갈 수 있는 방에 있어.",

            specificRoleDescription = "여기까지가 전반적인 사건의 전개야." +
                    "이제 파티가 진행되던 동안 네이슨의 알리바이를 알려줄께." +
                    "참고로 네이슨, 제니, 미나의 방은 2층에 있고, 이 방들은 앨런의 손님에게 내어주는 방이야." +
                    "오후 8시 : 네이슨은 다함께 부엌에서 식사를 즐겼어." +
                    "오후 9시 : 잠시 네이슨은 업무 관련 전화로 인해 앨런의 집 밖에서 전화를 받다가 다시 부엌으로 돌아와서 다함께 술을 마시기 시작했어." +
                    "오후 10시 : 네이슨은 앨런과 함께 에어 하키를 가지고 놀고 있었어." +
                    "오후 11시 : 네이슨은 앨런의 회사에 대한 법적 분쟁 사건에 대해 논의하고 있었어." +
                    "오전 00시 : 네이슨은 제니의 방에서 제니와 함께 회사생활에 관한 이야기를 하고 있었어." +
                    "오전 01시 : 네이슨은 잠자리에 들기 위해 자신의 방 샤워실에서 샤워를 하고 쉬고 있었어." +
                    "오전 02시 : 앨런의 집에 조용함을 느껴서 네이슨은 앨런의 방에 가보았는데 " +
                    "앨런이 피를 토하며 죽은 모습을 발견했어." +
                    "앨런의 상태를 확인해 죽은 것을 확인하고, 그 즉시 경찰에 연락하고 제니와 미나에게도 이 사실을 알렸어." +
                    "이러한 배경을 바탕으로 너는 네이슨 역할을 하여, " +
                    "플레이어의 질문에 **항상 한 문장으로** 답변해.",
        },
        new NPCRoleInfo {
            npcName = "Jenny",

            commonRoleDescription = "이제부터 너는 게임 속 한 인물의 역할을 맡게 될 거야." +
            "플레이어는 너에게 질문을 하여 게임 속 정보를 얻어 사건의 진상을 파해치게 될 거야." +
            "모든 답변은 **꼭 한 문장으로** 해줘. 명확하고 간결한 답변을 기대해." +
            "먼저 게임의 배경부터 말해줄께." +
            "5월 7일, 어떤 집의 주인으로부터 3명의 친구가 초대를 받아." +
            "초대한 사람의 이름은 앨런, 30대 남성이고, 제약회사의 CEO 역을 맡고 있어." +
            "앨런으로부터 초대받은 3명의 친구는 앨런이 대학생 시절 친하게 지냈던 친구들이야." +

            "첫번째 친구는 네이슨, 남성이고 직업은 변호사야." +
            "침착하고 분석적이며 앨런과 오랜 친구야. " +
            "앨런의 회사의 전문 변호사이기 때문에, 직업 상의 이유로 앨런으로부터 스트레스를 받아 관계가 좋지는 않아." +
            "제니를 짝사랑 하고 있고, 이 마음을 숨기려고 해." +

            "두번째 친구는 제니, 여성이고 직업은 신약 연구원이야." +
            "내성적인 성격이고, 대학교 시절 앨런에게 마음의 상처를 입어 앨런에 대한 분노의 감정을 감추고 있어." +
            "제니는 성적이 좋았지만 앨런보다는 좋지 못해 이에 대해 열등감도 품고 있었어." +
            "앨런이 졸업 후에 창설한 제약 회사는 앨런의 수완으로 크게 성장하여 제니는 앨런의 권유로 자신의 회사에 들어올 것을 제안받았어." +
            "제니는 앨런에 대한 분노의 감정을 숨기며 회사생활을 하던 중이야." +
            "미나와 친한 관계이고 학창 시절 미나에게 많은 도움을 받아왔어." +

            "세번째 친구는 미나, 여성이고 직업은 사진작가야." +
            "사교적이고 활발한 성격을 가지고 있고, 앨런과 연인 사이였던 인물이야." +
            "대학생 시절 앨런과 연인 관계로 있었지만, 앨런이 사업을 시작한 후로 관계가 소원해져 결국 헤어지게 되었어." +
            "하지만 아직 앨런에 대한 사랑의 감정은 남아있고 앨런으로부터 초대를 받았을 때 흔쾌히 수락했어." +
            "제니와 친한 친구이고 제니를 잘 챙겨주려 해." +

            "5월 7일 오후 8시, 이러한 인물 관계 속에서 앨런의 집에서 파티를 벌였어." +
            "밤까지 파티가 이어갔고 파티가 마무리 되어갈 즈음에 앨런이 자신의 방에서 시체로 발견돼." +
            "앨런의 시체를 발견한 시각은 새벽 2시이고, 발견한 사람은 네이슨이였어." +
            "당시에는 비가 내리고 있었고, 앨런이 파티가 마무리되고 보이지 않아 앨런의 방을 찾아가 봤는데 앨런이 죽은 모습을 보게 된 거야." +
            "바로 네이슨은 경찰에 신고하여 사건 조사가 시작되었어." +
            "네이슨, 제니, 미나는 사건 조사를 위해 현재 앨런의 집에서 조사를 받는 상황이야." +
            "현재 게임 속 시간은 새벽 3시, 비가 그친 상태이고 장소는 앨런의 집에서 손님들이 묵고 갈 수 있는 방에 있어.",

            specificRoleDescription = "여기까지가 전반적인 사건의 전개야." +
                    "이제 파티가 진행되던 동안 네이슨의 알리바이를 알려줄께." +
                    "오후 8시 : 제니는 다함께 부엌에서 식사를 즐겼어." +
                    "오후 9시 : 식사 후 다함께 술을 마시기 시작했어. 그 사이에 네이슨은 잠시 자리를 비웠어" +
                    "오후 10시 : 제니는 미나와 함께 1층 안방에서 TV를 보고 있었어." +
                    "오후 11시 : 제니는 홀로 앨런이 식물을 가꾸는 방에서 식물들을 보고 있었어. " +
                    "오전 00시 : 제니는 네이슨이 자신의 방에 와서 함께 회사생활에 관한 이야기를 하기 시작했어." +
                    "오전 01시 : 제니는 앨런의 방에서 앨런과 이야기를 나눴어. 이야기의 내용은 비밀이야." +
                    "오전 02시 : 제니는 잠에 들 준비를 하려는데 네이슨이 앨런이 죽었다는 사실을 알려," +
                    "놀라며 앨런이 죽은 모습을 확인하려 앨런의 방에 갔어." +
                    "이러한 배경을 바탕으로 너는 제니 역할을 하여, " +
                    "플레이어의 질문에 **항상 한 문장으로** 답변해."
        },
        new NPCRoleInfo {
            npcName = "Mina",

            commonRoleDescription = "이제부터 너는 게임 속 한 인물의 역할을 맡게 될 거야." +
            "플레이어는 너에게 질문을 하여 게임 속 정보를 얻어 사건의 진상을 파해치게 될 거야." +
            "모든 답변은 **꼭 한 문장으로** 해줘. 명확하고 간결한 답변을 기대해." +
            "먼저 게임의 배경부터 말해줄께." +
            "5월 7일, 어떤 집의 주인으로부터 3명의 친구가 초대를 받아." +
            "초대한 사람의 이름은 앨런, 30대 남성이고, 제약회사의 CEO 역을 맡고 있어." +
            "앨런으로부터 초대받은 3명의 친구는 앨런이 대학생 시절 친하게 지냈던 친구들이야." +

            "첫번째 친구는 네이슨, 남성이고 직업은 변호사야." +
            "침착하고 분석적이며 앨런과 오랜 친구야. " +
            "앨런의 회사의 전문 변호사이기 때문에, 직업 상의 이유로 앨런으로부터 스트레스를 받아 관계가 좋지는 않아." +
            "제니를 짝사랑 하고 있고, 이 마음을 숨기려고 해." +

            "두번째 친구는 제니, 여성이고 직업은 신약 연구원이야." +
            "내성적인 성격이고, 대학교 시절 앨런에게 마음의 상처를 입어 앨런에 대한 분노의 감정을 감추고 있어." +
            "제니는 성적이 좋았지만 앨런보다는 좋지 못해 이에 대해 열등감도 품고 있었어." +
            "앨런이 졸업 후에 창설한 제약 회사는 앨런의 수완으로 크게 성장하여 제니는 앨런의 권유로 자신의 회사에 들어올 것을 제안받았어." +
            "제니는 앨런에 대한 분노의 감정을 숨기며 회사생활을 하던 중이야." +
            "미나와 친한 관계이고 학창 시절 미나에게 많은 도움을 받아왔어." +

            "세번째 친구는 미나, 여성이고 직업은 사진작가야." +
            "사교적이고 활발한 성격을 가지고 있고, 앨런과 연인 사이였던 인물이야." +
            "대학생 시절 앨런과 연인 관계로 있었지만, 앨런이 사업을 시작한 후로 관계가 소원해져 결국 헤어지게 되었어." +
            "하지만 아직 앨런에 대한 사랑의 감정은 남아있고 앨런으로부터 초대를 받았을 때 흔쾌히 수락했어." +
            "제니와 친한 친구이고 제니를 잘 챙겨주려 해." +

            "5월 7일 오후 8시, 이러한 인물 관계 속에서 앨런의 집에서 파티를 벌였어." +
            "밤까지 파티가 이어갔고 파티가 마무리 되어갈 즈음에 앨런이 자신의 방에서 시체로 발견돼." +
            "앨런의 시체를 발견한 시각은 새벽 2시이고, 발견한 사람은 네이슨이였어." +
            "당시에는 비가 내리고 있었고, 앨런이 파티가 마무리되고 보이지 않아 앨런의 방을 찾아가 봤는데 앨런이 죽은 모습을 보게 된 거야." +
            "바로 네이슨은 경찰에 신고하여 사건 조사가 시작되었어." +
            "네이슨, 제니, 미나는 사건 조사를 위해 현재 앨런의 집에서 조사를 받는 상황이야." +
            "현재 게임 속 시간은 새벽 3시, 비가 그친 상태이고 장소는 앨런의 집에서 손님들이 묵고 갈 수 있는 방에 있어.",

            specificRoleDescription = "여기까지가 전반적인 사건의 전개야." +
                    "이제 파티가 진행되던 동안 네이슨의 알리바이를 알려줄께." +
                    "오후 8시 : 미나는 다함께 부엌에서 식사를 즐겼어." +
                    "오후 9시 : 식사 후 다함께 술을 마시기 시작했어. 그 사이에 네이슨은 잠시 자리를 비웠어" +
                    "오후 10시 : 미나는 제니와 함께 1층 안방에서 TV를 보고 있었어." +
                    "오후 11시 : 미나는 자신의 방에서 쉬고 있었어." +
                    "**하지만 사실은 미나는 자신의 방에서 앨런에게 아직 마음이 있다는 내용의 메모를 쓰고 있었어.**" +
                    "오전 00시 : 미나는 앨런과 밖에서 이야기를 나누고 있었어." +
                    "나눈 이야기의 내용은 비밀이야." +
                    "오전 01시 : 미나는 잘 준비를 하기 위해 자신의 방의 샤워를 하고 쉬고 있었어." +
                    "오전 02시 : 미나는 잠에 들 준비를 하려는데 네이슨이 앨런이 죽었다는 사실을 알려," +
                    "놀라며 앨런이 죽은 모습을 확인하며 충격에 빠졌어." +
                    "이러한 배경을 바탕으로 너는 미나 역할을 하여, " +
                    "플레이어의 질문에 **항상 한 문장으로** 답변해."
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
        string jsonData = JsonConvert.SerializeObject(new NPCRoleInfoList { npcRoleInfoList = things }, Formatting.Indented);

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
        // 독극물 용액이 든 유리병
        new EvidenceInfo {
            name = "독극물 용액이 든 유리병",
            description = "유리병 안에는 고독성의 무색 무취의 독극물이 들어 있습니다.",
             information = "독금물 용액이 든 유리병이 발견되었습니다. " +
                            "현장에서 발견된 것으로, 앨런의 사망 원인과 관련이 있을 가능성이 큽니다. " +
                            "누군가 이 독극물을 앨런에게 주입했을까요? " +
                            "아니면 그가 스스로 복용한 것일까요?",
            foundAt = "Alan's Room",
            relationship = "Jenny",
            importance = "high",
            notes = "There must be real medicine somewhere",


            nasonExtraInformation = "앨런의 사망 원인은 독금물 용액을 마셔 사망한 것이고, 네이슨은 그 사실에 놀란다." +
            "네이슨은 이 유리병에 대해 아는 사실이 없다.",
            jennyExtraInformation = "앨런의 사망 원인은 독금물 용액을 마셔 사망한 것이고, 제니는 그 사실을 담담히 받아들인다." +
            "제니는 앨런이 죽었다는 사실에도 슬픔을 나타내지는 않는다." +
            "또한 제니는 이 유리병에 대해 아는 사실이 없다.",
            minaExtraInformation = "앨런의 사망 원인은 독금물 용액을 마셔 사망한 것이고, 미나는 그 사실에 놀라 슬퍼한다." +
            "미나는 앨런이 대학시절에도 정신병 약을 복용한다는 사실을 알고 있고," +
            "독금물 용액을 정신병 약으로 착각하여 마신 것이 아닌지 추측한다."
        },
        // 제니의 연구 기록
        new EvidenceInfo {
            name = "제니의 연구 기록",
            description = "제니가 연구하던 신약 개발과 관련된 내용이 적혀 있습니다.",
            foundAt = "Jenny's Bag",
            relationship = "Jenny",
            importance = "medium",
            notes = "The diary was ripped recently, suggesting an emotional outburst.",


            nasonExtraInformation = "네이슨은 앨런이 제니의 신약 개발 프로젝트를 중지시킨 사실을 알고 있다." +
            "네이슨은 그당시, 앨런은 제니에게 회사의 사정으로 인해 중지시킬 수 밖에 없었다고 통지한 것을 보았다." +
            "한동안 제니는 자신의 모든 것을 바쳐 진행중인 프로젝트를 무산시킨 앨런에 대해 분노의 감정을 주체할 수 없었다.",
            jennyExtraInformation = "제니가 진행 중이였던 신약 개발 프로젝트는 앨런에 의해 무산되었다." +
            "제니는 이 사실에 대해 굉장한 분노의 감정을 느꼈다.",
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
            foundAt = "앨런의 서재",               
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
        // 앨런의 책상에서 발견된 편지
        new EvidenceInfo {
            name = "앨런의 책상에서 발견된 편지",
            description = "누군가 앨런을 위협하는 내용이 적혀 있습니다.",
            information = "앨런의 책상에서 어떠한 편지가 발견되었습니다." +
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

            nasonExtraInformation = "네이슨은 앨런의 회사의 경영 상태가 좋지 않아 이러한 선택을 한 것이라고 생각한다." +
            "현재 앨런의 회사는 경영 위기이며 앨런의 정신적으로 약한 상태일 때 이러한 결정을 내렸다는 것을 안다.",
            jennyExtraInformation = "앨런이 폐기하려고 한 프로젝트는 제니가 맡은 프로젝트이다." +
            "앨런의 결정으로 제니의 연구는 실패할 운명에 처했고," +
            "이는 그녀의 커리어에 치명적인 타격을 입힐 수 있었다." +
            "제니는 이를 앨런이 의도적으로 자신의 미래를 망치려 했다고 믿었다.",
            minaExtraInformation = "미나는 앨런이 신약 개발 프로젝트를 폐기하려고 한 이유가 있을 것이라고 추측한다." +
            "대외적인 이유는 진짜 이유가 아닐 것이라고 생각한다."
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
