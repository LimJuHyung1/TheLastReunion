//using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TMP : MonoBehaviour
{
    public float speed = 3f;
    public Transform cameraTransform;

    private Vector3 move;    
    private CharacterController characterController;    
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    void FixedUpdate()
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
        }
        else
        {
            anim.SetBool("isWalking", false);
        }
    }
}
