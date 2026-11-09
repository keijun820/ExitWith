using UnityEngine;

/// <summary>
/// 풍선 아이템.
/// - 퀘스트 진행 중에 플레이어(또는 지정한 캐릭터)가 가까이 오면 습득
/// - 습득하면 플레이어가 풍선을 들고 있는 상태가 됨 (아이에게 건네야 퀘스트 클리어)
/// 거리로 판정하므로 트리거 콜라이더가 필요 없습니다.
/// </summary>
public class Balloon : MonoBehaviour
{
    [Header("습득 설정")]
    [Tooltip("풍선을 주울 수 있는 캐릭터들. 비워두면 PlayerController를 자동으로 찾습니다. 아이도 줍게 하려면 아이 오브젝트도 넣으세요.")]
    [SerializeField] Transform[] collectors;
    [SerializeField] float pickupRadius = 1.5f;
    [Tooltip("체크하면 퀘스트를 받은 뒤에만 주울 수 있습니다.")]
    [SerializeField] bool requireActiveQuest = true;

    [Header("둥실둥실 연출")]
    [SerializeField] float bobHeight = 0.25f;
    [SerializeField] float bobSpeed = 2f;
    [SerializeField] float spinSpeed = 45f;

    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;

        if (collectors == null || collectors.Length == 0)
        {
            PlayerController player = FindFirstObjectByType<PlayerController>();
            if (player != null) collectors = new[] { player.transform };
        }
    }

    void Update()
    {
        // 위아래로 둥실둥실 + 회전
        transform.position = startPos + Vector3.up * (Mathf.Sin(Time.time * bobSpeed) * bobHeight);
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);

        if (requireActiveQuest &&
            (GameProgress.Instance == null || GameProgress.Instance.BalloonQuest != QuestState.InProgress))
            return;

        foreach (Transform c in collectors)
        {
            if (c == null || !c.gameObject.activeInHierarchy) continue;

            if (Vector3.Distance(c.position, transform.position) <= pickupRadius)
            {
                Collect(c);
                return;
            }
        }
    }

    void Collect(Transform collector)
    {
        Debug.Log($"{collector.name}이(가) 풍선을 습득했습니다!");
        GameProgress.Instance?.PickUpBalloon();
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}