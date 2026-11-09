using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 발판(Pressure Plate)
/// - 밟으면 OnPlatePressed() 함수 실행
/// - 내려오면 OnPlateReleased() 함수 실행
/// 각 함수 안에서 Debug.Log로 로그를 출력합니다.
/// 이 스크립트는 감지용 오브젝트(Sensor)에 붙이세요.
/// </summary>
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class PressurePlate : MonoBehaviour
{
    [Tooltip("이 태그를 가진 오브젝트만 감지합니다. 비워두면 모든 오브젝트를 감지합니다.")]
    [SerializeField] string targetTag = "Player";

    // 현재 발판 위에 있는 오브젝트들 (여러 개가 올라와도 정상 동작하도록)
    readonly HashSet<Collider> objectsOnPlate = new HashSet<Collider>();

    public bool IsPressed => objectsOnPlate.Count > 0;

    // 컴포넌트를 처음 붙일 때 자동 설정
    void Reset()
    {
        SetupComponents();
    }

    void Awake()
    {
        SetupComponents();
    }

    // Character Controller와도 확실히 감지되도록 Trigger + Kinematic Rigidbody로 설정
    void SetupComponents()
    {
        GetComponent<BoxCollider>().isTrigger = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsTarget(other)) return;

        bool wasPressed = IsPressed;
        objectsOnPlate.Add(other);

        // 아무도 없다가 처음 밟혔을 때만 실행
        if (!wasPressed) OnPlatePressed(other);
    }

    void OnTriggerExit(Collider other)
    {
        if (!objectsOnPlate.Remove(other)) return;

        // 마지막 오브젝트가 내려갔을 때만 실행
        if (!IsPressed) OnPlateReleased(other);
    }

    bool IsTarget(Collider other)
    {
        return string.IsNullOrEmpty(targetTag) || other.CompareTag(targetTag);
    }

    // ───────── 발판을 밟았을 때 실행되는 함수 ─────────
    void OnPlatePressed(Collider who)
    {
        Debug.Log($"[{name}] {who.name}이(가) 발판을 밟았습니다!");

        // 여기에 밟았을 때 할 일을 추가하세요. (문 열기, 소리 재생 등)
    }

    // ───────── 발판에서 내려왔을 때 실행되는 함수 ─────────
    void OnPlateReleased(Collider who)
    {
        Debug.Log($"[{name}] {who.name}이(가) 발판에서 내려왔습니다.");

        // 여기에 내려왔을 때 할 일을 추가하세요.
    }
}