using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour
{
    public ConversationManager conversationManager;
    public UIManager uIManager;    

    int[] layers = new int[4];
    bool isThisEvidence = false;
    float mouseSensitivity = 180f; // ���콺 ����
    float xRotation = 0f;
    [SerializeField] Transform playerBody;          // �÷��̾��� Transform (ī�޶� �پ����� ���)
    Player player;

    void Start()
    {
        // �÷��̾��� Transform�� Player ��ũ��Ʈ ��������
        playerBody = transform.parent;
        player = playerBody.GetComponent<Player>();
    }

    void FixedUpdate()
    {
        if (!player.GetIsTalking())
        {
            // ���콺 �Է��� �޾Ƶ���
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            // ���콺 Y�� �̵��� ���� ī�޶��� ���� ȸ�� ���
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // ���� 90�� �̻� ���� �ʵ��� ����

            // ī�޶��� ���� ȸ�� ���� (���� ȸ��)
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            // ���콺 X�� �̵��� ���� �÷��̾��� �¿� ȸ�� ��� �� ����
            playerBody.Rotate(Vector3.up * mouseX);           
        }        
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (!player.GetIsTalking())
        {
            if (IsAbleToShowDescription(other.gameObject.layer))
            {
                uIManager.ShowKeyAndDescriontion(other.gameObject);
            }
        }        
    }    

    void OnTriggerStay(Collider other)
    {
        if (!player.GetIsTalking())
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (IsEvidenceLayer(other.gameObject.layer))
                {
                    isThisEvidence = false;

                    // Thing ������Ʈ�� �ִ��� üũ�� ��, ���� �ڷ�ƾ ����
                    Evidence evidenceComponent = other.GetComponent<Evidence>();

                    if (evidenceComponent != null)
                    {
                        StartCoroutine(ShowDescription(evidenceComponent));
                    }
                }

                else if (IsNPCLayer(other.gameObject.layer))
                {
                    NPCRole npcRole = other.GetComponent<NPCRole>();
                    player.ActivateIsTalking();
                    conversationManager.GetNPCRole(npcRole);
                    conversationManager.AddListenersResponse();
                    conversationManager.StartConversation();
                    conversationManager.ShowName();
                }
                uIManager.HideKeyAndDescriontion();
            }
            else if (Input.GetKeyDown(KeyCode.F))
            {
                if (IsDoorLayer(other.gameObject.layer))
                {
                    other.GetComponent<Door>().Open();
                    uIManager.HideKeyAndDescriontion();
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!player.GetIsTalking())
        {
            if (IsAbleToShowDescription(other.gameObject.layer))
            {
                isThisEvidence = false;

                uIManager.HideKeyAndDescriontion();
            }
        }            
    }

    //------------------------------------------------------------------------//

    /// <summary>
    /// ������Ʈ�� ������ UI�� ǥ��
    /// </summary>
    /// <param ���� ������Ʈ="evidence"></param>
    /// <returns></returns>
    IEnumerator ShowDescription(Evidence evidence)
    {
        evidence.GetComponent<Evidence>().GetEvidence();
        yield return StartCoroutine
            (uIManager.ShowDescription(evidence.GetDescription()));
        // isThisThing = false;
    }

    bool IsEvidenceLayer(int layer)
    {
        return layer == LayerMask.NameToLayer("Evidence");
    }

    bool IsNPCLayer(int layer)
    {
        return layer == LayerMask.NameToLayer("NPC");
    }

    bool IsDoorLayer(int layer)
    {
        return layer == LayerMask.NameToLayer("Door");
    }

    bool IsAbleToShowDescription(int layer)
    {
        string[] validLayers = { "Evidence", "NPC", "Door" };

        foreach (string layerName in validLayers)
        {
            if (layer == LayerMask.NameToLayer(layerName))
            {
                return true;
            }
        }

        return false;
    }

    //------------------------------------------------------------------------//

    /// <summary>
    /// Player ��ũ��Ʈ���� ȣ��
    /// </summary>
    /// <param �÷��̾� transform="playerTrans"></param>
    public void FocusNPC(Transform playerTrans)
    {
        Vector3 direction = playerTrans.forward;

        // ī�޶� �ٶ󺸴� ������ y��ǥ�� ��¦ ����ϴ�.
        direction.y -= 0.5f;

        // ���ο� ���� ���͸� ����Ͽ� ī�޶� ȸ�� ����
        transform.rotation = Quaternion.LookRotation(direction.normalized);
    }
}
