using Unity.VisualScripting;
using UnityEngine;

public class Evidence : MonoBehaviour
{
    private string thisName;
    private string description;
    private string information;
    private string relationship;
    private string importance;
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
        �ٷ���_����_�����ؾ�_��_�๰
    }

    public EvidenceName evidenceName;

    public void Initialize(string name, string description, string information, string relationship, string importance, string notes)
    {
        this.thisName = name;
        this.description = description;
        this.information = information;
        this.relationship = relationship;
        this.importance = importance;
        this.notes = notes;
    }

    public void GetEvidence()
    {
        Debug.Log(name + "�� �߰��Ǿ����ϴ�.");

        evidenceManager.FindEvidence(this);
        gameObject.SetActive(false);
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

    public string GetRelationship()
    {
        return this.relationship;
    }

    public string GetImportance()
    {
        return this.importance;
    }

    public string GetNotets()
    {
        return this.notes;
    }
}