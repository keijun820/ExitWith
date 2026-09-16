using UnityEngine;

// 과학자 기믹 컨트롤러
// - 기본 이동 (WASD) : 기존 컨트롤러와 동일한 방식
// - 좌클릭 : 철 오브젝트를 밀어냄 (플레이어와 약간 떨어진 거리까지)
// - 우클릭 (누르고 있는 동안) : 철 오브젝트를 끌어당겨서 플레이어 바로 앞에 고정
//   - 끌어온 상태로 화면(시점)을 움직여도 오브젝트가 계속 따라옴
//   - 우클릭을 떼면 오브젝트를 놓아줌 (물리 적용 재개)
public class ScientistController : MonoBehaviour
{
    [Header("이동 관련")]
    public float walkingSpeed = 7.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    [Header("자력 기믹 관련")]
    public float interactRange = 8f;      // 밀기/끌기 대상을 찾는 최대 사거리
    public float pushForce = 8f;          // 밀어내는 힘
    public float holdDistance = 2f;       // 끌어온 오브젝트를 고정할 위치 (플레이어 바로 앞)
    public float pullSpeed = 10f;         // 오브젝트가 고정 위치로 이동하는 속도
    public LayerMask magneticLayer;       // 철 오브젝트가 속한 레이어

    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;

    [SerializeField]
    private float cameraYOffset = 0.4f;
    private Camera playerCamera;

    private Rigidbody heldObject = null; // 현재 끌어당기고 있는 오브젝트

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        playerCamera = Camera.main;
        playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y + cameraYOffset, transform.position.z);
        playerCamera.transform.SetParent(transform);
    }

    void Update()
    {
        HandleMovement();
        HandleLook();
        HandleMagnetInput();
    }

    // ---------- 이동 (WASD) ----------
    void HandleMovement()
    {
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
    }

    // ---------- 시점 회전 (마우스) ----------
    void HandleLook()
    {
        if (playerCamera == null) return;

        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
    }

    // ---------- 좌클릭 밀기 / 우클릭 끌어당기기 입력 처리 ----------
    void HandleMagnetInput()
    {
        // 좌클릭 : 밀어내기 (오브젝트를 들고 있지 않을 때만)
        if (Input.GetMouseButtonDown(0) && heldObject == null)
        {
            TryPush();
        }

        // 우클릭 : 끌어당기기 시작
        if (Input.GetMouseButtonDown(1) && heldObject == null)
        {
            TryStartPull();
        }

        // 우클릭을 떼면 놓아주기
        if (Input.GetMouseButtonUp(1) && heldObject != null)
        {
            ReleaseObject();
        }

        // 끌어당기는 중이면 매 프레임 위치를 플레이어 앞으로 갱신
        if (heldObject != null)
        {
            HoldObjectInFront();
        }
    }

    void TryPush()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange, magneticLayer))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // 플레이어에게서 약간 떨어진 거리까지 밀어냄 (자석 같은 극끼리 밀어내는 느낌)
                Vector3 pushDirection = (hit.point - playerCamera.transform.position).normalized;
                rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
            }
        }
    }

    void TryStartPull()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange, magneticLayer))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                heldObject = rb;
                heldObject.useGravity = false; // 끌려오는 동안 중력 끄기
            }
        }
    }

    void HoldObjectInFront()
    {
        // 플레이어 바로 앞(카메라 기준) 고정 위치 계산
        Vector3 targetPosition = playerCamera.transform.position + playerCamera.transform.forward * holdDistance;

        // 속도를 이용해서 부드럽게 목표 위치로 이동
        heldObject.linearVelocity = (targetPosition - heldObject.position) * pullSpeed;
    }

    void ReleaseObject()
    {
        heldObject.useGravity = true; // 중력 다시 켜기
        heldObject = null;
    }
}