using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 3f;// 플레이어 이동 속도

    [Header("Managers")]
    public GameObject conversationManager;
    public Transform cameraTransform;
    public UIManager uIManager;

    private bool isTalking = false;     // 대화 중 여부
    private bool isMoving = false;      // 이동 중 여부
    private string evidenceLayerName = "Evidence"; // 감지할 레이어 이름
    private int evidenceLayer; // 감지할 레이어의 정수 값

    private Animator anim;    
    private Camera cam;
    private CharacterController characterController;
    private ConversationManager cm;

    [SerializeField]
    private GameObject npcRole;     // 현재 대화 중인 NPC 오브젝트
    private Vector3 move;           // 이동 방향 벡터

    void Start()
    {        
        anim = GetComponent<Animator>();
        cam = transform.GetChild(0).GetComponent<Camera>();
        characterController = GetComponent<CharacterController>();
        cm = conversationManager.GetComponent<ConversationManager>();

        evidenceLayer = LayerMask.NameToLayer(evidenceLayerName);
    }

    /// <summary>
    /// 플레이어 이동 처리
    /// - 대화 중이 아닐 경우에만 이동 가능
    /// - 카메라 방향을 기준으로 이동 방향 결정
    /// - 애니메이션을 이동 상태에 맞게 조정
    /// </summary>
    void FixedUpdate()
    {
        // 대화 중이 아닐 경우만 이동 및 마우스 회전 가능
        if (!isTalking)
        {
            // 캐릭터 이동
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // 카메라가 바라보는 방향을 기준으로 이동 방향 설정
            move = cameraTransform.right * horizontal + cameraTransform.forward * vertical;
            move.y = 0f; // y축 이동을 방지 (플레이어가 점프하지 않도록)
            characterController.Move(move * speed * Time.deltaTime);

            bool isWalking = move.sqrMagnitude > 0.01f;
            if (isWalking != isMoving) // 상태가 변경될 때만 애니메이션 업데이트
            {
                anim.SetBool("isWalking", isWalking);
                isMoving = isWalking;
            }
        }
    }


    /// <summary>
    /// 플레이어가 증거와 접촉했을 때 호출됨
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // 접촉한 오브젝트의 레이어를 확인
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
    /// 플레이어가 NPC와 대화 중일 때 증거 상호작용을 비활성화
    /// </summary>
    private void OnTriggerStay(Collider other)
    {
        if(cm.GetIsTalking())
            uIManager.IsAttachedToEvidenceProperty = false;
    }

    /// <summary>
    /// 플레이어가 증거 오브젝트의 트리거 영역을 벗어났을 때 호출됨
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        // 트리거 영역을 벗어났는지 확인
        if (other.gameObject.layer == evidenceLayer)
        {
            if (!other.GetComponent<Evidence>().GetIsFound())
                uIManager.IsAttachedToEvidenceProperty = false;
        }
    }






    //----------------------------------------------------------//

    /// <summary>
    /// 현재 대화 중인 NPC 설정
    /// </summary>
    public void GetNPCRole(GameObject gameObject)
    {
        this.npcRole = gameObject;
    }

    /// <summary>
    /// 현재 대화 중인 NPC 초기화 (대화 종료 시 호출)
    /// </summary>

    public void SetNullNPCRole()
    {
        this.npcRole = null;
    }

    //----------------------------------------------------------//

    /// <summary>
    /// 대화를 시작할 때 호출
    /// - 이동을 멈추고 대화 상태로 변경
    /// </summary>
    public void ActivateIsTalking()
    {
        move = Vector3.zero;
        isTalking = true;
        isMoving = false;
    }

    /// <summary>
    /// 대화 종료 후 호출 (다시 이동 가능)
    /// </summary>
    public void UnactivateIsTalking() { isTalking = false; }

    //----------------------------------------------------------//

    /// <summary>
    /// 플레이어가 바닥과 충돌할 때 호출됨
    /// - 현재 바닥의 레이어에 따라 다른 발소리 출력
    /// </summary>
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (isMoving)
        {
            string tmpLayer;

            // 충돌한 오브젝트의 레이어에 따라 발소리를 다르게 재생
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
    /// 플레이어가 NPC 방향으로 회전
    /// - `Vector3.ProjectOnPlane()`을 사용하여 y축 회전을 방지
    /// </summary>
    public void TurnTowardNPC(Transform npcTrans)
    {
        Vector3 direction = npcTrans.position - transform.position;
        direction = Vector3.ProjectOnPlane(direction, Vector3.up); // y축을 고려하지 않고 평면에서 방향만 설정
        transform.rotation = Quaternion.LookRotation(direction);

        cam.GetComponent<CameraScript>().FocusNPC(this.transform);
    }

    /// <summary>
    /// 대화 준비 상태로 변경 (이동을 멈추고 정지)
    /// </summary>
    public void ReadyConversation()
    {
        move = Vector3.zero;
        anim.SetBool("isWalking", false);
    }

    /// <summary>
    /// 현재 대화 중인지 여부 반환
    /// </summary>
    public bool GetIsTalking()
    {
        return isTalking;
    }

    //----------------------------------------------------------//

}
