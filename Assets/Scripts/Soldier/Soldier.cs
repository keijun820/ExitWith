using UnityEngine;

// 군인 기믹 컨트롤러
//기본 이동 (WASD) : 기존 PlayerController와 동일한 방식(다른 스크립트로 대체~)
//좌클릭 : 총 발사 (히트스캔 방식, 1초 쿨타임)
//몬스터에게 맞으면 데미지 (3발 맞으면 사망)
//멀리 있는 오브젝트를 맞추면 밀어냄 (새총으로 캔 맞추는 느낌)
public class SoldierController : MonoBehaviour
{
    // 테스트용 PlayerController
    public float walkingSpeed = 7.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    public float fireRate = 1.0f;        // 발사 쿨타임 (초)
    public float fireRange = 100f;       // 사격 최대 사거리
    public float pushForce = 10f;        // 오브젝트를 밀어내는 힘
    public LayerMask hittableLayers;     // 레이캐스트가 맞을 레이어 (몬스터, 오브젝트 등)

    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero; // 시작 좌표 정하는 거
    private float rotationX = 0;

    // 임시 카메라 각도
    private float cameraYOffset = 0.4f;
    private Camera playerCamera;

    private float lastFireTime = -999f; // 마지막으로 발사한 시각? 시점? 저장 (쿨타임 계산용~~) (아직 미정)

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        playerCamera = Camera.main;
        playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y + cameraYOffset, transform.position.z);
        playerCamera.transform.SetParent(transform); //카메라를 캐릭터에 고정시켜서 같이 작동핟게 함.
    }

    void Update()
    {
        HandleMovement();
        HandleLook();
        HandleShooting();
    }

    // ----------(테스트용) 이동 (WASD) ----------
    void HandleMovement()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward); //앞
        Vector3 right = transform.TransformDirection(Vector3.right); // 오른쪽

        float curSpeedX = walkingSpeed * Input.GetAxis("Vertical");   // W/S 속도
        float curSpeedY = walkingSpeed * Input.GetAxis("Horizontal"); // A/D 속도
        float movementDirectionY = moveDirection.y; //중력(점프) 임시

        moveDirection = (forward * curSpeedX) + (right * curSpeedY); // 최종 이동 방향

        if (Input.GetKeyDown(KeyCode.Space) && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed; //스페이드 점프
        }
        else
        {
            moveDirection.y = movementDirectionY; // 기존 속도 유지
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime; // 공중일 때 중력 적용시키는 거
        }

        characterController.Move(moveDirection * Time.deltaTime); //최최종 이동
    }

    // ---------- 시점 회전 (마우스) ----------
    void HandleLook()
    {
        if (playerCamera == null) return;

        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed; //상하로 회전하는 거 누적
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit); //시야각 제한
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0); // 카메라만 상하 회전
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0); // 몸통 좌우로 회전
    }

    // ---------- 좌클릭 사격 ----------
    void HandleShooting()
    {
        // 좌클릭 안 눌렀으면 리턴
        if (!Input.GetMouseButtonDown(0)) return;

        // 쿨타임이 아직 안 지났으면 발사 못 함
        if (Time.time < lastFireTime + fireRate) return;

        lastFireTime = Time.time; // 발사 시각 갱신

        Fire();
    }

    void Fire()
    {
        // 카메라가 바라보는 정중앙으로 레이캐스트 발사 (히트스캔 = 총알 이동시간 없이 즉시 판정)
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        // TODO: 총구 이펙트, 발사 사운드는 여기서 재생하면 됨

        if (Physics.Raycast(ray, out hit, fireRange, hittableLayers))
        {
            // 몬스터를 맞췄을 경우
            Monster monster = hit.collider.GetComponent<Monster>();
            if (monster != null)
            {
                monster.TakeDamage(1); // 한 발당 1 데미지, 3발 맞으면 사망 (Monster 스크립트에서 처리)
                // TODO: 피격 이펙트 재생
                return;
            }

            // 밀 수 있는 오브젝트를 맞췄을 경우
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // 총을 쏜 방향(레이 방향)으로 밀어냄
                rb.AddForce(ray.direction * pushForce, ForceMode.Impulse);
            }
        }
    }
}