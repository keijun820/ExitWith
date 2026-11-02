using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 발판(Pressure Plate)
/// - 지정한 태그의 오브젝트가 올라오면 "밟았다" 로그 출력
/// - 모두 내려가면 "내려왔다" 로그 출력
/// 이 오브젝트에는 Collider(Is Trigger 체크)가 있어야 합니다.
/// </summary>
[RequireComponent(typeof(Collider))]
public class PressurePlate : MonoBehaviour
{
    [Header("감지 설정")]
    [Tooltip("이 태그를 가진 오브젝트만 감지합니다. 비워두면 모든 오브젝트를 감지합니다.")]
    [SerializeField] private string targetTag = "Player";

    [Header("이벤트 (문 열기 등 연결용)")]
    public UnityEvent onPressed;
    public UnityEvent onReleased;

    // 현재 발판 위에 있는 오브젝트들 (여러 개가 동시에 올라와도 정상 동작하도록)
    private readonly HashSet<Collider> objectsOnPlate = new HashSet<Collider>();

    public bool IsPressed => objectsOnPlate.Count > 0;

    private void Reset()
    {
        // 컴포넌트를 처음 붙일 때 Collider를 자동으로 Trigger로 설정
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"트리거 감지됨: {other.name} / 태그: {other.tag}");

        if (!IsTarget(other)) return;

        bool wasPressed = IsPressed;
        objectsOnPlate.Add(other);

        // 처음 밟혔을 때만 로그/이벤트 (이미 누가 서 있으면 중복 출력 안 함)
        if (!wasPressed)
        {
            Debug.Log($"[{name}] {other.name}이(가) 발판을 밟았습니다!");
            onPressed?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!objectsOnPlate.Remove(other)) return;

        // 마지막 오브젝트가 내려갔을 때만 로그/이벤트
        if (!IsPressed)
        {
            Debug.Log($"[{name}] {other.name}이(가) 발판에서 내려왔습니다.");
            onReleased?.Invoke();
        }
    }

    private bool IsTarget(Collider other)
    {
        return string.IsNullOrEmpty(targetTag) || other.CompareTag(targetTag);
    }

    // 씬 뷰에서 발판 상태를 색으로 표시 (선택 사항)
    private void OnDrawGizmos()
    {
        Gizmos.color = Application.isPlaying && IsPressed ? Color.green : Color.yellow;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}