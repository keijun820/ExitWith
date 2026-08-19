using UnityEngine;

public class CameraSkill : MonoBehaviour
{
    [Header("레이캐스트 설정")]
    public float rayDistance = 5f; // 레이를 쏠 거리
    public LayerMask interactableLayer; // 감지할 오브젝트의 레이어

    public GameObject currentTarget; // 현재 바라보고 있는 오브젝트

    public GameObject targetObject;
    public GameObject saveObject;

    private Vector3 savedPosition;
    private Quaternion savedRotation;
    private Vector3 savedScale;
    private bool hasSavedData = false;

    void Update()
    {
        // 바라보는 방향으로 레이캐스트 생성
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // 디버그용 선 그리기 (게임 뷰에서는 안 보이고 씬 뷰에서만 보임)
        Debug.DrawRay(transform.position, transform.forward * rayDistance, Color.red);

        // 레이캐스트 발사
        if (Physics.Raycast(ray, out hit, rayDistance, interactableLayer))
        {
            GameObject hitObject = hit.collider.gameObject;

            // 새로운 오브젝트를 바라보게 되었을 때
            if (hitObject != currentTarget)
            {
                ClearHighlight(); // 기존 오브젝트 윤곽선 끄기
                currentTarget = hitObject;
                HighlightObject(currentTarget); // 새 오브젝트 윤곽선 켜기
            }
        }
        else
        {
            // 아무것도 바라보지 않을 때
            ClearHighlight();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if(currentTarget != null)
            {
                SaveObjectState();
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            SpawnObjectFromSave();
        }
    }

    // 윤곽선을 켜는 함수
    private void HighlightObject(GameObject obj)
    {
        // 대상 오브젝트에서 Outline 컴포넌트를 가져옴
        Outline outline = obj.GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = true; // 윤곽선 활성화
        }
    }

    // 윤곽선을 끄는 함수
    private void ClearHighlight()
    {
        if (currentTarget != null)
        {
            Outline outline = currentTarget.GetComponent<Outline>();
            if (outline != null)
            {
                outline.enabled = false; // 윤곽선 비활성화
            }
            currentTarget = null;
        }
    }

    private void SaveObjectState()
    {
        // 트랜스폼 데이터 저장
        savedPosition = currentTarget.transform.position;
        savedRotation = currentTarget.transform.rotation;
        savedScale = currentTarget.transform.localScale;

        targetObject = currentTarget;
        saveObject = targetObject;

        hasSavedData = true;
        Debug.Log("찰칵");
    }

    private void SpawnObjectFromSave()
    {
        // E키로 저장한 적이 없다면 생성하지 않음
        if (!hasSavedData)
        {
            Debug.LogWarning("찍어놓은 오브젝트가 없음");
            return;
        }

        // 찍어놓은 오브젝트 생성
        GameObject newObject = Instantiate(saveObject, savedPosition, savedRotation);

        // 크기 적용
        newObject.transform.localScale = savedScale;

        Destroy(targetObject);

        Debug.Log("오브젝트 생성 완료");
    }
}
