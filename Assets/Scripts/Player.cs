using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 3f;// �÷��̾� �̵� �ӵ�

    [Header("Managers")]
    public GameObject conversationManager;
    public Transform cameraTransform;
    public UIManager uIManager;

    private bool isTalking = false;     // ��ȭ �� ����
    private bool isMoving = false;      // �̵� �� ����
    private string evidenceLayerName = "Evidence"; // ������ ���̾� �̸�
    private int evidenceLayer; // ������ ���̾��� ���� ��

    private Animator anim;    
    private Camera cam;
    private CharacterController characterController;
    private ConversationManager cm;

    [SerializeField]
    private GameObject npcRole;     // ���� ��ȭ ���� NPC ������Ʈ
    private Vector3 move;           // �̵� ���� ����

    void Start()
    {        
        anim = GetComponent<Animator>();
        cam = transform.GetChild(0).GetComponent<Camera>();
        characterController = GetComponent<CharacterController>();
        cm = conversationManager.GetComponent<ConversationManager>();

        evidenceLayer = LayerMask.NameToLayer(evidenceLayerName);
    }

    /// <summary>
    /// �÷��̾� �̵� ó��
    /// - ��ȭ ���� �ƴ� ��쿡�� �̵� ����
    /// - ī�޶� ������ �������� �̵� ���� ����
    /// - �ִϸ��̼��� �̵� ���¿� �°� ����
    /// </summary>
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

            bool isWalking = move.sqrMagnitude > 0.01f;
            if (isWalking != isMoving) // ���°� ����� ���� �ִϸ��̼� ������Ʈ
            {
                anim.SetBool("isWalking", isWalking);
                isMoving = isWalking;
            }
        }
    }


    /// <summary>
    /// �÷��̾ ���ſ� �������� �� ȣ���
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // ������ ������Ʈ�� ���̾ Ȯ��
        if (other.gameObject.layer == evidenceLayer)
        {
            Evidence evidence = other.GetComponent<Evidence>();
        if (evidence != null && !evidence.GetIsFound())
        {
            uIManager.IsAttachedToEvidenceProperty = true;
        }
        }
    }

    /// <summary>
    /// �÷��̾ NPC�� ��ȭ ���� �� ���� ��ȣ�ۿ��� ��Ȱ��ȭ
    /// </summary>
    private void OnTriggerStay(Collider other)
    {
        if(cm.GetIsTalking())
            uIManager.IsAttachedToEvidenceProperty = false;
    }

    /// <summary>
    /// �÷��̾ ���� ������Ʈ�� Ʈ���� ������ ����� �� ȣ���
    /// </summary>
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

    /// <summary>
    /// ���� ��ȭ ���� NPC ����
    /// </summary>
    public void GetNPCRole(GameObject gameObject)
    {
        this.npcRole = gameObject;
    }

    /// <summary>
    /// ���� ��ȭ ���� NPC �ʱ�ȭ (��ȭ ���� �� ȣ��)
    /// </summary>

    public void SetNullNPCRole()
    {
        this.npcRole = null;
    }

    //----------------------------------------------------------//

    /// <summary>
    /// ��ȭ�� ������ �� ȣ��
    /// - �̵��� ���߰� ��ȭ ���·� ����
    /// </summary>
    public void ActivateIsTalking()
    {
        move = Vector3.zero;
        isTalking = true;
        isMoving = false;
    }

    /// <summary>
    /// ��ȭ ���� �� ȣ�� (�ٽ� �̵� ����)
    /// </summary>
    public void UnactivateIsTalking() { isTalking = false; }

    //----------------------------------------------------------//

    /// <summary>
    /// �÷��̾ �ٴڰ� �浹�� �� ȣ���
    /// - ���� �ٴ��� ���̾ ���� �ٸ� �߼Ҹ� ���
    /// </summary>
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
    /// �÷��̾ NPC �������� ȸ��
    /// - `Vector3.ProjectOnPlane()`�� ����Ͽ� y�� ȸ���� ����
    /// </summary>
    public void TurnTowardNPC(Transform npcTrans)
    {
        Vector3 direction = npcTrans.position - transform.position;
        direction = Vector3.ProjectOnPlane(direction, Vector3.up); // y���� ������� �ʰ� ��鿡�� ���⸸ ����
        transform.rotation = Quaternion.LookRotation(direction);

        cam.GetComponent<CameraScript>().FocusNPC(this.transform);
    }

    /// <summary>
    /// ��ȭ �غ� ���·� ���� (�̵��� ���߰� ����)
    /// </summary>
    public void ReadyConversation()
    {
        move = Vector3.zero;
        anim.SetBool("isWalking", false);
    }

    /// <summary>
    /// ���� ��ȭ ������ ���� ��ȯ
    /// </summary>
    public bool GetIsTalking()
    {
        return isTalking;
    }

    //----------------------------------------------------------//

}
