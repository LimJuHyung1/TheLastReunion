using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 3f;

    public GameObject conversationManager;
    public Transform cameraTransform;
    public UIManager uIManager;

    [SerializeField] private bool isTalking = false;    
    private bool isMoving = false;
    private string evidenceLayerName = "Evidence"; // ������ ���̾� �̸�
    private int evidenceLayer; // ������ ���̾��� ���� ��


    private Animator anim;    
    private Camera cam;
    private CharacterController characterController;
    private ConversationManager cm;
    [SerializeField]
    private GameObject npcRole;
    private Vector3 move;

    void Start()
    {        
        anim = GetComponent<Animator>();
        cam = transform.GetChild(0).GetComponent<Camera>();
        characterController = GetComponent<CharacterController>();
        cm = conversationManager.GetComponent<ConversationManager>();

        evidenceLayer = LayerMask.NameToLayer(evidenceLayerName);
    }

    void FixedUpdate()
    {
        // ��ȭ ���� �ƴ� ��츸 �̵� �� ���콺 ȸ�� ����
        if (!isTalking)
        {
            // ĳ���� �̵�
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // ī�޶� �ٶ󺸴� ������ �������� �̵� ���� ����
            move = cameraTransform.right * horizontal + cameraTransform.forward * vertical;
            move.y = 0f; // y�� �̵��� ���� (�÷��̾ �������� �ʵ���)
            characterController.Move(move * speed * Time.deltaTime);

            if (move != Vector3.zero)
            {
                anim.SetBool("isWalking", true);
                isMoving = true;
            }
            else
            {
                anim.SetBool("isWalking", false);
                isMoving = false;
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        // ������ ������Ʈ�� ���̾ Ȯ��
        if (other.gameObject.layer == evidenceLayer)
        {
            if (other.GetComponent<Evidence>() != null)
            {
                if (!other.GetComponent<Evidence>().GetIsFound())
                    uIManager.IsAttachedToEvidenceProperty = true;
            }
            else
                Debug.Log("�� :" + other.name);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(cm.GetIsTalking())
            uIManager.IsAttachedToEvidenceProperty = false;
    }

    private void OnTriggerExit(Collider other)
    {
        // Ʈ���� ������ ������� Ȯ��
        if (other.gameObject.layer == evidenceLayer)
        {
            if (!other.GetComponent<Evidence>().GetIsFound())
                uIManager.IsAttachedToEvidenceProperty = false;
        }
    }






    //----------------------------------------------------------//

    public void GetNPCRole(GameObject gameObject)
    {
        this.npcRole = gameObject;
    }

    public void SetNullNPCRole()
    {
        this.npcRole = null;
    }

    //----------------------------------------------------------//

    public void ActivateIsTalking()
    {
        move = Vector3.zero;
        isTalking = true;
        isMoving = false;
    }
    
    public void UnactivateIsTalking() { isTalking = false; }

    //----------------------------------------------------------//

    /// <summary>
    /// �÷��̾ �ٴڿ� ��� �ִ� ������Ʈ�� ���̾ ����
    /// �ٸ� �Ҹ� ���
    /// </summary>
    /// <param name="hit"></param>
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (isMoving)
        {
            string tmpLayer;

            // �浹�� ������Ʈ�� ���̾ ���� �߼Ҹ��� �ٸ��� ���
            if (hit.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                tmpLayer = "Ground";
                SoundManager.Instance.PlayFootStepSound("Ground");
            }
            else if (hit.gameObject.layer == LayerMask.NameToLayer("Tile"))
            {
                tmpLayer = "Tile";
                SoundManager.Instance.PlayFootStepSound("Tile");
            }
            else if (hit.gameObject.layer == LayerMask.NameToLayer("House"))
            {
                tmpLayer = "House";
                SoundManager.Instance.PlayFootStepSound("House");
            }
        }
    }

    //----------------------------------------------------------//

    /// <summary>
    /// NPC �������� rotate
    /// </summary>
    public void TurnTowardNPC(Transform npcTrans)
    {
        // ���� ������Ʈ�� ��ġ
        Vector3 targetPosition = npcTrans.position;

        // ���� ������Ʈ�� y ��ġ�� �������� ����
        targetPosition.y = transform.position.y;        

        // Ÿ�� ��ġ�� �ٶ󺸵��� ȸ��
        transform.LookAt(targetPosition);
        cam.GetComponent<CameraScript>().FocusNPC(this.transform);
    }

    public void ReadyConversation()
    {
        move = Vector3.zero;
    }

    public bool GetIsTalking()
    {
        return isTalking;
    }

    //----------------------------------------------------------//

}
