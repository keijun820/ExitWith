using UnityEngine;

// 몬스터가 가져야 할 기본 스크립트
// - 군인 총에 맞으면 TakeDamage 호출됨
// - 총 세 발(maxHitCount) 맞으면 사망
public class Monster : MonoBehaviour
{
    [Header("체력 관련")]
    public int maxHitCount = 3;      // 죽는 데 필요한 피격 횟수
    private int currentHitCount = 0; // 현재까지 맞은 횟수

    [Header("이펙트 관련 (선택)")]
    public GameObject hitEffectPrefab; // 맞았을 때 재생할 이펙트 (없으면 비워둬도 됨)
    public GameObject deathEffectPrefab; // 죽었을 때 재생할 이펙트 (없으면 비워둬도 됨)

    // 외부(군인 스크립트 등)에서 데미지를 줄 때 호출하는 함수
    public void TakeDamage(int damage)
    {
        currentHitCount += damage;

        PlayHitEffect();

        if (currentHitCount >= maxHitCount)
        {
            Die();
        }
    }

    void PlayHitEffect()
    {
        // 맞을 때마다 이펙트 재생 (기획서: "맞을때마다 이펙트")
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }
    }

    void Die()
    {
        // 사망 이펙트 재생
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        // TODO: 사망 사운드 재생은 여기에 추가하면 됨

        Destroy(gameObject);
    }
}