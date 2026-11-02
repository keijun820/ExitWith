using UnityEngine;

// CameraSkill.cs의 레이캐스트 + 윤곽선 방식을 그대로 사용합니다.
// 바라보고 있는 아이템에 윤곽선을 띄우고, 그 상태에서 E를 누르면 습득합니다.
// 카메라(또는 플레이어) 오브젝트에 붙이세요.
public class ItemInteractor : MonoBehaviour
{
    [Header("레이캐스트 설정")]
    public float rayDistance = 5f; // 레이를 쏠 거리
    public LayerMask interactableLayer; // 감지할 오브젝트의 레이어

    public GameObject currentTarget; // 현재 바라보고 있는 오브젝트

    [Header("인벤토리")]
    public SimpleInventory inventory; // 습득한 아이템을 넣을 인벤토리 (인스펙터에서 플레이어를 연결)

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
            if (currentTarget != null)
            {
                TryInteract();
            }
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

    // 지금 바라보고 있는(윤곽선이 켜진) 대상과 상호작용.
    // 대상이 Item이면 줍기, Door면 열기를 시도합니다.
    private void TryInteract()
    {
        Item item = currentTarget.GetComponent<Item>();
        if (item != null)
        {
            TryPickup(item);
            return;
        }

        Door door = currentTarget.GetComponent<Door>();
        if (door != null)
        {   
            door.TryOpen(inventory); // 맞는 열쇠가 있으면 열리고, 없으면 Door.cs에서 "OO 이(가) 없습니다" 로그를 찍음
            return;
        }
    }

    private void TryPickup(Item item)
    {
        // 열쇠면 AddKey, 일반 아이템이면 AddItem 사용. 둘 다 true = 성공, false = 실패
        bool obtained = item.isKey ? inventory.AddKey(item.itemName) : inventory.AddItem(item.itemName);

        if (obtained)
        {
            GameObject pickedUp = currentTarget;
            ClearHighlight(); // 윤곽선 끄고 currentTarget 정리
            Destroy(pickedUp); // 얻었으면 월드에서 사라짐
        }
        else
        {
            Debug.Log(item.itemName + " 을(를) 얻지 못했습니다.");
        }
    }
}