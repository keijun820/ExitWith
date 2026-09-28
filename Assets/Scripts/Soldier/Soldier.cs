using UnityEngine;

public class Soldier : MonoBehaviour
{
    float fireRate = 1.0f;        // 발사 쿨타임 (초)
    float fireRange = 100f;       // 사격 최대 사거리
    float pushForce = 10f;        // 오브젝트를 밀어내는 힘
    [SerializeField] LayerMask hittableLayers;     // 레이캐스트가 맞을 레이어 (몬스터, 오브젝트 등)
    [SerializeField] Transform cam;

    private float lastFireTime = -999f; // 마지막으로 발사한 시각? 시점? 저장 (쿨타임 계산용~~) (아직 미정)

    void Update()
    {
        HandleShooting();
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
        Ray ray = new Ray(cam.position, cam.forward);
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