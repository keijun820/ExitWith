using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float walkingSpeed = 10f;
    float jumpSpeed = 8.0f;
    float gravity = 20.0f;
    float lookSpeed = 2.0f;
    float lookXLimit = 45.0f;

    [SerializeField] CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;
    [SerializeField] Camera playerCamera;
    [SerializeField] Camera kidCamera;
    public bool kidControlling;

    [SerializeField] MeshRenderer mesh;


    public void Start()
    {
        characterController = GetComponent<CharacterController>();

        kidControlling = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) kidControlling = !kidControlling;
        mesh.enabled = kidControlling;
        playerCamera.enabled = !kidControlling;
        kidCamera.enabled = kidControlling;
        if (kidControlling)
        {
            moveDirection.x = 0;
            moveDirection.z = 0;
            if (!characterController.isGrounded) moveDirection.y -= gravity * Time.deltaTime;

            characterController.Move(moveDirection * Time.deltaTime);
            return;
        }
        Control();
    }

    void Control()
    {
        // We are grounded, so recalculate move direction based on axis
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = walkingSpeed * Input.GetAxis("Vertical");
        float curSpeedY = walkingSpeed * Input.GetAxis("Horizontal");
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetKeyDown(KeyCode.Space) && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (playerCamera != null)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }
}