using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 3f;

    public GameObject conversationManager;
    public Transform cameraTransform;
    public UIManager uIManager;

    [SerializeField] private bool isTalking = false;    
    private bool isMoving = false;
    private string evidenceLayerName = "Evidence"; // 감지할 레이어 이름
    private int evidenceLayer; // 감지할 레이어의 정수 값


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
        // 접촉한 오브젝트의 레이어를 확인
        if (other.gameObject.layer == evidenceLayer)
        {
            if (other.GetComponent<Evidence>() != null)
            {
                if (!other.GetComponent<Evidence>().GetIsFound())
                    uIManager.IsAttachedToEvidenceProperty = true;
            }
            else
                Debug.Log("널 :" + other.name);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(cm.GetIsTalking())
            uIManager.IsAttachedToEvidenceProperty = false;
    }

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
    /// 플레이어가 바닥에 닿고 있는 오브젝트의 레이어에 따라
    /// 다른 소리 출력
    /// </summary>
    /// <param name="hit"></param>
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
    /// NPC 방향으로 rotate
    /// </summary>
    public void TurnTowardNPC(Transform npcTrans)
    {
        // 현재 오브젝트의 위치
        Vector3 targetPosition = npcTrans.position;

        // 현재 오브젝트의 y 위치는 변경하지 않음
        targetPosition.y = transform.position.y;        

        // 타겟 위치를 바라보도록 회전
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
