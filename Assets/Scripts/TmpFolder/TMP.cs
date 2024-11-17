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
        }
        else
        {
            anim.SetBool("isWalking", false);
        }
    }
}
