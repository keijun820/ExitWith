using System;
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

    // 아이와 첫 대화를 마쳤는지 (Tab, Q 사용 가능 여부)
    bool AbilitiesUnlocked => GameProgress.Instance != null && GameProgress.Instance.AbilitiesUnlocked;

    // 대화창이 열려 있는지 (열려 있으면 이동/조작 정지)
    bool InDialogue => DialogueManager.Instance != null && DialogueManager.Instance.IsOpen;

    public void Start()
    {
        characterController = GetComponent<CharacterController>();

        kidControlling = false;
    }

    void Update()
    {
        // Tab, Q는 아이와 대화한 뒤에만, 그리고 대화 중이 아닐 때만 동작
        if (AbilitiesUnlocked && !InDialogue)
        {
            if (Input.GetKeyDown(KeyCode.Tab)) kidControlling = !kidControlling;
            if (Input.GetKeyDown(KeyCode.Q)) OnQPressed();
        }

        mesh.enabled = kidControlling;
        playerCamera.enabled = !kidControlling;
        kidCamera.enabled = kidControlling;

        // 아이 조종 중이거나 대화 중이면 플레이어는 제자리 (중력만 적용)
        if (kidControlling || InDialogue)
        {
            moveDirection.x = 0;
            moveDirection.z = 0;
            if (!characterController.isGrounded) moveDirection.y -= gravity * Time.deltaTime;

            characterController.Move(moveDirection * Time.deltaTime);
            return;
        }
        Control();
    }

    void OnQPressed()
    {
        // TODO: Q 키 기능을 여기에 작성하세요.
        Debug.Log("Q 사용");
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