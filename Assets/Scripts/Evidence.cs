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
        ���ع�_�����_����ִ�_������,
        ������_����_���,
        �ٷ���_��_ó����,
        �ٷ���_å�忡��_�߰ߵ�_����,
        ���̽���_����_���濡��_�߰ߵ�_����_����,
        �̳���_�޸�,
        �ٷ���_��_�ֺ�����_�߰ߵ�_���ڱ�,
        �ٷ���_��ǻ�Ϳ�_ǥ�õ�_�̸���,
        �ٷ���_����_�����ؾ�_��_�๰,
        �ջ��_�Ĺ�,
        �濵_����_�Ϻ�
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
        // ������ ���� ���ڿ��� ��ȯ�ϰ� '_' ���ڸ� �������� ����
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