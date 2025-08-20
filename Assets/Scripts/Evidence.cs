using Unity.VisualScripting;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.PropertyVariants;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public class Evidence : MonoBehaviour
{
    private bool isFound = false;

    private string thisName;
    private string description;
    private string information;
    private string foundAt;
    private string notes;

    [SerializeField]
    private EvidenceManager evidenceManager;    

    /*
    public enum EvidenceName
    {
        Glass_Bottle_Containing_Poison,
        Research_Records_Of_Jenny,
        Prescription_Of_Alan,
        Letter_Found_Under_the_Curtain,
        Legal_Documents_Found_in_Briefcase_Of_Nason,
        Memo_Of_Mina,
        Footprints_Found_Around_House_Of_Alan,
        Email_Displayed_on_Computer_Of_Alan,
        Original_Medication_Of_Alan,
        Damaged_Plant,
        Partial_Management_Report,

        毒物が入っていたガラス瓶,
        ジェニーの研究記録,
        アランの処方箋,
        カーテンの下で見つかった手紙,
        ネイソンのブリーフケースで見つかった法的書類,
        ミナのメモ,
        アランの家の周辺で発見された足跡,
        アランのコンピューターに表示されたメール,
        アランが本来服用すべき薬,
        傷つけられた植物,
        経営報告書の一部,

        독극물_용액이_들어있던_유리병,
        제니의_연구_기록,
        앨런의_약_처방전,
        커튼_밑에서_발견된_편지,
        네이슨의_서류_가방에서_발견된_법률_서류,
        미나의_메모,
        앨런의_집_주변에서_발견된_발자국,
        앨런의_컴퓨터에_표시된_이메일,
        앨런이_본래_복용해야_할_약물,
        손상된_식물,
        경영_보고서_일부
    }*/
    

    private string evidenceName;

    void Awake()
    {
        SetName();
        StartCoroutine(DelayedSetEvidence());
    }

    private IEnumerator DelayedSetEvidence()
    {
        yield return new WaitForSeconds(2f);        
        evidenceManager = GameObject.Find("EvidenceManager").GetComponent<EvidenceManager>();
        evidenceManager.SetEvidence(this);
    }

    public void Initialize(string name, string description, string information, string foundAt, string notes)
    {
        this.thisName = name;
        this.description = description;
        this.information = information;
        this.foundAt = foundAt;
        this.notes = notes;
    }



    public bool GetIsFound()
    {
        return isFound;
    }

    public void SetIsFoundTrue()
    {
        isFound = true;
    }

    public void GetEvidence()
    {        
        evidenceManager.FindEvidence(this);
    }



    private void SetName()
    {
        // 1. 오브젝트의 tag를 가져옴
        string tagName = this.tag;

        // 2. 현재 언어 코드 가져오기 (Unity Localization 사용)
        string localeCode = LocalizationSettings.SelectedLocale.Identifier.Code;

        // 3. 언어 코드에 따라 EvidenceName 매칭
        switch (localeCode)
        {
            case "en":
                switch (tagName)
                {
                    case "Poison": evidenceName = "Glass Bottle Containing Poison"; break;
                    case "JennyRecord": evidenceName = "Research Records Of Jenny"; break;
                    case "Prescription": evidenceName = "Prescription Of Alan"; break;
                    case "Letter": evidenceName = "Letter Found Under the Curtain"; break;
                    case "LegalDocuments": evidenceName = "Legal Documents Found in Briefcase Of Nason"; break;
                    case "MinaMemo": evidenceName = "Memo Of Mina"; break;
                    case "Footprints": evidenceName = "Footprints Found Around House Of Alan"; break;
                    case "Email": evidenceName = "Email Displayed on Computer Of Alan"; break;
                    case "Medication": evidenceName = "Original Medication Of Alan"; break;
                    case "Plant": evidenceName = "Damaged Plant"; break;
                    case "ManagementReport": evidenceName = "Partial Management Report"; break;
                }
                break;
            case "ja":
                switch (tagName)
                {
                    case "Poison": evidenceName = "毒物が入っていたガラス瓶"; break;
                    case "JennyRecord": evidenceName = "ジェニーの研究記録"; break;
                    case "Prescription": evidenceName = "アランの処方箋"; break;
                    case "Letter": evidenceName = "カーテンの下で見つかった手紙"; break;
                    case "LegalDocuments": evidenceName = "ネイソンのブリーフケースで見つかった法的書類"; break;
                    case "MinaMemo": evidenceName = "ミナのメモ"; break;
                    case "Footprints": evidenceName = "アランの家の周辺で発見された足跡"; break;
                    case "Email": evidenceName = "アランのコンピューターに表示されたメール"; break;
                    case "Medication": evidenceName = "アランが本来服用すべき薬"; break;
                    case "Plant": evidenceName = "傷つけられた植物"; break;
                    case "ManagementReport": evidenceName = "経営報告書の一部"; break;                    
                }
                break;
            case "ko":
                switch (tagName)
                {
                    case "Poison": evidenceName = "독극물 용액이 들어있던 유리병"; break;
                    case "JennyRecord": evidenceName = "제니의 연구 기록"; break;
                    case "Prescription": evidenceName = "앨런의 약 처방전"; break;
                    case "Letter": evidenceName = "커튼 밑에서 발견된 편지"; break;
                    case "LegalDocuments": evidenceName = "네이슨의 서류 가방에서 발견된 법률 서류"; break;
                    case "MinaMemo": evidenceName = "미나의 메모"; break;
                    case "Footprints": evidenceName = "앨런의 집 주변에서 발견된 발자국"; break;
                    case "Email": evidenceName = "앨런의 컴퓨터에 표시된 이메일"; break;
                    case "Medication": evidenceName = "앨런이 본래 복용해야 할 약물"; break;
                    case "Plant": evidenceName = "손상된 식물"; break;
                    case "ManagementReport": evidenceName = "경영 보고서 일부"; break;
                }
                break;
            default:
                // 기본값: 영어로 처리
                goto case "en";
        }
    }


    public string GetName()
    {
        return this.evidenceName;
    }

    public string GetDescription()
    {
        return this.description;
    }

    public string GetInformation()
    {
        return this.information;
    }

    public string GetFoundAt()
    {
        return foundAt;
    }

    public string GetNotets()
    {
        return this.notes;
    }
}