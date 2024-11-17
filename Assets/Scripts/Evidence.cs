using Unity.VisualScripting;
using UnityEngine;

public class Evidence : MonoBehaviour
{
    private bool isFound = false;

    private string thisName;
    private string description;
    private string information;
    private string foundAt;
    private string relationship;
    private string notes;

    [SerializeField]
    private EvidenceManager evidenceManager;

    void Awake()
    {
        evidenceManager = GameObject.Find("EvidenceManager").GetComponent<EvidenceManager>();
        evidenceManager.SetEvidence(this);
    }

    public enum EvidenceName
    {
        독극물_용액이_들어있던_유리병,
        제니의_연구_기록,
        앨런의_약_처방전,
        앨런의_책장에서_발견된_편지,
        네이슨의_서류_가방에서_발견된_법률_서류,
        미나의_메모,
        앨런의_집_주변에서_발견된_발자국,
        앨런의_컴퓨터에_표시된_이메일,
        앨런이_본래_복용해야_할_약물,
        손상된_식물,
        경영_보고서_일부
    }

    public EvidenceName evidenceName;

    public void Initialize(string name, string description, string information, string foundAt, string relationship, string notes)
    {
        this.thisName = name;
        this.description = description;
        this.information = information;
        this.foundAt = foundAt;
        this.relationship = relationship;
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






    public string GetName()
    {
        // 열거형 값을 문자열로 변환하고 '_' 문자를 공백으로 변경
        return this.evidenceName.ToString().Replace('_', ' ');
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

    public string GetRelationship()
    {
        return this.relationship;
    }

    public string GetNotets()
    {
        return this.notes;
    }
}