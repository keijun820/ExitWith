using UnityEngine;

public class CameraSkill : MonoBehaviour
{
    [Header("레이캐스트 설정")]
    float rayDistance = 5f; // 레이를 쏠 거리

    GameObject currentTarget; // 현재 바라보고 있는 오브젝트
    GameObject targetObject;
    GameObject saveObject;
    [SerializeField] LayerMask InteractableLayer;
    private Vector3 savedPosition;
    private Quaternion savedRotation;
    private Vector3 savedScale;
    bool hasSavedData = false;

    [SerializeField] Transform cam;

    void Update()
    {
        // 바라보는 방향으로 레이캐스트 생성
        Ray ray = new Ray(cam.position, cam.forward);
        RaycastHit hit;

        // 레이캐스트 발사
        if (Physics.Raycast(ray, out hit, rayDistance, InteractableLayer))
        {
            GameObject hitObject = hit.collider.gameObject;
            Debug.Log(hitObject.name);

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

        if (Input.GetMouseButtonDown(0))
        {
            if(currentTarget != null)
            {
                SaveObjectState();
            }
        }

        if (Input.GetMouseButtonDown(1))
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
    }

    private void SpawnObjectFromSave()
    {
        // E키로 저장한 적이 없다면 생성하지 않음
        if (!hasSavedData) return;

        // 찍어놓은 오브젝트 생성
        GameObject newObject = Instantiate(saveObject, savedPosition, savedRotation);

        // 크기 적용
        newObject.transform.localScale = savedScale;

        Destroy(targetObject);

        Debug.Log("오브젝트 생성 완료");
    }
}
