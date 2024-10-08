using UnityEngine;

public class Evidence : Thing
{
    [SerializeField]
    private EvidenceManager evidenceManager;

    void Awake()
    {
        evidenceManager = GameObject.Find("EvidenceManager").GetComponent<EvidenceManager>();
        evidenceManager.SetEvidence(this);
    }

    public enum EvidenceName
    {
        독극물_용액이_든_유리병,
        제니의_연구_기록,
        앨런의_약_처방전,
        앨런의_책상에서_발견된_편지,
        네이슨의_서류_가방에서_발견된_법률_서류,
        미나의_메모,
        앨런의_집_주변에서_발견된_발자국,
        앨런의_컴퓨터에_표시된_이메일
    }

    public EvidenceName evidenceName;

    public void GetEvidence()
    {
        evidenceManager.FindEvidence(this);
        Debug.Log(name + "이 추가되었습니다.");

        // UI 상에 해당 오브젝트를 습득하였다는 문구 추가하기

        gameObject.SetActive(false);
    }

    public string GetEvidenceName()
    {
        // 열거형 값을 문자열로 변환하고 '_' 문자를 공백으로 변경
        return this.evidenceName.ToString().Replace('_', ' ');
    }
}
