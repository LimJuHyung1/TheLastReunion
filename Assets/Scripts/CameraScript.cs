using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour
{
    public ConversationManager conversationManager;
    public UIManager uIManager;    

    int[] layers = new int[4];
    bool isThisEvidence = false;
    float mouseSensitivity = 180f; // 마우스 감도
    float xRotation = 0f;
    [SerializeField] Transform playerBody;          // 플레이어의 Transform (카메라가 붙어있을 대상)
    Player player;

    void Start()
    {
        // 플레이어의 Transform과 Player 스크립트 가져오기
        playerBody = transform.parent;
        player = playerBody.GetComponent<Player>();
    }

    void FixedUpdate()
    {
        if (!player.GetIsTalking())
        {
            // 마우스 입력을 받아들임
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            // 마우스 Y축 이동에 따라 카메라의 상하 회전 계산
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // 고개를 90도 이상 꺾지 않도록 제한

            // 카메라의 상하 회전 적용 (로컬 회전)
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            // 마우스 X축 이동에 따라 플레이어의 좌우 회전 계산 및 적용
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

                    // Thing 컴포넌트가 있는지 체크한 후, 설명 코루틴 실행
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
    /// 오브젝트의 설명을 UI에 표시
    /// </summary>
    /// <param 증거 오브젝트="evidence"></param>
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
    /// Player 스크립트에서 호출
    /// </summary>
    /// <param 플레이어 transform="playerTrans"></param>
    public void FocusNPC(Transform playerTrans)
    {
        Vector3 direction = playerTrans.forward;

        // 카메라가 바라보는 방향의 y좌표를 살짝 낮춥니다.
        direction.y -= 0.5f;

        // 새로운 방향 벡터를 사용하여 카메라 회전 설정
        transform.rotation = Quaternion.LookRotation(direction.normalized);
    }
}
