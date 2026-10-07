using UnityEngine;

// 과학자 기믹 컨트롤러
// - 기본 이동 (WASD) : 기존 컨트롤러와 동일한 방식
// - 좌클릭 : 철 오브젝트를 밀어냄 (플레이어와 약간 떨어진 거리까지)
// - 우클릭 (누르고 있는 동안) : 플레이어 기준 일정 거리 안에서 "가장 가까운" 철 오브젝트를 끌어당겨서 플레이어 바로 앞에 고정
//   - 조준(레이캐스트)이 아니라 플레이어 주변 범위 안에서 찾음
//   - 우클릭을 누른 그 순간에만 타겟을 정하고, 누르고 있는 동안은 다른 물체가 더 가까워져도 타겟이 바뀌지 않음
//   - 끌어온 상태로 화면(시점)을 움직여도 오브젝트가 계속 따라옴
//   - 우클릭을 떼면 오브젝트를 놓아줌 (물리 적용 재개)
public class ScientistController : MonoBehaviour
{

    [Header("자력 기믹 관련")]
    public float interactRange = 8f;      // 밀기/끌기 대상을 찾는 최대 사거리
    public float pushForce = 8f;          // 밀어내는 힘
    public float holdDistance = 2f;       // 끌어온 오브젝트를 고정할 위치 (플레이어 바로 앞)
    public float pullSpeed = 10f;         // 오브젝트가 고정 위치로 이동하는 속도
    public LayerMask magneticLayer;       // 철 오브젝트가 속한 레이어


    [SerializeField] Transform playerCamera;

    private Rigidbody heldObject = null; // 현재 끌어당기고 있는 오브젝트


    void Update()
    {
        HandleMagnetInput();
    }


    // ---------- 좌클릭 밀기 / 우클릭 끌어당기기 입력 처리 ----------
    void HandleMagnetInput()
    {
        // 좌클릭 : 밀어내기 (오브젝트를 들고 있지 않을 때만) — 기존 그대로
        if (Input.GetMouseButtonDown(0) && heldObject == null)
        {
            TryPush();
        }

        // 우클릭을 누른 "그 순간"에만 가장 가까운 오브젝트를 찾아 타겟으로 정함
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
        // (여기서는 새 타겟을 다시 찾지 않으므로, 누르고 있는 동안 다른 물체가 더 가까워져도 타겟이 안 바뀜)
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

                // 테스트용 로그: 실제로 힘을 가했는지, 그 오브젝트 물리 상태는 어떤지 확인
                Debug.Log($"[자력] {rb.name} 에 힘 가함 | 방향={pushDirection}, 크기={pushForce}, " +
                          $"isKinematic={rb.isKinematic}, constraints={rb.constraints}");
            }
            else
            {
                Debug.Log($"[자력] {hit.collider.name} 에 맞았지만 Rigidbody가 없음"); // 테스트용 로그
            }
        }
        else
        {
            Debug.Log("[자력] 좌클릭 - 레이캐스트가 아무것도 못 맞춤"); // 테스트용 로그
        }
    }

    // 조준(레이캐스트)이 아니라, 플레이어를 중심으로 interactRange 반경 안에 있는
    // magneticLayer 오브젝트들 중 "가장 가까운" 것을 찾아서 끌어당길 타겟으로 정함
    void TryStartPull()
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, interactRange, magneticLayer);
        Debug.Log($"[자력] 우클릭 감지됨. 범위 안에서 찾은 오브젝트 수: {nearbyObjects.Length}"); // 테스트용 로그

        Rigidbody nearest = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider col in nearbyObjects)
        {
            Rigidbody rb = col.GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.Log($"[자력] {col.gameObject.name} 에는 Rigidbody가 없어서 건너뜀"); // 테스트용 로그
                continue;
            }

            float distance = Vector3.Distance(transform.position, rb.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = rb;
            }
        }

        if (nearest != null)
        {
            heldObject = nearest;
            heldObject.useGravity = false; // 끌려오는 동안 중력 끄기

            // 테스트용 로그: 이 오브젝트의 물리 상태를 한 번에 확인
            Debug.Log($"[자력] {heldObject.name} 을(를) 잡음 | " +
                      $"isKinematic={heldObject.isKinematic}, " +
                      $"constraints={heldObject.constraints}, " +
                      $"mass={heldObject.mass}, " +
                      $"collisionDetectionMode={heldObject.collisionDetectionMode}, " +
                      $"현재 위치={heldObject.position}");
        }
        else
        {
            Debug.Log("[자력] 범위 안에 끌어당길 대상이 없음"); // 테스트용 로그
        }
    }

    void HoldObjectInFront()
    {
        // 플레이어 바로 앞(카메라 기준) 고정 위치 계산
        Vector3 targetPosition = playerCamera.position + playerCamera.forward * holdDistance;

        // 속도를 이용해서 부드럽게 목표 위치로 이동
        heldObject.linearVelocity = (targetPosition - heldObject.position) * pullSpeed;
    }

    void ReleaseObject()
    {
        heldObject.useGravity = true; // 중력 다시 켜기
        heldObject = null;
    }
}