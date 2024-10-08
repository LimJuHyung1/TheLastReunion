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
        ���ع�_�����_��_������,
        ������_����_���,
        �ٷ���_��_ó����,
        �ٷ���_å�󿡼�_�߰ߵ�_����,
        ���̽���_����_���濡��_�߰ߵ�_����_����,
        �̳���_�޸�,
        �ٷ���_��_�ֺ�����_�߰ߵ�_���ڱ�,
        �ٷ���_��ǻ�Ϳ�_ǥ�õ�_�̸���
    }

    public EvidenceName evidenceName;

    public void GetEvidence()
    {
        evidenceManager.FindEvidence(this);
        Debug.Log(name + "�� �߰��Ǿ����ϴ�.");

        // UI �� �ش� ������Ʈ�� �����Ͽ��ٴ� ���� �߰��ϱ�

        gameObject.SetActive(false);
    }

    public string GetEvidenceName()
    {
        // ������ ���� ���ڿ��� ��ȯ�ϰ� '_' ���ڸ� �������� ����
        return this.evidenceName.ToString().Replace('_', ' ');
    }
}
